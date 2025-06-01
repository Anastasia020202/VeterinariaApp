<template>
  <div class="planes">
    <div class="planes__header">
      <div>
        <h1 class="planes__title">Planes de Salud</h1>
        <p class="planes__subtitle">Gestiona los planes de salud disponibles para mascotas</p>
      </div>
      <button @click="mostrarFormulario = true" class="btn btn--primary planes__add">
        + Nuevo Plan
      </button>
    </div>
    <PlanForm
      v-if="mostrarFormulario"
      @cerrar="mostrarFormulario = false"
      @guardado="onGuardado"
    />
    <div class="planes__content">
      <div v-if="store.loading" class="loading">Cargando planes de salud...</div>
      <div v-else-if="store.error" class="error">
        <p>{{ store.error }}</p>
        <button @click="cargar" class="btn btn--primary">Reintentar</button>
      </div>
      <div v-else-if="store.planes.length === 0" class="empty">
        <p>No hay planes de salud registrados.</p>
      </div>
      <table v-else class="planes__table">
        <thead>
          <tr>
            <th>Nombre</th>
            <th>Tratamiento</th>
            <th>Descripción</th>
            <th>Costo</th>
            <th>Duración</th>
            <th>Visitas</th>
            <th>Activo</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="p in store.planes" :key="p.id">
            <td>{{ p.nombre }}</td>
            <td>{{ p.nombreTratamiento }}</td>
            <td>{{ p.descripcion }}</td>
            <td>{{ p.costo }}</td>
            <td>{{ p.duracionMeses }} m</td>
            <td>{{ p.visitasIncluidas }}</td>
            <td>{{ p.activo ? 'Sí' : 'No' }}</td>
            <td>
              <button @click="onEliminar(p.id)" class="btn btn--danger">Eliminar</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { usePlanesStore } from '@/stores/planes';
import PlanForm from '@/components/planes/PlanForm.vue';

const store = usePlanesStore();
const mostrarFormulario = ref(false);

const cargar = async () => {
  try { await store.obtenerPlanes(); } catch (e) { console.error('Error cargando planes:', e); }
};
onMounted(cargar);

const onGuardado = () => {
  cargar();
  mostrarFormulario.value = false;
};

async function onEliminar(id: number) {
  if (!confirm('¿Eliminar este plan de salud?')) return;
  try { await store.eliminarPlan(id); cargar(); } catch(e){console.error('Error eliminando plan:',e);}  
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables';
@import '@/assets/styles/mixins';
.planes__add { @include button($bg: $primary-color); }
.planes {
  &__header { display: flex; justify-content: space-between; align-items: center; margin-bottom: $spacing-unit; }
  &__title { font-size: 2rem; color: $primary-color; margin-bottom: calc($spacing-unit/4); }
  &__subtitle { font-size: 1.2rem; color: $secondary-color; }
  &__content { @include card; padding: $spacing-unit; }
  &__table { width:100%; border-collapse:collapse; th,td{ padding:calc($spacing-unit/2); border-bottom:1px solid $border-color;} th{background:rgba($primary-color,0.05);font-weight:600;} }
}
.loading, .error, .empty { text-align:center; padding:$spacing-unit; }
.error p { color:$danger-color; }
</style>
