import type { AuditedEntityDto } from '@abp/ng.core';

export interface BuscarCiudadRequestDto {
  nombreCiudad?: string;
}

export interface BuscarCiudadResultDto {
  ciudades: CiudadDto[];
}

export interface CalificacionDto {
  comentario?: string;
  puntaje: number;
  destinoId?: string;
  userId?: string;
}

export interface CiudadDto {
  nombre?: string;
  pais?: string;
  latitud: number;
  longitud: number;
  paisPoblacion: number;
}

export interface CoordenadasDto extends AuditedEntityDto<string> {
  latitud: number;
  longitud: number;
}

export interface CrearCalificacionDto {
  destinoId?: string;
  puntaje: number;
  comentario?: string;
  userId?: string;
}

export interface DestinoTuristicoDto extends AuditedEntityDto<string> {
  id?: string;
  nombre?: string;
  foto?: string;
  ultimaActualizacion?: string;
  pais: PaisDto;
  coordenadas: CoordenadasDto;
}

export interface FiltrarCiudadesRequestDto {
  paisCodigo?: string;
  minPoblacion?: number;
  limit: number;
}

export interface FiltrarCiudadesResultDto {
  ciudades: CiudadDto[];
}

export interface GuardarDestinos {
  nombre: string;
  foto: string;
  ultimaActualizacion?: string;
  paisNombre: string;
  paisPoblacion: number;
  coordenadasLatitud: number;
  coordenadasLongitud: number;
}

export interface PaisDto extends AuditedEntityDto<string> {
  nombre?: string;
  poblacion: number;
}
