<template>
  <div class="consulta-form card">
    <div class="card-header">
      <h2>Nueva Consulta Veterinaria</h2>
      <button class="card-close" @click="cerrar">×</button>
    </div>
    <form @submit.prevent="guardar" class="card-body">
      <!-- Especie Animal -->
      <div class="form-group">
        <label for="especieAnimalId">Especie</label>
        <select id="especieAnimalId" v-model="dto.especieAnimalId" required>
          <option value="">Seleccione una especie</option>
          <option v-for="e in especies" :key="e.id" :value="e.id">{{ e.nombre }}</option>
        </select>
        <p v-if="submitted && !dto.especieAnimalId" class="error">Seleccione una especie</p>
      </div>

      <!-- Tratamiento -->
      <div class="form-group">
        <label for="tratamientoId">Tratamiento</label>
        <select id="tratamientoId" v-model="dto.tratamientoId" required>
          <option value="">Seleccione un tratamiento</option>
          <option v-for="t in tratamientos" :key="t.id" :value="t.id">{{ t.nombre }}</option>
        </select>
        <p v-if="submitted && !dto.tratamientoId" class="error">Seleccione un tratamiento</p>
      </div>

      <!-- Nombre de Mascota -->
      <div class="form-group">
        <label for="nombreMascota">Nombre de Mascota</label>
        <input id="nombreMascota" v-model="dto.nombreMascota" type="text" placeholder="Ej: Rocky" required />
        <p v-if="submitted && !dto.nombreMascota.trim()" class="error">El nombre de la mascota es obligatorio</p>
      </div>

      <!-- Nombre de Propietario -->
      <div class="form-group">
        <label for="nombrePropietario">Nombre del Propietario</label>
        <input id="nombrePropietario" v-model="dto.nombrePropietario" type="text" placeholder="Ej: Juan Pérez" required />
        <p v-if="submitted && !dto.nombrePropietario.trim()" class="error">El nombre del propietario es obligatorio</p>
      </div>

      <!-- Información de Contacto -->
      <div class="form-group">
        <label for="informacionContacto">Información de Contacto</label>
        <input id="informacionContacto" v-model="dto.informacionContacto" type="text" placeholder="Ej: 555-1234" required />
        <p v-if="submitted && !dto.informacionContacto.trim()" class="error">La información de contacto es obligatoria</p>
      </div>

      <!-- Costo -->
      <div class="form-group">
        <label for="costo">Costo</label>
        <input id="costo" v-model.number="dto.costo" type="number" min="0" placeholder="0.00" required />
        <p v-if="submitted && dto.costo < 0" class="error">El costo debe ser positivo</p>
      </div>

      <!-- Descripción -->
      <div class="form-group">
        <label for="descripcion">Descripción</label>
        <textarea id="descripcion" v-model="dto.descripcion" placeholder="Descripción de la consulta" required></textarea>
        <p v-if="submitted && !dto.descripcion.trim()" class="error">La descripción es obligatoria</p>
      </div>

      <!-- Notas y Prescripción -->
      <div class="form-group">
        <label for="notasTratamiento">Notas del Tratamiento</label>
        <textarea id="notasTratamiento" v-model="dto.notasTratamiento" placeholder="Notas adicionales"></textarea>
      </div>
      <div class="form-group">
        <label for="prescripcion">Prescripción</label>
        <textarea id="prescripcion" v-model="dto.prescripcion" placeholder="Detalle de la prescripción"></textarea>
      </div>

      <!-- Fecha de Consulta -->
      <div class="form-group">
        <label for="fechaConsulta">Fecha de Consulta</label>
        <input id="fechaConsulta" v-model="dto.fechaConsulta" type="date" required />
        <p v-if="submitted && !dto.fechaConsulta" class="error">La fecha de consulta es obligatoria</p>
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
import { useConsultasStore } from '@/stores/consultas';
import apiService from '@/services/api-service';
import type { ConsultaVeterinariaDto } from '@/types/consultas';
import type { EspecieAnimal } from '@/types/especies';
import type { Tratamiento } from '@/types/tratamientos';

const emit = defineEmits(['cerrar','guardado']);
const store = useConsultasStore();

const loading = ref(false);
const submitted = ref(false);
const error = ref('');
const especies = ref<EspecieAnimal[]>([]);
const tratamientos = ref<Tratamiento[]>([]);
const dto = ref<ConsultaVeterinariaDto>({
  especieAnimalId: 0,
  tratamientoId: 0,
  nombreMascota: '',
  nombrePropietario: '',
  informacionContacto: '',
  costo: 0,
  descripcion: '',
  notasTratamiento: '',
  prescripcion: '',
  fechaConsulta: new Date().toISOString().substring(0,10)
});

async function cargarListas() {
  try {
    especies.value = await apiService.getEspecies();
    tratamientos.value = await apiService.getTratamientos();
  } catch (e) {
    console.error('Error al cargar listas:', e);
  }
}

onMounted(cargarListas);

async function guardar() {
  submitted.value = true;
  loading.value = true;
  error.value = '';
  const errors: string[] = [];
  if (!dto.value.especieAnimalId) errors.push('Seleccione una especie');
  if (!dto.value.tratamientoId) errors.push('Seleccione tratamiento');
  if (!dto.value.nombreMascota.trim()) errors.push('El nombre de la mascota es obligatorio');
  if (!dto.value.nombrePropietario.trim()) errors.push('El nombre del propietario es obligatorio');
  if (!dto.value.informacionContacto.trim()) errors.push('La información de contacto es obligatoria');
  if (dto.value.costo < 0) errors.push('El costo debe ser positivo');
  if (!dto.value.descripcion.trim()) errors.push('La descripción es obligatoria');
  if (!dto.value.fechaConsulta) errors.push('La fecha de consulta es obligatoria');
  if (errors.length) { error.value = errors.join(', '); loading.value = false; return; }
  try {
    await store.crearConsulta(dto.value);
    emit('guardado');
    cerrar();
  } catch (e: any) {
    error.value = e.response?.data?.mensaje || 'Error creando consulta';
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
  max-width: 500px;
  margin: auto;
}
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}
.card-close {
  background: transparent;
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
