export interface ConsultaVeterinaria {
  id: number;
  especieAnimalId: number;
  tratamientoId: number;
  nombreMascota: string;
  nombrePropietario: string;
  informacionContacto: string;
  costo: number;
  descripcion: string;
  notasTratamiento?: string;
  prescripcion?: string;
  fechaConsulta: string;
  // fechaSeguimiento eliminada
  nombreEspecieAnimal?: string;
  nombreTratamiento?: string;
}

export interface ConsultaVeterinariaDto {
  id?: number;
  especieAnimalId: number;
  tratamientoId: number;
  nombreMascota: string;
  nombrePropietario: string;
  informacionContacto: string;
  costo: number;
  descripcion: string;
  notasTratamiento?: string;
  prescripcion?: string;
  fechaConsulta: string;
  // fechaSeguimiento eliminada
}
