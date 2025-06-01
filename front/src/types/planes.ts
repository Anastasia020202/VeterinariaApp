export interface PlanSalud {
  id: number;
  nombre: string;
  tratamientoId: number;
  nombreTratamiento?: string;
  descripcion: string;
  costo: number;
  duracionMeses: number;
  visitasIncluidas: number;
  incluyeEmergencias: boolean;
  porcentajeDescuento: number;
  detallesCobertura: string;
  fechaInicio: string;
  fechaFin: string;
  activo: boolean;
}

export interface PlanSaludDto {
  id?: number;
  nombre: string;
  tratamientoId: number;
  descripcion: string;
  costo: number;
  duracionMeses: number;
  visitasIncluidas: number;
  incluyeEmergencias: boolean;
  porcentajeDescuento: number;
  detallesCobertura: string;
  fechaInicio: string;
  fechaFin: string;
  activo?: boolean;
}
