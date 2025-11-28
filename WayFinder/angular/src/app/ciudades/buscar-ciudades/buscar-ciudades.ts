import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, of } from 'rxjs';

import {
  debounceTime,
  distinctUntilChanged,
  switchMap,
  tap,
  finalize,
} from 'rxjs/operators';
import { CiudadDto } from '../../proxy/destinos-turisticos-dtos/models';
import { DestinoTuristicoService } from 'src/app/proxy/destinos-turisticos';
import { BuscarCiudadRequestDto, BuscarCiudadResultDto } from 'src/app/proxy/destinos-turisticos-dtos';

@Component({
  selector: 'app-search-city',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './buscar-ciudades.html',
  styleUrls: ['./buscar-ciudades.scss'],
})
export class BuscarCiudades implements OnInit {
  // 👉 inyectamos el servicio así, sin usar el constructor
  private readonly ciudadService = inject(DestinoTuristicoService);

  term$ = new Subject<string>();
  ciudades: CiudadDto[] = [];
  allCities: CiudadDto[] = [];
  loading = false;
  errorMsg = '';

  // Campos de búsqueda
  searchText = '';
  searchCountry = '';
  
  // Paginación
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;


  ngOnInit(): void {
    this.term$
      .pipe(
        debounceTime(400),
        distinctUntilChanged(),
       
        tap(() => {
          this.loading = true;
          this.errorMsg = '';
        }),
switchMap(term => {
          const text = term?.trim();
          if (!text) {
            // Se devuelve un objeto vacío del tipo esperado para limpiar la lista
            return of({ ciudades: [] } as BuscarCiudadResultDto).pipe(
              tap(() => (this.loading = false)),
            );
          }
        const request: BuscarCiudadRequestDto = { nombreCiudad: text };

          return this.ciudadService
            .buscarCiudad(request) // Se envía el objeto, no el string
            .pipe(finalize(() => (this.loading = false)));
        }),
      )
      .subscribe({
        next: (res: BuscarCiudadResultDto) => {
          // SOLUCIÓN ERROR 2: Acceder a la propiedad 'ciudades' del resultado
          this.allCities = res.ciudades || [] as any;
          this.applyFiltersAndPagination();
        },
        error: _ =>
          (this.errorMsg =
            'No se pudo buscar. ¿Backend levantado y proxy correcto?'),
      });
  }

  onInput(value: string) {
    this.term$.next(value);
  }
 buscar(): void {
    if (this.searchText.trim()) {
      this.currentPage = 1;
      this.term$.next(this.searchText);
    }
  }

  limpiar(): void {
    this.searchText = '';
    this.searchCountry = '';
    this.currentPage = 1;
    this.allCities = [];
    this.ciudades = [];
  }

  applyFiltersAndPagination(): void {
    let filtered = [...this.allCities];
  
    
    // Calcular paginación
    this.totalPages = Math.ceil(filtered.length / this.pageSize);
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.ciudades = filtered.slice(startIndex, endIndex);
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.applyFiltersAndPagination();
    }
  }

  goToFirstPage(): void {
    this.goToPage(1);
  }

  goToLastPage(): void {
    this.goToPage(this.totalPages);
  }

  getPaginationPages(): number[] {
    const pages: number[] = [];
    const maxPages = 5;
    let startPage = Math.max(1, this.currentPage - Math.floor(maxPages / 2));
    let endPage = Math.min(this.totalPages, startPage + maxPages - 1);

    if (endPage - startPage < maxPages - 1) {
      startPage = Math.max(1, endPage - maxPages + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    return pages;
  }

  verEnMapa(city: CiudadDto): void {
    if (city.latitud != null && city.longitud != null) {
      const url = `https://www.google.com/maps/search/?api=1&query=${city.latitud},${city.longitud}`;
      window.open(url, '_blank');
    }
  }


  getCityImage(city: CiudadDto): string {
    // Aquí puedes implementar la lógica para obtener imágenes desde un servicio
    // Por ahora, usamos Unsplash con el nombre de la ciudad
    const cityName = encodeURIComponent(city.nombre || 'city');
    return `https://source.unsplash.com/400x250/?${cityName},city`;
  }
}
