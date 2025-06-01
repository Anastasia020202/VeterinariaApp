<template>
  <div class="tratamiento-form card">
    <div class="card-header">
      <h2>{{ props.mode === 'edit' ? 'Editar Tratamiento' : 'Nuevo Tratamiento' }}</h2>
      <button class="card-close" @click="cerrar">×</button>
    </div>
    <form @submit.prevent="guardar" class="card-body">
      <div class="form-group">
        <label for="nombre">Nombre</label>
        <input id="nombre" v-model="dto.nombre" type="text" placeholder="Nombre del tratamiento" required />
      </div>
      <div class="form-group">
        <label for="descripcion">Descripción</label>
        <textarea id="descripcion" v-model="dto.descripcion" placeholder="Descripción del tratamiento" required></textarea>
      </div>
      <div class="form-group inline">
        <label for="costo">Costo Estándar</label>
        <input id="costo" v-model.number="dto.costoEstandar" type="number" min="0" placeholder="0.00" required />
      </div>
      <div class="form-group inline">
        <label><input type="checkbox" v-model="dto.activo" /> Activo</label>
      </div>
      <div class="form-actions">
        <button type="button" @click="cerrar" class="btn btn-secondary">Cancelar</button>
        <button type="submit" :disabled="loading" class="btn btn-primary">{{ loading ? (props.mode === 'edit' ? 'Actualizando...' : 'Guardando...') : (props.mode === 'edit' ? 'Actualizar' : 'Guardar') }}</button>
      </div>
      <p v-if="error" class="error">{{ error }}</p>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, defineProps } from 'vue';
import { useTratamientosStore } from '@/stores/tratamientos';
import type { TratamientoDto } from '@/types/tratamientos';

// Aceptamos una prop 'mode' para distinguir entre creación y edición
const props = defineProps({
  mode: { type: String, default: 'create' },
  dto: { type: Object as () => TratamientoDto, default: () => ({
    nombre: '',
    descripcion: '',
    costoEstandar: 0,
    activo: true
  }) }
});

const emit = defineEmits(['cerrar','guardado']);
const store = useTratamientosStore();

const loading = ref(false);
const error = ref('');

// Si estamos en modo edición, usamos el objeto recibido en props; caso contrario, iniciamos uno nuevo.
const dto = ref<TratamientoDto>(props.dto);

async function guardar() {
  loading.value = true;
  error.value = '';
  try {
    if (props.mode === 'edit') {
      // Actualización: se asume que dto.id existe
      await store.actualizarTratamiento(dto.value.id!, dto.value);
    } else {
      // Creación
      await store.crearTratamiento(dto.value);
    }
    emit('guardado');
    cerrar();
  } catch (e: any) {
    error.value = e.response?.data?.mensaje || (props.mode === 'edit' ? 'Error actualizando tratamiento' : 'Error creando tratamiento');
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

.card-header h2 {
  margin: 0;
  font-size: 1.5rem;
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

input[type="text"], input[type="number"], textarea {
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
  transition: background-color 0.3s ease, transform 0.2s ease;
  font-size: 1rem;
}

.btn-primary {
  background: #007bff;
  color: #fff;
}

.btn-primary:hover {
  background: #0056b3;
  transform: scale(1.02);
}

.btn-secondary {
  background: #6c757d;
  color: #fff;
}

.btn-secondary:hover {
  background: #50575e;
  transform: scale(1.02);
}

.btn--danger {
  background: #dc3545;
  color: #fff;
}

.btn--danger:hover {
  background: #a71d2a;
  transform: scale(1.02);
}

.error {
  color: #c00;
  font-size: 0.9rem;
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
