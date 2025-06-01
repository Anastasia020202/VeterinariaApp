<template>
  <div class="especie-form">
    <div class="especie-form__header">
      <h2 class="especie-form__title">{{ esEdicion ? 'Editar Especie' : 'Nueva Especie' }}</h2>
      <button v-if="mostrarBotonCerrar" @click="cerrarFormulario" class="especie-form__close-button">
        <span>×</span>
      </button>
    </div>
    
    <form @submit.prevent="guardarEspecie" class="especie-form__form">
      <div class="form-group">
        <label for="nombre" class="form-label">Nombre:</label>
        <input 
          type="text" 
          id="nombre" 
          v-model="especie.nombre" 
          class="form-control" 
          :class="{ 'form-control--error': errores.nombre }"
          required
        />
        <p v-if="errores.nombre" class="form-error">{{ errores.nombre }}</p>
      </div>
      
      <div class="form-group">
        <label for="descripcion" class="form-label">Descripción:</label>
        <textarea 
          id="descripcion" 
          v-model="especie.descripcion" 
          class="form-control form-control--textarea" 
          :class="{ 'form-control--error': errores.descripcion }"
          required
        ></textarea>
        <p v-if="errores.descripcion" class="form-error">{{ errores.descripcion }}</p>
      </div>
      
      <div class="form-group">
        <label for="cuidadosEspeciales" class="form-label">Cuidados especiales:</label>
        <textarea 
          id="cuidadosEspeciales" 
          v-model="especie.cuidadosEspeciales" 
          class="form-control form-control--textarea"
          placeholder="(Opcional)"
        ></textarea>
      </div>
      
      <div v-if="esEdicion" class="form-group form-group--checkbox">
        <label class="checkbox-label">
          <input type="checkbox" v-model="especie.activo">
          Especie activa
        </label>
      </div>
      
      <div class="form-actions">
        <button 
          type="button" 
          @click="cerrarFormulario" 
          class="btn btn--secondary"
          :disabled="loading"
        >
          Cancelar
        </button>
        
        <button 
          type="submit" 
          class="btn btn--primary" 
          :disabled="!esFormularioValido || loading"
        >
          {{ loading ? 'Guardando...' : (esEdicion ? 'Actualizar' : 'Crear') }}
        </button>
      </div>
      
      <div v-if="error" class="alert alert--error">
        {{ error }}
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useEspeciesStore } from '@/stores/especies';
import type { EspecieAnimalDTO } from '@/types/especies';

const props = defineProps({
  especieId: {
    type: Number,
    default: null
  },
  mostrarBotonCerrar: {
    type: Boolean,
    default: true
  }
});

const emit = defineEmits(['cerrar', 'guardado']);

const especiesStore = useEspeciesStore();
const loading = ref(false);
const error = ref('');

// El modelo de especie para el formulario
const especie = ref<EspecieAnimalDTO>({
  nombre: '',
  descripcion: '',
  activo: true,
  cuidadosEspeciales: ''
});

// Control de errores de validación
const errores = ref({
  nombre: '',
  descripcion: ''
});

// Determinar si estamos en modo edición
const esEdicion = computed(() => !!props.especieId);

// Comprobar si el formulario es válido
const esFormularioValido = computed(() => {
  return !!especie.value.nombre && !!especie.value.descripcion && !errores.value.nombre && !errores.value.descripcion;
});

// Cargar los datos de la especie en edición
onMounted(async () => {
  if (props.especieId) {
    loading.value = true;
    try {
      const especieCargada = await especiesStore.obtenerEspecie(props.especieId);
      // Solo copiamos los campos para el formulario
      especie.value = {
        nombre: especieCargada.nombre,
        descripcion: especieCargada.descripcion,
        activo: especieCargada.activo,
        cuidadosEspeciales: especieCargada.cuidadosEspeciales || ''
      };
    } catch (err) {
      error.value = 'No se pudo cargar la especie para editar';
    } finally {
      loading.value = false;
    }
  }
});

// Validar campos
const validarFormulario = () => {
  errores.value.nombre = '';
  errores.value.descripcion = '';
  
  if (!especie.value.nombre.trim()) {
    errores.value.nombre = 'El nombre es obligatorio';
  } else if (especie.value.nombre.length < 3) {
    errores.value.nombre = 'El nombre debe tener al menos 3 caracteres';
  }
  
  if (!especie.value.descripcion.trim()) {
    errores.value.descripcion = 'La descripción es obligatoria';
  } else if (especie.value.descripcion.length < 10) {
    errores.value.descripcion = 'La descripción debe tener al menos 10 caracteres';
  }
  
  return !errores.value.nombre && !errores.value.descripcion;
};

// Guardar la especie
const guardarEspecie = async () => {
  if (!validarFormulario()) {
    return;
  }
  
  loading.value = true;
  error.value = '';
  
  try {
    if (esEdicion.value) {
      // Actualizar especie existente
      await especiesStore.actualizarEspecie(props.especieId!, especie.value);
    } else {
      // Crear nueva especie
      await especiesStore.crearEspecie(especie.value);
    }
    
    emit('guardado');
    cerrarFormulario();
  } catch (err: any) {
    console.error('Error al guardar especie:', err);
    error.value = err.response?.data?.mensaje || 'Error al guardar la especie. Inténtalo de nuevo.';
  } finally {
    loading.value = false;
  }
};

// Cerrar formulario
const cerrarFormulario = () => {
  emit('cerrar');
};
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables';
@import '@/assets/styles/mixins';

.especie-form {
  @include card;
  padding: $spacing-unit;
  
  &__header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: $spacing-unit;
  }
  
  &__title {
    font-size: 1.5rem;
    color: $primary-color;
    margin: 0;
  }
  
  &__close-button {
    background: none;
    border: none;
    color: $secondary-color;
    font-size: 1.5rem;
    cursor: pointer;
    padding: 0;
    width: 30px;
    height: 30px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    
    &:hover {
      background-color: rgba($secondary-color, 0.1);
    }
  }
  
  &__form {
    @include flex-column;
  }
}

.form-group {
  margin-bottom: $spacing-unit;
  
  &--checkbox {
    display: flex;
    align-items: center;
  }
}

.form-label {
  display: block;
  margin-bottom: calc($spacing-unit / 2);
  font-weight: 500;
}

.checkbox-label {
  display: flex;
  align-items: center;
  font-weight: 500;
  gap: calc($spacing-unit / 2);
  cursor: pointer;
}

.form-control {
  @include form-control;
  
  &--textarea {
    min-height: 100px;
    resize: vertical;
  }
  
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
  @include flex-between;
  margin-top: $spacing-unit;
}

.btn {
  @include button;
  
  &--primary {
    @include button($bg: $primary-color);
  }
  
  &--secondary {
    @include button($bg: $secondary-color);
  }
  
  &:disabled {
    opacity: 0.6;
    cursor: not-allowed;
  }
}

.alert {
  padding: $spacing-unit;
  margin-top: $spacing-unit;
  border-radius: $border-radius;
  
  &--error {
    background-color: rgba($danger-color, 0.1);
    color: $danger-color;
    border: 1px solid rgba($danger-color, 0.2);
  }
}
</style>
