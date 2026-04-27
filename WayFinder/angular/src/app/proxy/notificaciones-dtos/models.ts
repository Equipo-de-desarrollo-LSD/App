import type { EntityDto } from '@abp/ng.core';

export interface NotificacionDto extends EntityDto<string> {
  usuarioId?: string;
  destinoId?: string;
  titulo?: string;
  mensaje?: string;
  fechaHora?: string;
  leido: boolean;
}
