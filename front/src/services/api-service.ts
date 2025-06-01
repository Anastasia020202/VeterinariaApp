import axios from 'axios';
import type { TratamientoDto } from '@/types/tratamientos';
import type { PlanSaludDto } from '@/types/planes';

// URL base de la API (se utiliza la variable de entorno)
const API_URL = import.meta.env.VITE_API_BASE || 'http://localhost:5000/api';

/**
 * Servicio para gestionar las comunicaciones con la API
 * Incluye métodos para especies, tratamientos, consultas y planes de salud
 */
class ApiService {
  // Asegurarse de que siempre se envía el token en cada petición
  private getHeaders() {
    const token = localStorage.getItem('token');
    return {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': token ? `Bearer ${token}` : ''
      }
    };
  }

  // ======================
  // ESPECIES
  // ======================
  
  /**
   * Obtener todas las especies
   * @param busqueda Filtro opcional por nombre o descripción
   */
  async getEspecies(busqueda?: string) {
    const params = busqueda ? `?busqueda=${encodeURIComponent(busqueda)}` : '';
    try {
      const response = await axios.get(`${API_URL}/especies${params}`, this.getHeaders());
      return response.data;
    } catch (error) {
      console.error('Error al obtener especies:', error);
      throw error;
    }
  }

  /**
   * Obtener una especie por ID
   * @param id ID de la especie a obtener
   */
  async getEspecie(id: number) {
    try {
      const response = await axios.get(`${API_URL}/especies/${id}`, this.getHeaders());
      return response.data;
    } catch (error) {
      console.error(`Error al obtener especie ${id}:`, error);
      throw error;
    }
  }

  /**
   * Crear una nueva especie
   * @param especie Datos de la especie a crear
   */
  async createEspecie(especie: any) {
    try {
      console.log('Enviando solicitud para crear especie con token:', localStorage.getItem('token')?.substring(0, 20) + '...');
      const response = await axios.post(`${API_URL}/especies`, especie, this.getHeaders());
      return response.data;
    } catch (error) {
      console.error('Error al crear especie:', error);
      throw error;
    }
  }

  /**
   * Actualizar una especie existente
   * @param id ID de la especie a actualizar
   * @param especie Datos actualizados de la especie
   */
  async updateEspecie(id: number, especie: any) {
    try {
      const response = await axios.put(`${API_URL}/especies/${id}`, especie, this.getHeaders());
      return response.data;
    } catch (error) {
      console.error(`Error al actualizar especie ${id}:`, error);
      throw error;
    }
  }

  /**
   * Eliminar una especie
   * @param id ID de la especie a eliminar
   */
  async deleteEspecie(id: number) {
    try {
      await axios.delete(`${API_URL}/especies/${id}`, this.getHeaders());
      return true;
    } catch (error) {
      console.error(`Error al eliminar especie ${id}:`, error);
      throw error;
    }
  }

  // ======================
  // TRATAMIENTOS 
  // ======================

  async getTratamientos(busqueda?: string, activo?: boolean) {
    let params = new URLSearchParams();
    if (busqueda) params.append('busqueda', busqueda);
    if (activo !== undefined) params.append('activo', activo.toString());
    
    const queryString = params.toString() ? `?${params.toString()}` : '';
    
    try {
      const response = await axios.get(`${API_URL}/tratamientos${queryString}`, this.getHeaders());
      return response.data;
    } catch (error) {
      console.error('Error al obtener tratamientos:', error);
      throw error;
    }
  }

  /**
   * Crear un nuevo tratamiento
   */
  async createTratamiento(dto: TratamientoDto) {
    try {
      const response = await axios.post(`${API_URL}/tratamientos`, dto, this.getHeaders());
      return response.data;
    } catch (error) {
      console.error('Error al crear tratamiento:', error);
      throw error;
    }
  }

  /**
   * Actualizar un tratamiento existente
   */
  async updateTratamiento(id: number, dto: TratamientoDto) {
    try {
      const response = await axios.put(`${API_URL}/tratamientos/${id}`, dto, this.getHeaders());
      return response.data;
    } catch (error) {
      console.error(`Error al actualizar tratamiento ${id}:`, error);
      throw error;
    }
  }

  /**
   * Eliminar un tratamiento
   */
  async deleteTratamiento(id: number) {
    try {
      await axios.delete(`${API_URL}/tratamientos/${id}`, this.getHeaders());
      return true;
    } catch (error) {
      console.error(`Error al eliminar tratamiento ${id}:`, error);
      throw error;
    }
  }

  // ======================
  // CONSULTAS
  // ======================

  async getConsultas(params: any = {}) {
    const queryParams = new URLSearchParams();
    Object.keys(params).forEach(key => {
      if (params[key] !== undefined && params[key] !== null) {
        queryParams.append(key, params[key].toString());
      }
    });
    
    const queryString = queryParams.toString() ? `?${queryParams.toString()}` : '';
    
    try {
      const response = await axios.get(`${API_URL}/consultas${queryString}`, this.getHeaders());
      return response.data;
    } catch (error) {
      console.error('Error al obtener consultas:', error);
      throw error;
    }
  }

  /**
   * Crear nueva consulta
   */
  async createConsulta(dto: any) {
    // Convert date fields to 'yyyy-MM-dd' format
    dto.fechaConsulta = new Date(dto.fechaConsulta).toISOString().substring(0,10);
    if(dto.fechaSeguimiento) {
      dto.fechaSeguimiento = new Date(dto.fechaSeguimiento).toISOString().substring(0,10);
    }
    try {
      const response = await axios.post(`${API_URL}/consultas`, dto, this.getHeaders());
      return response.data;
    } catch (error) {
      console.error('Error al crear consulta:', error);
      throw error;
    }
  }

  /**
   * Actualizar consulta existente
   */
  async updateConsulta(id: number, dto: any) {
    try {
      const response = await axios.put(`${API_URL}/consultas/${id}`, dto, this.getHeaders());
      return response.data;
    } catch (error) {
      console.error(`Error al actualizar consulta ${id}:`, error);
      throw error;
    }
  }

  /**
   * Eliminar consulta por ID
   */
  async deleteConsulta(id: number) {
    try {
      await axios.delete(`${API_URL}/consultas/${id}`, this.getHeaders());
      return true;
    } catch (error) {
      console.error(`Error al eliminar consulta ${id}:`, error);
      throw error;
    }
  }

  // ======================
  // PLANES DE SALUD
  // ======================

  async getPlanesSalud(busqueda?: string, activo?: boolean) {
    let params = new URLSearchParams();
    if (busqueda) params.append('busqueda', busqueda);
    if (activo !== undefined) params.append('activo', activo.toString());
    
    const queryString = params.toString() ? `?${params.toString()}` : '';
    
    try {
      const response = await axios.get(`${API_URL}/planes${queryString}`, this.getHeaders());
      return response.data;
    } catch (error) {
      console.error('Error al obtener planes de salud:', error);
      throw error;
    }
  }

  /**
   * Crear nuevo plan de salud
   */
  async createPlanSalud(dto: PlanSaludDto) {
    try {
      const response = await axios.post(`${API_URL}/planes`, dto, this.getHeaders());
      return response.data;
    } catch (error) {
      console.error('Error al crear plan de salud:', error);
      throw error;
    }
  }

  /**
   * Actualizar plan de salud existente
   */
  async updatePlanSalud(id: number, dto: PlanSaludDto) {
    try {
      const response = await axios.put(`${API_URL}/planes/${id}`, dto, this.getHeaders());
      return response.data;
    } catch (error) {
      console.error(`Error al actualizar plan de salud ${id}:`, error);
      throw error;
    }
  }

  /**
   * Eliminar plan de salud
   */
  async deletePlanSalud(id: number) {
    try {
      await axios.delete(`${API_URL}/planes/${id}`, this.getHeaders());
      return true;
    } catch (error) {
      console.error(`Error al eliminar plan de salud ${id}:`, error);
      throw error;
    }
  }

  // ======================
  // ESTADÍSTICAS (para dashboard)
  // ======================

  async getEstadisticas() {
    try {
      const response = await axios.get(`${API_URL}/estadisticas`, this.getHeaders());
      return response.data;
    } catch (error) {
      console.error('Error al obtener estadísticas:', error);
      throw error;
    }
  }
}

// Exportar instancia
const apiService = new ApiService();
export default apiService;
