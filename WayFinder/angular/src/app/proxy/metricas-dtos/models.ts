import type { EntityDto } from '@abp/ng.core';

export interface MetricaDto extends EntityDto<string> {
  nombreServicio?: string;
  endpoint?: string;
  fechaEjecucion?: string;
  tiempoRespuestaMs: number;
}
