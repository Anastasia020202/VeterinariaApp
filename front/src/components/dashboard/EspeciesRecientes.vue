<template>
  <div class="especies-recientes">
    <h3 class="especies-recientes__title">Especies Recientes</h3>
    
    <div v-if="loading" class="especies-recientes__loading">
      <p>Cargando especies...</p>
    </div>
    
    <div v-else-if="error" class="especies-recientes__error">
      <p>{{ error }}</p>
      <button @click="cargarEspecies" class="especies-recientes__retry">
        Reintentar
      </button>
    </div>
    
    <div v-else-if="especies.length === 0" class="especies-recientes__empty">
      <p>No hay especies registradas aún.</p>
      <router-link to="/especies" class="especies-recientes__link">
        Agregar especies
      </router-link>
    </div>
    
    <ul v-else class="especies-recientes__list">
      <li v-for="especie in especies.slice(0, 5)" :key="especie.id" class="especies-recientes__item">
        <div class="especies-recientes__item-header">
          <span class="especies-recientes__item-name">{{ especie.nombre }}</span>
          <span 
            class="especies-recientes__item-badge"
            :class="especie.activo ? 'especies-recientes__item-badge--active' : 'especies-recientes__item-badge--inactive'"
          >
            {{ especie.activo ? 'Activo' : 'Inactivo' }}
          </span>
        </div>
        <p class="especies-recientes__item-description">{{ especie.descripcion }}</p>
        <router-link :to="`/especies`" class="especies-recientes__item-link">
          Ver detalles
        </router-link>
      </li>
    </ul>
    
    <div class="especies-recientes__footer">
      <router-link to="/especies" class="especies-recientes__view-all">
        Ver todas las especies
      </router-link>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useEspeciesStore } from '@/stores/especies';
import type { EspecieAnimal } from '@/types/especies';

const especiesStore = useEspeciesStore();
const especies = ref<EspecieAnimal[]>([]);
const loading = ref(true);
const error = ref('');

onMounted(() => {
  cargarEspecies();
});

async function cargarEspecies() {
  loading.value = true;
  error.value = '';
  
  try {
    await especiesStore.obtenerEspecies();
    especies.value = [...especiesStore.especies].sort((a, b) => {
      // Ordenar por fecha de creación, las más recientes primero
      return new Date(b.fechaCreacion).getTime() - new Date(a.fechaCreacion).getTime();
    });
  } catch (err: any) {
    console.error('Error al cargar especies recientes:', err);
    error.value = 'No se pudieron cargar las especies.';
  } finally {
    loading.value = false;
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables';
@import '@/assets/styles/mixins';

.especies-recientes {
  @include card;
  padding: $spacing-unit;
  
  &__title {
    font-size: 1.2rem;
    color: $secondary-color;
    margin-top: 0;
    margin-bottom: $spacing-unit;
    padding-bottom: calc($spacing-unit / 2);
    border-bottom: 2px solid $primary-color;
  }
  
  &__loading, &__error, &__empty {
    padding: $spacing-unit;
    text-align: center;
    
    p {
      margin-bottom: calc($spacing-unit / 2);
      color: $secondary-color;
    }
  }
  
  &__retry, &__link {
    @include button($bg: $primary-color);
    display: inline-block;
  }
  
  &__list {
    list-style: none;
    padding: 0;
    margin: 0;
  }
  
  &__item {
    padding: $spacing-unit;
    border-bottom: 1px solid $border-color;
    
    &:last-child {
      border-bottom: none;
    }
    
    &:hover {
      background-color: rgba($primary-color, 0.05);
    }
  }
  
  &__item-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: calc($spacing-unit / 2);
  }
  
  &__item-name {
    font-weight: bold;
    color: $primary-color;
  }
  
  &__item-badge {
    font-size: 0.8rem;
    padding: calc($spacing-unit / 4) calc($spacing-unit / 2);
    border-radius: $border-radius;
    
    &--active {
      background-color: rgba($success-color, 0.15);
      color: $success-color;
    }
    
    &--inactive {
      background-color: rgba($danger-color, 0.15);
      color: $danger-color;
    }
  }
  
  &__item-description {
    margin: 0 0 calc($spacing-unit / 2) 0;
    color: $text-color;
    font-size: $font-size-small;
    
    // Limitar a 2 líneas
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
  }
  
  &__item-link {
    color: $primary-color;
    font-size: $font-size-small;
    text-decoration: none;
    
    &:hover {
      text-decoration: underline;
    }
  }
  
  &__footer {
    margin-top: $spacing-unit;
    text-align: right;
  }
  
  &__view-all {
    color: $primary-color;
    text-decoration: none;
    font-weight: 500;
    
    &:hover {
      text-decoration: underline;
    }
  }
}
</style>
