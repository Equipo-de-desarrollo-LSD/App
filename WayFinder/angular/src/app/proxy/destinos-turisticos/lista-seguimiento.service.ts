import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ListaSeguimientoService {
  apiName = 'Default';
  

  dejarDeSeguirDestino = (destinoId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/lista-seguimiento/dejar-de-seguir-destino/${destinoId}`,
    },
    { apiName: this.apiName,...config });
  

  seguirDestino = (destinoId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/lista-seguimiento/seguir-destino/${destinoId}`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
