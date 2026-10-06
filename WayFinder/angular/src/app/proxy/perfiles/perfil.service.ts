import type { ActualizarPerfilDto, PerfilDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { PerfilPublicoDto } from '../destinos-turisticos-dtos/perfiles/models';

@Injectable({
  providedIn: 'root',
})
export class PerfilService {
  apiName = 'Default';
  

  eliminarMiCuenta = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/perfil/eliminar-mi-cuenta',
    },
    { apiName: this.apiName,...config });
  

  getMiPerfil = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, PerfilDto>({
      method: 'GET',
      url: '/api/app/perfil/mi-perfil',
    },
    { apiName: this.apiName,...config });
  

  getPerfilPublico = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PerfilPublicoDto>({
      method: 'GET',
      url: `/api/app/perfil/${id}/perfil-publico`,
    },
    { apiName: this.apiName,...config });
  

  updateMiPerfil = (input: ActualizarPerfilDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: '/api/app/perfil/mi-perfil',
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
