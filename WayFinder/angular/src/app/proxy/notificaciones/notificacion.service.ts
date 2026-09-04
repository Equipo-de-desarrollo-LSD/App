import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { NotificacionDto } from '../notificaciones-dtos/models';

@Injectable({
  providedIn: 'root',
})
export class NotificacionService {
  apiName = 'Default';
  

  getCountNoLeidas = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, number>({
      method: 'GET',
      url: '/api/app/notificacion/count-no-leidas',
    },
    { apiName: this.apiName,...config });
  

  getLista = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, NotificacionDto[]>({
      method: 'GET',
      url: '/api/app/notificacion/a',
    },
    { apiName: this.apiName,...config });
  

  marcarComoLeida = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/notificacion/${id}/marcar-como-leida`,
    },
    { apiName: this.apiName,...config });
  

  marcarComoNoLeida = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/notificacion/${id}/marcar-como-no-leida`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
