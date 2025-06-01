// Interfaces para Especies

export interface EspecieAnimal {
  id: number;
  nombre: string;
  descripcion: string;
  activo: boolean;
  cuidadosEspeciales?: string;
  fechaCreacion: string;
  fechaActualizacion: string;
}

export interface EspecieAnimalDTO {
  id?: number;
  nombre: string;
  descripcion: string;
  activo?: boolean;
  cuidadosEspeciales?: string;
}
