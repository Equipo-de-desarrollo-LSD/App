import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Subject, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { CiudadDto, FiltrarCiudadesRequestDto } from 'src/app/proxy/destinos-turisticos-dtos/models';
import { DestinoTuristicoService } from 'src/app/proxy/destino-turisticos/destino-turistico.service';
import { CoreModule } from '@abp/ng.core';

@Component({
  selector: 'app-search-city',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, CoreModule],
  templateUrl: './buscar-ciudades.html',
  styleUrls: ['./buscar-ciudades.scss'],
})
export class BuscarCiudades implements OnInit, OnDestroy {
  private readonly ciudadService = inject(DestinoTuristicoService);

  ciudades: CiudadDto[] = [];
  allCities: CiudadDto[] = [];
  loading = false;
  errorMsg = '';

  searchText = '';
  codigoPais = '';
  poblacionMinima: number | null = null;
  
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;
  imagenesReales: { [nombreCiudad: string]: string } = {};

  private nombresPaises: { [key: string]: string } = {
    'AR': 'Argentina', 'BR': 'Brasil', 'CL': 'Chile', 'CO': 'Colombia',
    'ES': 'España', 'US': 'Estados Unidos', 'MX': 'México', 'UY': 'Uruguay'
  };

  // ⏱️ Variables para la búsqueda en tiempo real
  private searchSubject = new Subject<void>();
  private searchSubscription!: Subscription;

  ngOnInit(): void {
    // Escucha cada vez que tecleas, pero espera 500 milisegundos de pausa para buscar
    this.searchSubscription = this.searchSubject.pipe(
      debounceTime(500) 
    ).subscribe(() => {
      this.buscar();
    });
  }

  ngOnDestroy(): void {
    if (this.searchSubscription) this.searchSubscription.unsubscribe();
  }

  // Se ejecuta cada vez que escribes una letra en el HTML
  onBuscadorChange(): void {
    this.searchSubject.next();
  }

  buscar(): void {
    if (!this.searchText.trim() && !this.codigoPais.trim() && !this.poblacionMinima) {
      this.errorMsg = 'Ingresa al menos un filtro de búsqueda.';
      this.ciudades = [];
      this.allCities = [];
      return;
    }

    this.loading = true;
    this.errorMsg = '';
    this.currentPage = 1;

    if (this.searchText.trim()) {
      this.ciudadService.buscarCiudadesByRequest({ nombreCiudad: this.searchText.trim() }).subscribe({
        next: (res) => {
          let listaCiudades = res.ciudades || [];
          if (this.codigoPais.trim()) {
            const paisBuscado = this.nombresPaises[this.codigoPais];
            listaCiudades = listaCiudades.filter(ciudad => ciudad.pais === paisBuscado);
          }
          this.procesarRespuesta(listaCiudades);
        },
        error: (err) => this.manejarError(err)
      });
    } 
    else {
      const request: FiltrarCiudadesRequestDto = {
        paisCodigo: this.codigoPais.trim() ? this.codigoPais.trim() : undefined,
        minPoblacion: this.poblacionMinima ? this.poblacionMinima : undefined,
        limit: 10
      };

      this.ciudadService.filtrarCiudades(request).subscribe({
        next: (res) => this.procesarRespuesta(res.ciudades),
        error: (err) => this.manejarError(err)
      });
    }
  }

 private procesarRespuesta(ciudades: CiudadDto[] | undefined | null): void {
    this.allCities = ciudades || [];
    
    // 👇 NUEVO: Mandamos a buscar las fotos reales de todas las ciudades recibidas
    this.allCities.forEach(c => this.obtenerFotoReal(c.nombre));
    
    this.applyFiltersAndPagination();
    this.loading = false;
  }

  private manejarError(err: any): void {
    console.error('Error del servidor:', err);
    this.errorMsg = 'Hubo un error al buscar los destinos. Revisa los filtros o intenta de nuevo.';
    this.loading = false;
  }

  limpiar(): void {
    this.searchText = '';
    this.codigoPais = '';
    this.poblacionMinima = null;
    this.currentPage = 1;
    this.allCities = [];
    this.ciudades = [];
    this.errorMsg = '';
  }

  applyFiltersAndPagination(): void {
    let filtered = [...this.allCities];
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

  goToFirstPage(): void { this.goToPage(1); }
  goToLastPage(): void { this.goToPage(this.totalPages); }

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
      // 🗺️ URL corregida y oficial de Google Maps
      const url = `https://www.google.com/maps/search/?api=1&query=${city.latitud},${city.longitud}`;
      window.open(url, '_blank');
    }
  }

  // 📸 Función para generar imágenes atractivas
  getCityImage(city: CiudadDto): string {
    const seed = encodeURIComponent(city.nombre || 'city');
    return `https://picsum.photos/seed/${seed}/400/250`;
  }

  // Busca una foto real en Wikipedia de forma silenciosa
  obtenerFotoReal(nombreCiudad: string | null | undefined): void {
    if (!nombreCiudad || this.imagenesReales[nombreCiudad]) return;

    // Le preguntamos a Wikipedia si tiene un resumen y una foto de esta ciudad
    fetch(`https://es.wikipedia.org/api/rest_v1/page/summary/${encodeURIComponent(nombreCiudad)}`)
      .then(res => res.json())
      .then(data => {
        if (data.thumbnail && data.thumbnail.source) {
          // Si tiene foto, la guardamos en nuestro diccionario
          this.imagenesReales[nombreCiudad] = data.thumbnail.source;
        }
      })
      .catch(() => { /* Si Wikipedia falla, no hacemos nada, quedará la random */ });
  }
guardarDestino(city: CiudadDto): void {
    // 1. Tomamos la foto real de Wikipedia que ya buscamos, o usamos la generada
    const fotoUrl = this.imagenesReales[city.nombre || ''] || this.getCityImage(city);

    // 2. Armamos el paquete EXACTAMENTE con los nombres que exige el backend
    const destinoAGuardar = {
      nombre: city.nombre,
      paisNombre: city.pais,         // 👈 Corregido: antes decía 'pais'
      poblacion: city.paisPoblacion || 0, 
      latitud: city.latitud,
      longitud: city.longitud,
      foto: fotoUrl                  // 👈 Nuevo: agregamos la foto obligatoria
    };

    // 3. Lo enviamos a la base de datos
    this.ciudadService.create(destinoAGuardar as any).subscribe({
      next: () => {
        alert(`¡Éxito! ${city.nombre} se guardó en tu base de datos.`);
      },
      error: (err) => {
        console.error('Error al guardar:', err);
        alert(`No se pudo guardar ${city.nombre}. Revisa la consola para más detalles.`);
      }
    });
  }
}

/*
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
import { DestinoTuristicoService }from 'src/app/proxy/destino-turisticos/destino-turistico.service';
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


  // getCityImage(city: CiudadDto): string {
  //  const cityName = encodeURIComponent(city.nombre || 'city');
  //  return `https://source.unsplash.com/400x250/?${cityName},city`;
  // }
    
}*/
