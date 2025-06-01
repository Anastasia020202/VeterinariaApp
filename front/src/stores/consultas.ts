import { defineStore } from 'pinia';
import apiService from '@/services/api-service';
import type { ConsultaVeterinaria, ConsultaVeterinariaDto } from '@/types/consultas';

interface ConsultasState {
  consultas: ConsultaVeterinaria[];
  loading: boolean;
  error: string | null;
}

export const useConsultasStore = defineStore('consultas', {
  state: (): ConsultasState => ({
    consultas: [],
    loading: false,
    error: null
  }),
  actions: {
    async obtenerConsultas(params: any = {}) {
      this.loading = true; this.error = null;
      try {
        this.consultas = await apiService.getConsultas(params);
      } catch (err: any) {
        console.error('Error al cargar consultas:', err);
        this.error = err.response?.data?.mensaje || 'Error al cargar consultas';
      } finally { this.loading = false; }
    },
    async crearConsulta(dto: ConsultaVeterinariaDto) {
      this.loading = true; this.error = null;
      try {
        const nueva = await apiService.createConsulta(dto);
        this.consultas.push(nueva);
      } catch (err: any) {
        console.error('Error al crear consulta:', err);
        this.error = err.response?.data?.mensaje || 'Error al crear consulta';
        throw err;
      } finally { this.loading = false; }
    },
    async actualizarConsulta(id: number, dto: ConsultaVeterinariaDto) {
      this.loading = true; this.error = null;
      try {
        const upd = await apiService.updateConsulta(id, dto);
        const idx = this.consultas.findIndex(c=>c.id===id);
        if(idx>=0) this.consultas[idx] = upd;
      } catch (err: any) {
        console.error('Error al actualizar consulta:', err);
        this.error = err.response?.data?.mensaje || 'Error al actualizar consulta';
        throw err;
      } finally { this.loading = false; }
    },
    async eliminarConsulta(id: number) {
      this.loading = true; this.error = null;
      try {
        await apiService.deleteConsulta(id);
        this.consultas = this.consultas.filter(c=>c.id!==id);
      } catch (err: any) {
        console.error('Error al eliminar consulta:', err);
        this.error = err.response?.data?.mensaje || 'Error al eliminar consulta';
        throw err;
      } finally { this.loading = false; }
    }
  }
});
