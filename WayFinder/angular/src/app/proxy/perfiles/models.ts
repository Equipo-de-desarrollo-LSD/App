import type { EntityDto } from '@abp/ng.core';

export interface ActualizarPerfilDto {
  nombre: string;
  apellido: string;
  email: string;
  foto?: string;
  preferencias?: string;
}

export interface PerfilDto extends EntityDto<string> {
  userName?: string;
  email?: string;
  nombre?: string;
  apellido?: string;
  foto?: string;
  preferencias?: string;
}
