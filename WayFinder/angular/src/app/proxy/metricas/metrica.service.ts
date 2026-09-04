import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { MetricaDto } from '../metricas-dtos/models';

@Injectable({
  providedIn: 'root',
})
export class MetricaService {
  apiName = 'Default';
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, MetricaDto[]>({
      method: 'GET',
      url: '/api/app/metrica',
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
