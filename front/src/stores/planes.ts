import { defineStore } from 'pinia';
import apiService from '@/services/api-service';
import type { PlanSalud, PlanSaludDto } from '@/types/planes';

interface PlanesState {
  planes: PlanSalud[];
  loading: boolean;
  error: string | null;
}

export const usePlanesStore = defineStore('planes', {
  state: (): PlanesState => ({
    planes: [],
    loading: false,
    error: null,
  }),
  actions: {
    async obtenerPlanes(busqueda?: string, activo?: boolean) {
      this.loading = true; this.error = null;
      try {
        this.planes = await apiService.getPlanesSalud(busqueda, activo);
      } catch (err: any) {
        console.error('Error al cargar planes de salud:', err);
        this.error = err.response?.data?.mensaje || 'Error al cargar planes de salud';
      } finally { this.loading = false; }
    },
    async crearPlan(dto: PlanSaludDto) {
      this.loading = true; this.error = null;
      try {
        const nuevo = await apiService.createPlanSalud(dto);
        this.planes.push(nuevo);
      } catch (err: any) {
        console.error('Error al crear plan de salud:', err);
        this.error = err.response?.data?.mensaje || 'Error al crear plan de salud';
        throw err;
      } finally { this.loading = false; }
    },
    async actualizarPlan(id: number, dto: PlanSaludDto) {
      this.loading = true; this.error = null;
      try {
        const upd = await apiService.updatePlanSalud(id, dto);
        const idx = this.planes.findIndex(p => p.id === id);
        if (idx >= 0) this.planes[idx] = upd;
      } catch (err: any) {
        console.error('Error al actualizar plan de salud:', err);
        this.error = err.response?.data?.mensaje || 'Error al actualizar plan de salud';
        throw err;
      } finally { this.loading = false; }
    },
    async eliminarPlan(id: number) {
      this.loading = true; this.error = null;
      try {
        await apiService.deletePlanSalud(id);
        this.planes = this.planes.filter(p => p.id !== id);
      } catch (err: any) {
        console.error('Error al eliminar plan de salud:', err);
        this.error = err.response?.data?.mensaje || 'Error al eliminar plan de salud';
        throw err;
      } finally { this.loading = false; }
    }
  }
});
