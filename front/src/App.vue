<script setup lang="ts">
import { RouterView } from 'vue-router'
import Navbar from './components/Navbar.vue'
import Footer from './components/Footer.vue'
import { useAuthStore } from './stores/auth'
import { onMounted, onBeforeMount, provide } from 'vue'
import axios from 'axios'

console.log('App: Inicialización del componente');
const authStore = useAuthStore();

// Proporcionar el store de autenticación a todos los componentes
provide('authStore', authStore);

// Configurar autenticación antes del montaje
onBeforeMount(() => {
  console.log('App: Configurando autenticación antes del montaje');
  
  // Configurar interceptores globales una sola vez
  if (!window.interceptorsInitialized) {
    console.log('App: Inicializando interceptores globales');
    
    // Interceptor de solicitud global
    axios.interceptors.request.use(
      config => {
        console.log(`App: Petición a ${config.url}`);
        return config;
      },
      error => {
        console.error('App: Error en petición:', error);
        return Promise.reject(error);
      }
    );
    
    window.interceptorsInitialized = true;
  }
});

// Inicializar autenticación al montar
onMounted(() => {
  console.log('App: Componente montado, inicializando autenticación');
  authStore.initialize();
  
  // Registrar estado de autenticación
  console.log(`App: Estado de autenticación: ${authStore.isAuthenticated ? 'Autenticado' : 'No autenticado'}`);
})
</script>

<template>
  <div class="app-container">
    <Navbar />
    <main class="app-content">
      <RouterView />
    </main>
    <Footer />
  </div>
</template>

<style lang="scss">
@import './assets/styles/variables';

html, body, #app {
  height: 100%;
  margin: 0;
  font-family: $font-family-base;
  font-size: $font-size-base;
  color: $text-color;
  background-color: $background-color;
}

.app-container {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

.app-content {
  flex: 1;
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
  padding: $spacing-unit;
  box-sizing: border-box;
}

// Estilos generales para elementos comunes
h1, h2, h3, h4, h5, h6 {
  color: $secondary-color;
  margin-top: 0;
}

a {
  color: $primary-color;
  text-decoration: none;
  
  &:hover {
    text-decoration: underline;
  }
}

// Resetear estilos de lista
ul {
  padding: 0;
  margin: 0;
  list-style: none;
}
</style>

<style>
/* Declare la variable global typescipt para detectores de interceptores */
declare global {
  interface Window {
    interceptorsInitialized?: boolean;
  }
}
</style>
