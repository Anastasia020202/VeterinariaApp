<template>
  <div class="consultas">
    <div class="consultas__header">
      <div>
        <h1 class="consultas__title">Consultas Veterinarias</h1>
        <p class="consultas__subtitle">Gestiona las consultas y visitas de pacientes</p>
      </div>
      <button @click="mostrarFormulario = true" class="btn btn--primary consultas__add">
        + Nueva Consulta
      </button>
    </div>
    <!-- Formulario modal de creación -->
    <ConsultaForm
      v-if="mostrarFormulario"
      @cerrar="mostrarFormulario = false"
      @guardado="onGuardado"
    />
    <div class="consultas__content">
      <div v-if="store.loading" class="loading">Cargando consultas...</div>
      <div v-else-if="store.error" class="error">
        <p>{{ store.error }}</p>
        <button @click="cargar" class="btn btn--primary">Reintentar</button>
      </div>
      <div v-else-if="store.consultas.length === 0" class="empty">
        <p>No hay consultas registradas.</p>
      </div>
      <table v-else class="consultas__table">
        <thead>
          <tr>
            <th>Fecha</th>
            <th>Mascota</th>
            <th>Especie</th>
            <th>Tratamiento</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="c in store.consultas" :key="c.id">
            <td>{{ formatoFecha(c.fechaConsulta) }}</td>
            <td>{{ c.nombreMascota }}</td>
            <td>{{ c.nombreEspecieAnimal }}</td>
            <td>{{ c.nombreTratamiento }}</td>
            <td>
              <button @click="onEliminar(c.id)" class="btn btn--danger">Eliminar</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import apiService from '@/services/api-service';
import { useConsultasStore } from '@/stores/consultas';
import ConsultaForm from '@/components/consultas/ConsultaForm.vue';

const store = useConsultasStore();
const mostrarFormulario = ref(false);

const cargar = async () => {
  try { await store.obtenerConsultas(); } catch (e) { console.error('Error cargando consultas:', e); }
};
onMounted(cargar);

const onGuardado = () => {
  cargar();
  mostrarFormulario.value = false;
};

// Función eliminar
async function onEliminar(id: number) {
  if (!confirm('¿Eliminar esta consulta?')) return;
  try { await store.eliminarConsulta(id); cargar(); }
  catch(e){console.error('Error eliminando consulta:',e);}
}

// Formatear fecha
function formatoFecha(fecha: string): string {
  return new Date(fecha).toLocaleDateString();
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables';
@import '@/assets/styles/mixins';
.tratamientos__add,
.consultas__add {
  @include button($bg: $primary-color);
}
.consultas {
  &__header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: $spacing-unit;
  }
  &__title { font-size: 2rem; color: $primary-color; margin-bottom: calc($spacing-unit/4); }
  &__subtitle { font-size: 1.2rem; color: $secondary-color; }
  &__content { @include card; padding: $spacing-unit; }
  &__table {
    width: 100%; border-collapse: collapse;
    th, td { padding: calc($spacing-unit/2); border-bottom: 1px solid $border-color; }
    th { background: rgba($primary-color,0.05); font-weight: 600; }
  }
}
.loading, .error, .empty { text-align: center; padding: $spacing-unit; }
.error p { color: $danger-color; }
</style>
