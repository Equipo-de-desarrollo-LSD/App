import type { CreateUpdateExperienciaViajeDto, ExperienciaViajeDto, GetExperienciasInput } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ExperienciaViajeService {
  apiName = 'Default';
  

  create = (input: CreateUpdateExperienciaViajeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ExperienciaViajeDto>({
      method: 'POST',
      url: '/api/app/experiencia-viaje',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/experiencia-viaje/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ExperienciaViajeDto>({
      method: 'GET',
      url: `/api/app/experiencia-viaje/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetExperienciasInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ExperienciaViajeDto>>({
      method: 'GET',
      url: '/api/app/experiencia-viaje',
      params: { destinoTuristicoId: input.destinoTuristicoId, sentimiento: input.sentimiento, filter: input.filter, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateExperienciaViajeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ExperienciaViajeDto>({
      method: 'PUT',
      url: `/api/app/experiencia-viaje/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
