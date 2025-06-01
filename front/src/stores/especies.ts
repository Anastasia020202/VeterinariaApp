import { defineStore } from 'pinia';
import apiService from '@/services/api-service';
import { EspecieAnimal, EspecieAnimalDTO } from '@/types/especies';

interface EspeciesState {
  especies: EspecieAnimal[];
  especieSeleccionada: EspecieAnimal | null;
  loading: boolean;
  error: string | null;
}

export const useEspeciesStore = defineStore('especies', {
  state: (): EspeciesState => ({
    especies: [],
    especieSeleccionada: null,
    loading: false,
    error: null
  }),

  getters: {
    especiesActivas: (state) => state.especies.filter(especie => especie.activo),
    especieById: (state) => (id: number) => state.especies.find(e => e.id === id)
  },

  actions: {
    async obtenerEspecies() {
      this.loading = true;
      this.error = null;
      
      try {
        // Usar el servicio API dedicado
        const especies = await apiService.getEspecies();
        this.especies = especies;
        return this.especies;
      } catch (error: any) {
        console.error('Error al obtener especies:', error);
        this.error = error.response?.data?.mensaje || 'Error al cargar las especies';
        throw error;
      } finally {
        this.loading = false;
      }
    },
    
    async obtenerEspecie(id: number) {
      this.loading = true;
      this.error = null;
      
      try {
        const especie = await apiService.getEspecie(id);
        this.especieSeleccionada = especie;
        return this.especieSeleccionada;
      } catch (error: any) {
        console.error(`Error al obtener especie ${id}:`, error);
        this.error = error.response?.data?.mensaje || 'Error al cargar la especie';
        throw error;
      } finally {
        this.loading = false;
      }
    },
    
    async crearEspecie(especie: EspecieAnimalDTO) {
      this.loading = true;
      this.error = null;
      
      try {
        const nuevaEspecie = await apiService.createEspecie(especie);
        // Añadir la nueva especie a la lista local
        this.especies.push(nuevaEspecie);
        return nuevaEspecie;
      } catch (error: any) {
        console.error('Error al crear especie:', error);
        this.error = error.response?.data?.mensaje || 'Error al crear la especie';
        throw error;
      } finally {
        this.loading = false;
      }
    },
    
    async actualizarEspecie(id: number, especie: EspecieAnimalDTO) {
      this.loading = true;
      this.error = null;
      
      try {
        const especieActualizada = await apiService.updateEspecie(id, especie);
        // Actualizar la especie en la lista local
        const index = this.especies.findIndex(e => e.id === id);
        if (index !== -1) {
          this.especies[index] = especieActualizada;
        }
        return especieActualizada;
      } catch (error: any) {
        console.error(`Error al actualizar especie ${id}:`, error);
        this.error = error.response?.data?.mensaje || 'Error al actualizar la especie';
        throw error;
      } finally {
        this.loading = false;
      }
    },
    
    async eliminarEspecie(id: number) {
      this.loading = true;
      this.error = null;
      
      try {
        await apiService.deleteEspecie(id);
        // Eliminar la especie de la lista local
        this.especies = this.especies.filter(e => e.id !== id);
        return true;
      } catch (error: any) {
        console.error(`Error al eliminar especie ${id}:`, error);
        this.error = error.response?.data?.mensaje || 'Error al eliminar la especie';
        throw error;
      } finally {
        this.loading = false;
      }
    }
  }
});
