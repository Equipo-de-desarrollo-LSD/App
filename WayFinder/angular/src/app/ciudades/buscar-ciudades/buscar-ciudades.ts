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
            //.buscarCiudad(request) 
            .buscarCiudadesByRequest(request)
            .pipe(finalize(() => (this.loading = false)));
        }),
      )
      // ... dentro del ngOnInit / .subscribe ...
.subscribe({
  next: (res: BuscarCiudadResultDto) => {
    
    // 🕵️‍♂️ LOG 1: ¿Qué llegó exactamente del servidor?
    console.log('1. Objeto de Respuesta del Servidor (RES):', res);

    // 🕵️‍♂️ LOG 2: ¿La propiedad 'ciudades' tiene datos?
    if (res.ciudades && res.ciudades.length > 0) {
       console.log('2. ✅ Éxito: Array con datos de ciudades recibido.');
    } else {
       // Esto puede ocurrir si res.ciudades es null o undefined
       console.warn('2. ⚠️ La propiedad "ciudades" está vacía o no existe en la respuesta.');
    }

    // El código que asigna y pagina:
    this.allCities = res.ciudades || []; 
    //this.applyFiltersAndPagination();
    this.ciudades = this.allCities 
    this.loading = false;
    // 🕵️‍♂️ LOG 3: ¿Se llenó la variable final que usa el HTML?
    console.log('3. Variable final (this.ciudades) para el HTML:', this.ciudades);
  },
  // ...
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

/*
  getCityImage(city: CiudadDto): string {
    const cityName = encodeURIComponent(city.nombre || 'city');
    return `https://source.unsplash.com/400x250/?${cityName},city`;
  }
    */
}
