<template>
  <div class="plan-form card">
    <div class="card-header">
      <h2>Nuevo Plan de Salud</h2>
      <button class="card-close" @click="cerrar">×</button>
    </div>
    <form @submit.prevent="guardar" class="card-body">
      <!-- Nombre -->
      <div class="form-group">
        <label for="nombre">Nombre</label>
        <input id="nombre" v-model="dto.nombre" type="text" placeholder="Ej: Plan Básico" required />
        <p v-if="submitted && !dto.nombre.trim()" class="error">El nombre es obligatorio</p>
      </div>
      <!-- Tratamiento -->
      <div class="form-group">
        <label for="tratamiento">Tratamiento</label>
        <select id="tratamiento" v-model="dto.tratamientoId" required>
          <option value="">Seleccione un tratamiento</option>
          <option v-for="t in tratamientos" :key="t.id" :value="t.id">{{ t.nombre }}</option>
        </select>
        <p v-if="submitted && !dto.tratamientoId" class="error">Seleccione un tratamiento</p>
      </div>
      <!-- Descripción -->
      <div class="form-group">
        <label for="descripcion">Descripción</label>
        <textarea id="descripcion" v-model="dto.descripcion" placeholder="Cobertura, detalles, etc." required></textarea>
        <p v-if="submitted && !dto.descripcion.trim()" class="error">La descripción es obligatoria</p>
      </div>
      <!-- Costo -->
      <div class="form-group">
        <label for="costo">Costo</label>
        <input id="costo" v-model.number="dto.costo" type="number" min="0" placeholder="0.00" required />
        <p v-if="submitted && dto.costo < 0" class="error">El costo debe ser positivo</p>
      </div>
      <!-- Duración (meses) -->
      <div class="form-group">
        <label for="duracionMeses">Duración (meses)</label>
        <input id="duracionMeses" v-model.number="dto.duracionMeses" type="number" min="1" max="120" placeholder="12" required />
        <p v-if="submitted && (dto.duracionMeses < 1 || dto.duracionMeses > 120)" class="error">La duración debe estar entre 1 y 120</p>
      </div>
      <!-- Visitas Incluidas -->
      <div class="form-group inline">
        <label for="visitasIncluidas">Visitas Incluidas</label>
        <input id="visitasIncluidas" v-model.number="dto.visitasIncluidas" type="number" min="0" placeholder="0" required />
      </div>
      <!-- Fechas -->
      <div class="form-group">
        <label for="fechaInicio">Fecha Inicio</label>
        <input id="fechaInicio" v-model="dto.fechaInicio" type="date" required />
        <p v-if="submitted && !dto.fechaInicio" class="error">Fecha de inicio obligatoria</p>
      </div>
      <div class="form-group">
        <label for="fechaFin">Fecha Fin</label>
        <input id="fechaFin" v-model="dto.fechaFin" type="date" required />
        <p v-if="submitted && !dto.fechaFin" class="error">Fecha de fin obligatoria</p>
      </div>
      <!-- Activo -->
      <div class="form-group">
        <label><input type="checkbox" v-model="dto.activo" /> Activo</label>
      </div>
      <!-- Acciones -->
      <div class="form-actions">
        <button type="button" @click="cerrar" class="btn btn-secondary">Cancelar</button>
        <button type="submit" :disabled="loading" class="btn btn-primary">{{ loading ? 'Guardando...' : 'Guardar' }}</button>
      </div>
      <p v-if="error" class="error">{{ error }}</p>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { usePlanesStore } from '@/stores/planes';
import apiService from '@/services/api-service';
import type { PlanSaludDto } from '@/types/planes';
import type { Tratamiento } from '@/types/tratamientos';

const emit = defineEmits(['cerrar','guardado']);
const store = usePlanesStore();

const loading = ref(false);
const submitted = ref(false);
const error = ref('');
const tratamientos = ref<Tratamiento[]>([]);
const dto = ref<PlanSaludDto>({
  nombre: '',
  tratamientoId: 0,
  descripcion: '',
  costo: 0,
  duracionMeses: 12,
  visitasIncluidas: 0,
  fechaInicio: new Date().toISOString().substr(0,10),
  fechaFin: new Date().toISOString().substr(0,10),
  activo: true
});

async function cargarListas() {
  try {
    tratamientos.value = await apiService.getTratamientos();
  } catch (e) {
    console.error('Error al cargar tratamientos:', e);
  }
}

onMounted(cargarListas);

async function guardar() {
  submitted.value = true;
  loading.value = true;
  error.value = '';
  const errors: string[] = [];
  if (!dto.value.nombre.trim()) errors.push('Nombre obligatorio');
  if (!dto.value.tratamientoId) errors.push('Seleccione tratamiento');
  if (!dto.value.descripcion.trim()) errors.push('Descripción obligatoria');
  if (dto.value.costo < 0) errors.push('Costo positivo');
  if (dto.value.duracionMeses < 1 || dto.value.duracionMeses > 120) errors.push('Duración inválida');
  if (!dto.value.fechaInicio) errors.push('Fecha inicio');
  if (!dto.value.fechaFin) errors.push('Fecha fin');
  if (dto.value.fechaInicio >= dto.value.fechaFin) errors.push('La fecha de inicio debe ser anterior a la fecha de fin');
  if (errors.length) { error.value = errors.join(', '); loading.value = false; return; }
  try {
    await store.crearPlan(dto.value);
    emit('guardado');
    cerrar();
  } catch (e: any) {
    error.value = e.response?.data?.mensaje || 'Error creando plan';
  } finally {
    loading.value = false;
  }
}

function cerrar() {
  emit('cerrar');
}
</script>

<style scoped>
.card {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0,0,0,0.1);
  padding: 1.5rem;
  max-width: 600px;
  margin: auto;
}
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}
.card-close {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
}
.card-body {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}
.form-group {
  display: flex;
  flex-direction: column;
  margin-bottom: 1rem;
}
.form-group.inline {
  flex-direction: row;
  align-items: center;
  gap: 0.5rem;
}
.form-group label {
  font-weight: 500;
  margin-bottom: 0.3rem;
}
input[type="text"],
input[type="number"],
input[type="date"], textarea, select {
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}
textarea {
  resize: vertical;
  min-height: 80px;
}
.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
}
.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}
.btn-primary {
  background: #007bff;
  color: #fff;
}
.btn-secondary {
  background: #6c757d;
  color: #fff;
}
.error {
  color: #c00;
  font-size: 0.9rem;
  margin-top: 0.5rem;
}
@media (max-width: 600px) {
  .card {
    padding: 1rem;
    margin: 0 1rem;
  }
  .form-group.inline {
    flex-direction: column;
    align-items: flex-start;
  }
}
</style>
