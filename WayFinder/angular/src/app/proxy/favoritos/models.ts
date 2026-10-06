import type { EntityDto } from '@abp/ng.core';

export interface CreateDestinoFavoritoDto {
  destinoTuristicoId?: string;
}

export interface DestinoFavoritoDto extends EntityDto<string> {
  destinoTuristicoId?: string;
  nombreDestino?: string;
  fotoDestino?: string;
  paisDestino?: string;
}
