<template>
  <footer class="footer">
    <div class="footer__container">
      <div class="footer__info">
        <h3 class="footer__title">Clínica Veterinaria</h3>
        <p class="footer__text">Sistema de gestión para clínicas veterinarias</p>
        <p class="footer__copyright">© {{ currentYear }} Clínica Veterinaria. Todos los derechos reservados.</p>
      </div>
      
      <div class="footer__links">
        <h4 class="footer__subtitle">Enlaces rápidos</h4>
        <ul class="footer__menu">
          <li><router-link to="/" class="footer__link">Dashboard</router-link></li>
          <li><router-link to="/especies" class="footer__link">Especies</router-link></li>
          <li><router-link to="/tratamientos" class="footer__link">Tratamientos</router-link></li>
          <li><router-link to="/consultas" class="footer__link">Consultas</router-link></li>
        </ul>
      </div>
      
      <div class="footer__contact">
        <h4 class="footer__subtitle">Contacto</h4>
        <p class="footer__text">Dirección: Calle Ejemplo, 123</p>
        <p class="footer__text">Teléfono: 900 123 456</p>
        <p class="footer__text">Email: info@clinicaveterinaria.com</p>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
import { computed } from 'vue';

const currentYear = computed(() => new Date().getFullYear());
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables';
@import '@/assets/styles/mixins';

.footer {
  background-color: $secondary-color;
  color: $light-color;
  padding: $spacing-unit * 2 0;
  margin-top: auto; // Empuja el footer al final de la página
  
  &__container {
    @include container;
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: $spacing-unit * 2;
  }
  
  &__title {
    font-size: 1.5rem;
    margin-top: 0;
    margin-bottom: $spacing-unit;
  }
  
  &__subtitle {
    font-size: 1.2rem;
    margin-top: 0;
    margin-bottom: $spacing-unit;
  }
  
  &__text {
    margin-bottom: $spacing-unit / 2;
    opacity: 0.8;
  }
  
  &__copyright {
    margin-top: $spacing-unit;
    font-size: 0.9rem;
    opacity: 0.7;
  }
  
  &__menu {
    list-style: none;
    padding: 0;
    margin: 0;
    
    li {
      margin-bottom: $spacing-unit / 2;
    }
  }
  
  &__link {
    color: $light-color;
    text-decoration: none;
    opacity: 0.8;
    transition: opacity 0.3s ease;
    
    &:hover {
      opacity: 1;
      text-decoration: underline;
    }
  }
}

@media (max-width: $breakpoint-md) {
  .footer {
    &__container {
      grid-template-columns: 1fr;
      text-align: center;
    }
  }
}
</style>
