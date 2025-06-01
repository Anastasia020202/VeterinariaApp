<template>
  <div class="register-page">
    <div class="register-container">
      <h1 class="register-title">Crear Cuenta</h1>
      
      <form @submit.prevent="handleRegister" class="register-form">
        <div class="form-group">
          <label for="email">Correo Electrónico</label>
          <input 
            type="email" 
            id="email" 
            v-model="email" 
            class="form-control" 
            required
            placeholder="ejemplo@correo.com"
            :class="{ 'form-control--error': errors.email }"
          />
          <p v-if="errors.email" class="form-error">{{ errors.email }}</p>
        </div>
        
        <div class="form-group">
          <label for="password">Contraseña</label>
          <input 
            type="password" 
            id="password" 
            v-model="password" 
            class="form-control" 
            required
            placeholder="Al menos 6 caracteres"
            :class="{ 'form-control--error': errors.password }"
          />
          <p v-if="errors.password" class="form-error">{{ errors.password }}</p>
        </div>
        
        <div class="form-group">
          <label for="confirmPassword">Confirmar Contraseña</label>
          <input 
            type="password" 
            id="confirmPassword" 
            v-model="confirmPassword" 
            class="form-control" 
            required
            :class="{ 'form-control--error': errors.confirmPassword }"
          />
          <p v-if="errors.confirmPassword" class="form-error">{{ errors.confirmPassword }}</p>
        </div>
        
        <div v-if="error" class="alert alert--error">
          {{ error }}
        </div>
        
        <div class="form-actions">
          <button 
            type="submit" 
            class="btn btn--primary btn--block" 
            :disabled="loading"
          >
            {{ loading ? 'Cargando...' : 'Registrarse' }}
          </button>
        </div>
        
        <div class="register-links">
          <p>¿Ya tienes cuenta? <router-link to="/login" class="register-link">Inicia Sesión</router-link></p>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

const router = useRouter();
const authStore = useAuthStore();

const email = ref('');
const password = ref('');
const confirmPassword = ref('');
const error = ref('');
const loading = ref(false);

const errors = computed(() => {
  const errors: Record<string, string> = {};
  
  if (password.value && password.value.length < 6) {
    errors.password = 'La contraseña debe tener al menos 6 caracteres';
  }
  
  if (confirmPassword.value && confirmPassword.value !== password.value) {
    errors.confirmPassword = 'Las contraseñas no coinciden';
  }
  
  return errors;
});

const isFormValid = computed(() => {
  return email.value.includes('@') && 
         password.value.length >= 6 && 
         password.value === confirmPassword.value;
});

const handleRegister = async () => {
  if (!isFormValid.value) {
    return;
  }
  
  error.value = '';
  loading.value = true;
  
  try {
    await authStore.register({
      email: email.value,
      password: password.value,
      confirmPassword: confirmPassword.value
    });
    
    // Si el registro fue exitoso, redirige al login
    router.push('/login');
  } catch (err: any) {
    console.error('Error en registro:', err);
    error.value = err.response?.data?.mensaje || 'Error al registrar usuario. Inténtalo de nuevo.';
  } finally {
    loading.value = false;
  }
};
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables';
@import '@/assets/styles/mixins';

.register-page {
  @include center-flex;
  min-height: calc(100vh - 130px); // Ajustar por altura de navbar y footer
  background-color: $background-color;
  padding: $spacing-unit;
}

.register-container {
  @include card($padding: $spacing-unit * 2, $bg: $light-color);
  width: 100%;
  max-width: 400px;
}

.register-title {
  font-size: 1.8rem;
  color: $primary-color;
  margin-bottom: $spacing-unit * 2;
  text-align: center;
}

.register-form {
  @include flex-column;
}

.form-group {
  margin-bottom: $spacing-unit;
  
  label {
    display: block;
    margin-bottom: $spacing-unit / 2;
    font-weight: 500;
    color: $text-color;
  }
}

.form-control {
  @include form-control;
  
  &--error {
    border-color: $danger-color;
  }
}

.form-error {
  color: $danger-color;
  font-size: $font-size-small;
  margin-top: 0.25rem;
  margin-bottom: 0;
}

.form-actions {
  margin-top: $spacing-unit;
}

.btn {
  @include button;
  
  &--primary {
    @include button($bg: $primary-color);
  }
  
  &--block {
    display: block;
    width: 100%;
  }
}

.alert {
  padding: $spacing-unit;
  margin: $spacing-unit 0;
  border-radius: $border-radius;
  
  &--error {
    background-color: rgba($danger-color, 0.1);
    color: $danger-color;
    border: 1px solid rgba($danger-color, 0.2);
  }
}

.register-links {
  margin-top: $spacing-unit;
  text-align: center;
  font-size: $font-size-small;
}

.register-link {
  color: $primary-color;
  font-weight: 500;
  text-decoration: none;
  
  &:hover {
    text-decoration: underline;
  }
}
</style>
