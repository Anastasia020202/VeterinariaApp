export interface Tratamiento {
  id: number;
  nombre: string;
  descripcion: string;
  costoEstandar: number;
  requisitosVeterinarios?: string;
  cuidadosPosterior?: string;
  medicacionTipica?: string;
  duracionEstimadaMinutos: number;
  iconoNombre?: string;
  codigoColor?: string;
  activo: boolean;
  fechaCreacion: string;
}

export interface TratamientoDto {
  id?: number;
  nombre: string;
  descripcion: string;
  costoEstandar: number;
  requisitosVeterinarios?: string;
  cuidadosPosterior?: string;
  medicacionTipica?: string;
  duracionEstimadaMinutos: number;
  iconoNombre?: string;
  codigoColor?: string;
  activo?: boolean;
}
