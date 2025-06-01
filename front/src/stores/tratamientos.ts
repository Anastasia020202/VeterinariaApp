import { defineStore } from 'pinia';
import apiService from '@/services/api-service';
import { Tratamiento, TratamientoDto } from '@/types/tratamientos';

interface TratamientosState {
  tratamientos: Tratamiento[];
  tratamientoSeleccionado: Tratamiento | null;
  loading: boolean;
  error: string | null;
}

export const useTratamientosStore = defineStore('tratamientos', {
  state: (): TratamientosState => ({
    tratamientos: [],
    tratamientoSeleccionado: null,
    loading: false,
    error: null
  }),
  getters: {
    activos: state => state.tratamientos.filter(t => t.activo),
    byId: state => (id: number) => state.tratamientos.find(t => t.id === id)
  },
  actions: {
    async obtenerTratamientos() {
      this.loading = true; this.error = null;
      try {
        this.tratamientos = await apiService.getTratamientos();
      } catch (err: any) {
        console.error('Error al cargar tratamientos:', err);
        this.error = err.response?.data?.mensaje || 'Error al cargar tratamientos';
      } finally { this.loading = false; }
    },
    async crearTratamiento(dto: TratamientoDto) {
      this.loading = true; this.error = null;
      try {
        const nuevo = await apiService.createTratamiento(dto);
        this.tratamientos.push(nuevo);
      } catch (err: any) {
        console.error('Error al crear tratamiento:', err);
        this.error = err.response?.data?.mensaje || 'Error al crear tratamiento';
        throw err;
      } finally { this.loading = false; }
    },
    async actualizarTratamiento(id: number, dto: TratamientoDto) {
      this.loading = true; this.error = null;
      try {
        const upd = await apiService.updateTratamiento(id, dto);
        const idx = this.tratamientos.findIndex(t=>t.id===id);
        if(idx>=0) this.tratamientos[idx] = upd;
      } catch(err: any) {
        console.error('Error al actualizar tratamiento:', err);
        this.error = err.response?.data?.mensaje || 'Error al actualizar tratamiento';
        throw err;
      } finally { this.loading = false; }
    },
    async eliminarTratamiento(id: number) {
      this.loading = true; this.error = null;
      try {
        await apiService.deleteTratamiento(id);
        this.tratamientos = this.tratamientos.filter(t=>t.id!==id);
      } catch(err: any) {
        console.error('Error al eliminar tratamiento:', err);
        this.error = err.response?.data?.mensaje || 'Error al eliminar tratamiento';
        throw err;
      } finally { this.loading = false; }
    }
  }
});
