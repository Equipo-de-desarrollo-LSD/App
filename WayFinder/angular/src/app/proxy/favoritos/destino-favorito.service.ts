import type { CreateDestinoFavoritoDto, DestinoFavoritoDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DestinoFavoritoService {
  apiName = 'Default';
  

  create = (input: CreateDestinoFavoritoDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/destino-favorito',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  deleteByDestinoId = (destinoId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/destino-favorito/by-destino-id/${destinoId}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<DestinoFavoritoDto>>({
      method: 'GET',
      url: '/api/app/destino-favorito',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  isFavorito = (destinoId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, boolean>({
      method: 'POST',
      url: `/api/app/destino-favorito/is-favorito/${destinoId}`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
