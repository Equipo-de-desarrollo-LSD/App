import { mapEnumToOptions } from '@abp/ng.core';

export enum TipoExperiencia {
  Positiva = 0,
  Neutral = 1,
  Negativa = 2,
}

export const tipoExperienciaOptions = mapEnumToOptions(TipoExperiencia);
