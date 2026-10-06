import type { TipoExperiencia } from './tipo-experiencia.enum';
import type { AuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateUpdateExperienciaViajeDto {
  destinoTuristicoId: string;
  titulo: string;
  contenido: string;
  sentimiento: TipoExperiencia;
}

export interface ExperienciaViajeDto extends AuditedEntityDto<string> {
  destinoTuristicoId?: string;
  titulo?: string;
  contenido?: string;
  sentimiento?: TipoExperiencia;
  creatorName?: string;
}

export interface GetExperienciasInput extends PagedAndSortedResultRequestDto {
  destinoTuristicoId?: string;
  sentimiento?: TipoExperiencia;
  filter?: string;
}
