<template>
  <div class="dashboard">
    <header class="dashboard__header">
      <h1 class="dashboard__title">Dashboard Clínica Veterinaria</h1>
      <p class="dashboard__subtitle">Sistema de gestión veterinaria</p>
    </header>

    <!-- Tarjetas de estadísticas -->
    <section class="dashboard__stats">
      <div class="stat-card stat-card--especies">
        <div class="stat-card__icon">🐾</div>
        <div class="stat-card__content">
          <h3 class="stat-card__title">Especies</h3>
          <p class="stat-card__number">{{ stats.totalEspecies || 0 }}</p>
        </div>
      </div>

      <div class="stat-card stat-card--tratamientos">
        <div class="stat-card__icon">💉</div>
        <div class="stat-card__content">
          <h3 class="stat-card__title">Tratamientos</h3>
          <p class="stat-card__number">{{ stats.totalTratamientos }}</p>
        </div>
      </div>

      <div class="stat-card stat-card--consultas">
        <div class="stat-card__icon">📋</div>
        <div class="stat-card__content">
          <h3 class="stat-card__title">Consultas</h3>
          <p class="stat-card__number">{{ stats.totalConsultas }}</p>
        </div>
      </div>

      <div class="stat-card stat-card--planes">
        <div class="stat-card__icon">🏥</div>
        <div class="stat-card__content">
          <h3 class="stat-card__title">Planes de Salud</h3>
          <p class="stat-card__number">{{ stats.totalPlanes }}</p>
        </div>
      </div>
    </section>

    <!-- Accesos rápidos a módulos -->
    <section class="dashboard__shortcuts">
      <h2 class="dashboard__section-title">Accesos Rápidos</h2>
      <div class="shortcut-grid">
        <router-link to="/especies" class="shortcut-card">
          <div class="shortcut-card__icon">🐾</div>
          <h3 class="shortcut-card__title">Especies</h3>
          <p class="shortcut-card__description">Gestionar especies de animales</p>
        </router-link>

        <router-link to="/tratamientos" class="shortcut-card">
          <div class="shortcut-card__icon">💉</div>
          <h3 class="shortcut-card__title">Tratamientos</h3>
          <p class="shortcut-card__description">Administrar tratamientos médicos</p>
        </router-link>

        <router-link to="/consultas" class="shortcut-card">
          <div class="shortcut-card__icon">📋</div>
          <h3 class="shortcut-card__title">Consultas</h3>
          <p class="shortcut-card__description">Registrar y ver consultas veterinarias</p>
        </router-link>

        <router-link to="/planes" class="shortcut-card">
          <div class="shortcut-card__icon">🏥</div>
          <h3 class="shortcut-card__title">Planes de Salud</h3>
          <p class="shortcut-card__description">Configurar planes de salud para mascotas</p>
        </router-link>
      </div>
    </section>

    <!-- Contenedores de datos recientes -->
    <div class="dashboard__recent-data">
      <!-- Especies recientes -->
      <section class="dashboard__recent-data-section">
        <EspeciesRecientes />
      </section>
      
      <!-- Consultas recientes -->
      <section class="dashboard__recent-data-section">
        <h2 class="dashboard__section-title">Consultas Recientes</h2>
        
        <div v-if="loading" class="dashboard__loading">
          Cargando consultas recientes...
        </div>
        
        <div v-else-if="error" class="dashboard__error">
          <p>{{ error }}</p>
          <button @click="cargarDatos" class="btn btn--primary">Reintentar</button>
        </div>
        
        <div v-else-if="consultasRecientes.length === 0" class="dashboard__empty">
          <p>No hay consultas registradas aún.</p>
          <router-link to="/consultas" class="btn btn--primary">Crear Consulta</router-link>
        </div>
        
        <table v-else class="dashboard__table">
          <thead>
            <tr>
              <th>Fecha</th>
              <th>Mascota</th>
              <th>Especie</th>
              <th>Tratamiento</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="consulta in consultasRecientes" :key="consulta.id">
              <td>{{ formatoFecha(consulta.fechaConsulta) }}</td>
              <td>{{ consulta.nombreMascota }}</td>
              <td>{{ consulta.nombreEspecieAnimal }}</td>
              <td>{{ consulta.nombreTratamiento }}</td>
            </tr>
          </tbody>
        </table>
        
        <div class="dashboard__more">
          <router-link to="/consultas" class="btn btn--secondary">
            Ver Todas las Consultas
          </router-link>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import axios from 'axios';
import { useAuthStore } from '@/stores/auth';
import EspeciesRecientes from '@/components/dashboard/EspeciesRecientes.vue';

// Estado del componente
const authStore = useAuthStore();
const loading = ref(false);
const error = ref('');
const consultasRecientes = ref([]);
const stats = ref({
  totalEspecies: 0,
  totalTratamientos: 0,
  totalConsultas: 0,
  totalPlanes: 0
});

// Ciclo de vida
onMounted(async () => {
  await cargarDatos();
});

// Métodos
async function cargarDatos() {
  loading.value = true;
  error.value = '';
  
  try {
    await Promise.all([
      cargarConsultasRecientes(),
      cargarEstadisticas()
    ]);
  } catch (err) {
    console.error('Error cargando datos del dashboard:', err);
    error.value = 'No se pudieron cargar los datos. Por favor, inténtalo de nuevo.';
  } finally {
    loading.value = false;
  }
}

async function cargarConsultasRecientes() {
  try {
    const { data } = await axios.get('/api/consultas?limite=5', authStore.authHeader);
    consultasRecientes.value = data;
  } catch (error) {
    console.error('Error cargando consultas recientes:', error);
    throw error;
  }
}

async function cargarEstadisticas() {
  try {
    const { data } = await axios.get('/api/dashboard/stats', authStore.authHeader);
    stats.value = data;
  } catch (error) {
    console.error('Error cargando estadísticas:', error);
    throw error;
  }
}

function formatoFecha(fecha: string): string {
  return new Date(fecha).toLocaleDateString('es-ES', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables';
@import '@/assets/styles/mixins';

.dashboard {
  &__header {
    margin-bottom: $spacing-unit * 2;
    text-align: center;
  }
  
  &__title {
    font-size: 2rem;
    color: $primary-color;
    margin-bottom: $spacing-unit / 2;
  }
  
  &__subtitle {
    font-size: 1.2rem;
    color: $secondary-color;
    opacity: 0.8;
  }
  
  &__stats {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: $spacing-unit;
    margin-bottom: $spacing-unit * 2;
  }
  
  &__section-title {
    font-size: 1.5rem;
    margin-bottom: $spacing-unit;
    color: $secondary-color;
    border-bottom: 2px solid $primary-color;
    padding-bottom: $spacing-unit / 2;
  }
  
  &__shortcuts {
    margin-bottom: $spacing-unit * 2;
  }
  
  &__recent-data {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(450px, 1fr));
    gap: $spacing-unit * 2;
    margin-bottom: $spacing-unit * 2;
  }
  
  &__recent-data-section {
    background-color: #fff;
    padding: 1rem;
    border: 1px solid #ddd;
    border-radius: 4px;
    margin-bottom: 1rem;
  }
  
  &__recent-data-section h2 {
    font-size: 1.5rem;
    margin-bottom: 0.5rem;
  }
  
  &__recent-data-section table {
    width: 100%;
    border-collapse: collapse;
  }
  
  &__recent-data-section table th,
  &__recent-data-section table td {
    padding: 0.5rem;
    text-align: left;
    border-bottom: 1px solid #eee;
  }
  
  &__loading, &__error, &__empty {
    padding: $spacing-unit * 2;
    text-align: center;
    margin-bottom: $spacing-unit;
  }
  
  &__error {
    color: $danger-color;
  }
  
  &__table {
    width: 100%;
    border-collapse: collapse;
    background-color: $light-color;
    border-radius: $border-radius;
    overflow: hidden;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
    
    th, td {
      padding: $spacing-unit;
      text-align: left;
    }
    
    th {
      background-color: $secondary-color;
      color: $light-color;
      font-weight: bold;
    }
    
    tr:nth-child(even) {
      background-color: rgba($border-color, 0.2);
    }
    
    tr:hover {
      background-color: rgba($primary-color, 0.1);
    }
  }
  
  &__more {
    margin-top: $spacing-unit;
    text-align: right;
  }
}

// Estilos para tarjetas de estadísticas
.stat-card {
  @include card;
  display: flex;
  align-items: center;
  padding: $spacing-unit;
  
  &__icon {
    font-size: 2rem;
    margin-right: $spacing-unit;
  }
  
  &__content {
    flex: 1;
  }
  
  &__title {
    font-size: 1rem;
    margin: 0 0 $spacing-unit / 4 0;
    color: $text-color;
  }
  
  &__number {
    font-size: 1.8rem;
    font-weight: bold;
    margin: 0;
    color: $primary-color;
  }
  
  // Variantes
  &--especies {
    border-left: 4px solid $primary-color;
  }
  
  &--tratamientos {
    border-left: 4px solid $info-color;
  }
  
  &--consultas {
    border-left: 4px solid $warning-color;
  }
  
  &--planes {
    border-left: 4px solid $success-color;
  }
}

// Estilos para accesos rápidos
.shortcut-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: $spacing-unit;
}

.shortcut-card {
  @include card;
  display: block;
  padding: $spacing-unit * 1.5;
  text-align: center;
  text-decoration: none;
  color: $text-color;
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  
  &:hover {
    transform: translateY(-5px);
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
    text-decoration: none;
  }
  
  &__icon {
    font-size: 2.5rem;
    margin-bottom: $spacing-unit;
  }
  
  &__title {
    font-size: 1.2rem;
    margin-bottom: $spacing-unit / 2;
    color: $primary-color;
  }
  
  &__description {
    font-size: 0.9rem;
    color: $text-color;
    opacity: 0.8;
  }
}

// Estilos para acciones en tabla
.table-action {
  color: $primary-color;
  text-decoration: none;
  font-weight: 500;
  
  &:hover {
    text-decoration: underline;
  }
}

// Estilos para botones
.btn {
  @include button;
  margin-left: $spacing-unit / 2;
  
  &--primary {
    @include button($bg: $primary-color);
  }
  
  &--secondary {
    @include button($bg: $secondary-color);
  }
}

// Responsive
@media (max-width: $breakpoint-md) {
  .dashboard {
    &__stats {
      grid-template-columns: repeat(2, 1fr);
    }
    
    &__table {
      display: block;
      overflow-x: auto;
    }
  }
}

@media (max-width: $breakpoint-sm) {
  .dashboard {
    &__stats {
      grid-template-columns: 1fr;
    }
  }
}

@media (max-width: 768px) {
  .dashboard__recent-data .dashboard__table {
    font-size: 0.8rem;
    overflow-x: auto;
    display: block;
  }
  .dashboard__recent-data {
    padding: 0 1rem;
  }
  .dashboard__recent-data-section {
    padding: 0.5rem;
  }
  .dashboard__recent-data-section h2 {
    font-size: 1.2rem;
  }
  .dashboard__recent-data-section table th,
  .dashboard__recent-data-section table td {
    padding: 0.3rem;
    font-size: 0.8rem;
  }
}
</style>
