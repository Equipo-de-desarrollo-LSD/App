import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// 👉 Exportamos el tipo que usará el componente
export interface CiudadDto {
  id: string;
  nombre: string;   // usa el nombre real que devuelve tu API
  pais?: string;
}

// 👉 Exportamos una clase inyectable (el servicio)
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