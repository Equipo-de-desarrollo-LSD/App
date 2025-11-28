import type { AuditedEntityDto } from '@abp/ng.core';


export interface BuscarCiudadRequestDto {
  nombreCiudad?: string;
}

export interface BuscarCiudadResultDto {
  ciudades: CiudadDto[];
}

export interface CiudadDto {
  nombre?: string;
  pais?: string;
  latitud: number;
  longitud: number;
}

export interface CoordenadasDto extends AuditedEntityDto<string> {
  latitud: number;
  longitud: number;
}

export interface DestinoTuristicoDto extends AuditedEntityDto<string> {
  id?: string;
  nombre?: string;
  foto?: string;
  ultimaActualizacion?: string;
  pais: PaisDto;
  coordenadas: CoordenadasDto;
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
