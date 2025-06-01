<template>
  <nav class="navbar">
    <div class="navbar__container">
      <div class="navbar__logo">
        <router-link to="/">
          <h1>Clínica Veterinaria</h1>
        </router-link>
      </div>
      
      <div v-if="authStore.isAuthenticated" class="navbar__menu">
        <router-link to="/" class="navbar__link">Dashboard</router-link>
        <router-link to="/especies" class="navbar__link">Especies</router-link>
        <router-link to="/tratamientos" class="navbar__link">Tratamientos</router-link>
        <router-link to="/consultas" class="navbar__link">Consultas</router-link>
        <router-link to="/planes" class="navbar__link">Planes de Salud</router-link>
      </div>
      
      <div class="navbar__actions">
        <template v-if="authStore.isAuthenticated">
          <button @click="authStore.logout" class="navbar__button navbar__button--logout">
            Cerrar Sesión
          </button>
        </template>
        <template v-else>
          <router-link to="/login" class="navbar__button">Iniciar Sesión</router-link>
          <router-link to="/registro" class="navbar__button navbar__button--primary">Registro</router-link>
        </template>
      </div>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth';

const authStore = useAuthStore();
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables';
@import '@/assets/styles/mixins';

.navbar {
  background-color: $primary-color;
  color: $light-color;
  padding: $spacing-unit 0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  
  &__container {
    @include container;
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
  
  &__logo {
    a {
      text-decoration: none;
      color: $light-color;
      
      h1 {
        font-size: 1.5rem;
        margin: 0;
      }
    }
  }
  
  &__menu {
    display: flex;
    gap: $spacing-unit;
  }
  
  &__link {
    color: $light-color;
    text-decoration: none;
    padding: 0.5rem 1rem;
    border-radius: $border-radius;
    transition: background-color 0.3s ease;
    
    &:hover, &.router-link-active {
      background-color: rgba(255, 255, 255, 0.1);
    }
  }
  
  &__actions {
    display: flex;
    gap: $spacing-unit;
  }
  
  &__button {
    padding: 0.5rem 1rem;
    border-radius: $border-radius;
    background-color: transparent;
    border: 1px solid $light-color;
    color: $light-color;
    cursor: pointer;
    text-decoration: none;
    transition: all 0.3s ease;
    
    &:hover {
      background-color: rgba(255, 255, 255, 0.1);
    }
    
    &--primary {
      background-color: $accent-color;
      border-color: $accent-color;
      
      &:hover {
        background-color: darken($accent-color, 10%);
      }
    }
    
    &--logout {
      background-color: $danger-color;
      border-color: $danger-color;
      
      &:hover {
        background-color: darken($danger-color, 10%);
      }
    }
  }
}

@media (max-width: $breakpoint-md) {
  .navbar {
    &__container {
      flex-direction: column;
      align-items: flex-start;
      padding: $spacing-unit;
    }
    
    &__menu {
      margin: $spacing-unit 0;
      flex-direction: column;
      width: 100%;
    }
    
    &__link {
      width: 100%;
    }
    
    &__actions {
      width: 100%;
      justify-content: flex-end;
    }
  }
}
</style>
