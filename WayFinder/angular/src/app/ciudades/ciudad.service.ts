import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CiudadDto {
  nombre: string;
  pais: string;
  latitud: number;
  longitud: number;
}

@Injectable({ providedIn: 'root' })
export class CiudadService {
  constructor(private http: HttpClient) {}

  buscarPorNombre(nombre: string): Observable<CiudadDto[]> {
    return this.http.post<CiudadDto[]>(
      '/api/app/destino/buscar-por-nombre-externamente',
      {}, // body vacío porque es POST
      {
        params: { nombre },
      }
    );
  }
}