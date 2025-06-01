<template>
  <div class="especies">
    <div class="especies__header">
      <div>
        <h1 class="especies__title">Especies de Animales</h1>
        <p class="especies__subtitle">Gestiona las diferentes especies registradas en la clínica</p>
      </div>
      
      <button @click="mostrarFormulario = true" class="btn btn--primary especies__add-button">
        <span class="especies__add-icon">+</span>
        Nueva Especie
      </button>
    </div>
    
    <!-- Filtros y búsqueda -->
    <div class="especies__filters">
      <div class="search-box">
        <input 
          type="text" 
          v-model="busqueda" 
          placeholder="Buscar especies..." 
          class="search-box__input"
          @input="filtrarEspecies"
        />
      </div>
      
      <div class="filter-controls">
        <label class="filter-label">
          <input type="checkbox" v-model="mostrarSoloActivos" @change="filtrarEspecies">
          Mostrar solo activos
        </label>
      </div>
    </div>
    
    <!-- Listado de especies -->
    <div class="especies__content">
      <div v-if="especiesStore.loading" class="especies__loading">
        <p>Cargando especies...</p>
      </div>
      
      <div v-else-if="especiesStore.error" class="especies__error">
        <p>{{ especiesStore.error }}</p>
        <button @click="cargarEspecies" class="btn btn--primary">
          Reintentar
        </button>
      </div>
      
      <div v-else-if="especiesFiltradas.length === 0" class="especies__empty">
        <p v-if="busqueda || mostrarSoloActivos">
          No se encontraron especies con los filtros actuales.
        </p>
        <p v-else>
          Aún no hay especies registradas.
        </p>
        <button @click="mostrarFormulario = true" class="btn btn--primary">
          Agregar primera especie
        </button>
      </div>
      
      <table v-else class="especies__table">
        <thead>
          <tr>
            <th>Nombre</th>
            <th>Descripción</th>
            <th>Estado</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="especie in especiesFiltradas" :key="especie.id" :class="{ 'inactive-row': !especie.activo }">
            <td>{{ especie.nombre }}</td>
            <td class="descripcion-cell">{{ especie.descripcion }}</td>
            <td>
              <span 
                class="estado-badge" 
                :class="especie.activo ? 'estado-badge--activo' : 'estado-badge--inactivo'"
              >
                {{ especie.activo ? 'Activo' : 'Inactivo' }}
              </span>
            </td>
            <td>
              <div class="acciones">
                <button 
                  @click="editarEspecie(especie.id)" 
                  class="accion-btn accion-btn--editar"
                  title="Editar especie"
                >
                  ✏️
                </button>
                <button 
                  @click="confirmarEliminar(especie)" 
                  class="accion-btn accion-btn--eliminar"
                  title="Eliminar especie"
                >
                  ❌
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    
    <!-- Modal para formulario de especie -->
    <div v-if="mostrarFormulario" class="modal">
      <div class="modal__backdrop" @click="mostrarFormulario = false"></div>
      <div class="modal__content">
        <EspecieForm 
          :especie-id="especieEditandoId" 
          @cerrar="mostrarFormulario = false" 
          @guardado="onEspecieGuardada"
        />
      </div>
    </div>
    
    <!-- Modal de confirmación para eliminar -->
    <div v-if="mostrarConfirmacion" class="modal">
      <div class="modal__backdrop" @click="mostrarConfirmacion = false"></div>
      <div class="modal__content modal__content--sm">
        <div class="confirmacion">
          <h3 class="confirmacion__title">Confirmar eliminación</h3>
          <p class="confirmacion__message">
            ¿Estás seguro de que deseas eliminar la especie "{{ especieEliminar?.nombre }}"?
          </p>
          <div class="confirmacion__actions">
            <button @click="mostrarConfirmacion = false" class="btn btn--secondary">
              Cancelar
            </button>
            <button @click="eliminarEspecie" class="btn btn--danger">
              {{ eliminando ? 'Eliminando...' : 'Eliminar' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useEspeciesStore } from '@/stores/especies';
import EspecieForm from '@/components/especies/EspecieForm.vue';
import type { EspecieAnimal } from '@/types/especies';

// Estado
const especiesStore = useEspeciesStore();
const busqueda = ref('');
const mostrarSoloActivos = ref(false);
const mostrarFormulario = ref(false);
const especieEditandoId = ref<number | null>(null);
const mostrarConfirmacion = ref(false);
const especieEliminar = ref<EspecieAnimal | null>(null);
const eliminando = ref(false);

// Especies filtradas
const especiesFiltradas = computed(() => {
  let resultado = [...especiesStore.especies];
  
  if (busqueda.value) {
    const busquedaLower = busqueda.value.toLowerCase();
    resultado = resultado.filter(e => 
      e.nombre.toLowerCase().includes(busquedaLower) || 
      e.descripcion.toLowerCase().includes(busquedaLower)
    );
  }
  
  if (mostrarSoloActivos.value) {
    resultado = resultado.filter(e => e.activo);
  }
  
  return resultado;
});

// Cargar especies al montar el componente
onMounted(() => {
  cargarEspecies();
});

// Métodos
async function cargarEspecies() {
  try {
    await especiesStore.obtenerEspecies();
  } catch (error) {
    console.error('Error al cargar especies:', error);
  }
}

function filtrarEspecies() {
  // La filtración ocurre automáticamente gracias al computed
}

function editarEspecie(id: number) {
  especieEditandoId.value = id;
  mostrarFormulario.value = true;
}

function confirmarEliminar(especie: EspecieAnimal) {
  especieEliminar.value = especie;
  mostrarConfirmacion.value = true;
}

async function eliminarEspecie() {
  if (!especieEliminar.value) return;
  
  eliminando.value = true;
  try {
    await especiesStore.eliminarEspecie(especieEliminar.value.id);
    mostrarConfirmacion.value = false;
  } catch (error) {
    console.error('Error al eliminar especie:', error);
  } finally {
    eliminando.value = false;
  }
}

function onEspecieGuardada() {
  // Recargar la lista para obtener datos actualizados
  cargarEspecies();
  // Resetear el ID de edición
  especieEditandoId.value = null;
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables';
@import '@/assets/styles/mixins';

.especies {
  &__header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    margin-bottom: $spacing-unit;
  }
  
  &__title {
    font-size: 2rem;
    color: $primary-color;
    margin-bottom: calc($spacing-unit / 4);
  }
  
  &__subtitle {
    font-size: 1.2rem;
    color: $secondary-color;
    margin-bottom: 0;
  }
  
  &__add-button {
    display: flex;
    align-items: center;
    gap: calc($spacing-unit / 2);
  }
  
  &__add-icon {
    font-size: 1.2rem;
    font-weight: bold;
  }
  
  &__filters {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: $spacing-unit;
    flex-wrap: wrap;
    gap: $spacing-unit;
  }
  
  &__content {
    @include card;
    padding: $spacing-unit;
  }
  
  &__loading, &__error, &__empty {
    text-align: center;
    padding: $spacing-unit;
    
    p {
      margin-bottom: $spacing-unit;
      color: $secondary-color;
    }
  }
  
  &__table {
    width: 100%;
    border-collapse: collapse;
    
    th, td {
      padding: calc($spacing-unit / 2);
      text-align: left;
      border-bottom: 1px solid $border-color;
    }
    
    th {
      font-weight: bold;
      color: $secondary-color;
    }
    
    tbody tr:hover {
      background-color: rgba($primary-color, 0.05);
    }
  }
}

.search-box {
  position: relative;
  
  &__input {
    @include form-control;
    padding-left: calc($spacing-unit * 1.5);
    width: 250px;
  }
  
  &::before {
    content: 'ὐD';
    position: absolute;
    left: $spacing-unit / 2;
    top: 50%;
    transform: translateY(-50%);
    color: $secondary-color;
    opacity: 0.6;
  }
}

.filter-label {
  display: flex;
  align-items: center;
  gap: calc($spacing-unit / 2);
  cursor: pointer;
}

.descripcion-cell {
  max-width: 300px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.estado-badge {
  display: inline-block;
  padding: calc($spacing-unit / 4) calc($spacing-unit / 2);
  border-radius: $border-radius;
  font-size: $font-size-small;
  font-weight: 500;
  
  &--activo {
    background-color: rgba($success-color, 0.15);
    color: $success-color;
  }
  
  &--inactivo {
    background-color: rgba($danger-color, 0.15);
    color: $danger-color;
  }
}

.acciones {
  display: flex;
  gap: calc($spacing-unit / 2);
}

.accion-btn {
  background: none;
  border: none;
  cursor: pointer;
  font-size: 1rem;
  padding: calc($spacing-unit / 4);
  border-radius: $border-radius;
  transition: all 0.2s ease;
  
  &--editar:hover {
    background-color: rgba($primary-color, 0.1);
  }
  
  &--eliminar:hover {
    background-color: rgba($danger-color, 0.1);
  }
}

.inactive-row {
  opacity: 0.7;
}

.modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100;
  
  &__backdrop {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: rgba(0, 0, 0, 0.5);
  }
  
  &__content {
    position: relative;
    width: 100%;
    max-width: 600px;
    max-height: 90vh;
    overflow-y: auto;
    z-index: 101;
    
    &--sm {
      max-width: 400px;
    }
  }
}

.confirmacion {
  @include card;
  padding: $spacing-unit;
  
  &__title {
    color: $danger-color;
    margin-top: 0;
    margin-bottom: $spacing-unit;
  }
  
  &__message {
    margin-bottom: $spacing-unit * 1.5;
  }
  
  &__actions {
    display: flex;
    justify-content: flex-end;
    gap: $spacing-unit;
  }
}

.btn {
  @include button;
  
  &--primary {
    @include button($bg: $primary-color);
  }
  
  &--secondary {
    @include button($bg: $secondary-color);
  }
  
  &--danger {
    @include button($bg: $danger-color);
  }
}

// Responsive
@media (max-width: $breakpoint-md) {
  .especies {
    &__header {
      flex-direction: column;
      gap: $spacing-unit;
    }
    
    &__filters {
      flex-direction: column;
      align-items: stretch;
    }
  }
  
  .search-box__input {
    width: 100%;
  }
}
</style>
