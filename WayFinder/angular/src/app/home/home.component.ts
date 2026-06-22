import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Subject, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { CiudadDto, FiltrarCiudadesRequestDto } from 'src/app/proxy/destinos-turisticos-dtos/models';
import { DestinoTuristicoService } from 'src/app/proxy/destino-turisticos/destino-turistico.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule], 
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  // TODO: Reemplazar este Mock con la llamada al backend cuando esté listo el merge de Favoritos
 destinosPopulares: any[] = [
    {
      id: '11111111-1111-1111-1111-111111111111',
      nombre: 'Buenos Aires',
      pais: 'Argentina',
      imagenUrl: 'https://images.unsplash.com/photo-1612294037345-b4eb04c96e1b?q=80&w=800&auto=format&fit=crop',
      favoritos: 1540,
      calificacion: 4.8
    },
    {
      id: '22222222-2222-2222-2222-222222222222',
      nombre: 'Bariloche',
      pais: 'Argentina',
      imagenUrl: 'https://images.unsplash.com/photo-1522228115018-d838bcce5c3a?q=80&w=800&auto=format&fit=crop',
      favoritos: 1205,
      calificacion: 4.9
    },
    {
      id: '33333333-3333-3333-3333-333333333333',
      nombre: 'Mendoza',
      pais: 'Argentina',
      imagenUrl: 'https://images.unsplash.com/photo-1584464491033-06628f3a6b7b?q=80&w=800&auto=format&fit=crop',
      favoritos: 980,
      calificacion: 4.7
    }
  ];
  private router = inject(Router);
  private ciudadService = inject(DestinoTuristicoService);
  

  // --- VARIABLES DEL BUSCADOR ---
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

  private searchSubject = new Subject<void>();
  private searchSubscription!: Subscription;


  ngOnInit(): void {
    this.searchSubscription = this.searchSubject.pipe(
      debounceTime(500) 
    ).subscribe(() => {
      this.buscar();
    });
  }

  ngOnDestroy(): void {
    if (this.searchSubscription) this.searchSubscription.unsubscribe();
  }

  // --- LÓGICA DEL BUSCADOR ---
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
    } else {
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
    if (endPage - startPage < maxPages - 1) startPage = Math.max(1, endPage - maxPages + 1);
    for (let i = startPage; i <= endPage; i++) pages.push(i);
    return pages;
  }

  verEnMapa(city: CiudadDto): void {
    if (city.latitud != null && city.longitud != null) {
      const url = `https://www.google.com/maps/search/?api=1&query=${city.latitud},${city.longitud}`;
      window.open(url, '_blank');
    }
  }

  getCityImage(city: CiudadDto): string {
    const seed = encodeURIComponent(city.nombre || 'city');
    return `https://picsum.photos/seed/${seed}/400/250`;
  }

  obtenerFotoReal(nombreCiudad: string | null | undefined): void {
    if (!nombreCiudad || this.imagenesReales[nombreCiudad]) return;
    fetch(`https://es.wikipedia.org/api/rest_v1/page/summary/${encodeURIComponent(nombreCiudad)}`)
      .then(res => res.json())
      .then(data => {
        if (data.thumbnail && data.thumbnail.source) {
          this.imagenesReales[nombreCiudad] = data.thumbnail.source;
        }
      })
      .catch(() => {});
  }

  guardarDestino(city: CiudadDto): void {
    const fotoUrl = this.imagenesReales[city.nombre || ''] || this.getCityImage(city);
    const destinoAGuardar = {
      nombre: city.nombre,
      paisNombre: city.pais,         
      poblacion: city.paisPoblacion || 0, 
      latitud: city.latitud,
      longitud: city.longitud,
      foto: fotoUrl                  
    };

    this.ciudadService.create(destinoAGuardar as any).subscribe({
      next: () => alert(`¡Éxito! ${city.nombre} se guardó en tu base de datos.`),
      error: (err) => alert(`No se pudo guardar ${city.nombre}. Revisa la consola para más detalles.`)
    });
  }
  calificarDirecto(city: CiudadDto): void {
    // La forma más rápida y limpia sin crear ventanas flotantes nuevas
    // es redirigir al usuario directamente a la pantalla de detalle de esa ciudad.
    
    if (city && city.nombre) {
      this.irADetalle(city.nombre);
    }
  }
  // --- LÓGICA DE NAVEGACIÓN COMPARTIDA ---
  irADetalle(nombreDestino: string) {
    this.router.navigate(['/ciudades', nombreDestino]);
  }
}