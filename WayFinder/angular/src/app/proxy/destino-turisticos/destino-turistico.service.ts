import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { BuscarCiudadRequestDto, BuscarCiudadResultDto, DestinoTuristicoDto, GuardarDestinos } from '../destinos-turisticos-dtos/models';

@Injectable({
  providedIn: 'root',
})
export class DestinoTuristicoService {
  apiName = 'Default';
  

  buscarCiudad = (request: BuscarCiudadRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BuscarCiudadResultDto>({
      method: 'POST',
      url: '/api/app/destino-turistico/buscar-ciudad',
      body: request,
    },
    { apiName: this.apiName,...config });
  

  buscarCiudadesByRequest = (request: BuscarCiudadRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BuscarCiudadResultDto>({
      method: 'POST',
      url: '/api/app/destino-turistico/buscar-ciudades',
      body: request,
    },
    { apiName: this.apiName,...config });
  

  crearByInput = (input: GuardarDestinos, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoTuristicoDto>({
      method: 'POST',
      url: '/api/app/destino-turistico/crear',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  create = (input: GuardarDestinos, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoTuristicoDto>({
      method: 'POST',
      url: '/api/app/destino-turistico',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/destino-turistico/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoTuristicoDto>({
      method: 'GET',
      url: `/api/app/destino-turistico/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getAllDestinosTuristicos = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoTuristicoDto[]>({
      method: 'GET',
      url: '/api/app/destino-turistico/destinos-turisticos',
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<DestinoTuristicoDto>>({
      method: 'GET',
      url: '/api/app/destino-turistico',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: GuardarDestinos, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoTuristicoDto>({
      method: 'PUT',
      url: `/api/app/destino-turistico/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
