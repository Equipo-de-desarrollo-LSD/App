export interface CiudadDto {
  name?: string;
  country?: string;
  latitude: number;
  longitude: number;
}

export interface GuardarDestinos {
  nombre: string;
  pais: string;
  descripcion?: string;
}

export interface DestinoTuristicoDto {
  id?: string;
  nombre?: string;
  pais?: string;
  descripcion?: string;
}