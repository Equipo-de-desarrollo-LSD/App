import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { DestinoTuristicoService } from 'src/app/proxy/destino-turisticos/destino-turistico.service';
import { CiudadDto } from 'src/app/proxy/destinos-turisticos-dtos/models';

@Component({
  selector: 'app-detalle-ciudad',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="container mt-4">
      <button class="btn btn-outline-secondary mb-4" routerLink="/ciudades">
        <i class="bi bi-arrow-left"></i> Volver a resultados
      </button>

      <div *ngIf="loading" class="text-center my-5">
        <div class="spinner-border text-primary" role="status"></div>
        <p class="mt-2">Cargando detalles de la ciudad...</p>
      </div>

      <div *ngIf="error" class="alert alert-danger">
        Ocurrió un error al cargar la ciudad.
      </div>

      <div *ngIf="!loading && !error && ciudad" class="card shadow-sm">
        <div class="card-header bg-primary text-white">
          <h2 class="mb-0">{{ ciudad.nombre }}</h2>
        </div>
        <div class="card-body">
          <div class="row">
            <div class="col-md-6">
              <ul class="list-group list-group-flush">
                <li class="list-group-item"><strong>País:</strong> {{ ciudad.pais }}</li>
                <li class="list-group-item"><strong>Región:</strong> {{ ciudad.region || 'No disponible' }}</li>
                <li class="list-group-item"><strong>Población:</strong> {{ (ciudad.poblacion | number) || 'No disponible' }}</li>
              </ul>
            </div>
            <div class="col-md-6">
              <ul class="list-group list-group-flush">
                <li class="list-group-item"><strong>Latitud:</strong> {{ ciudad.latitud }}</li>
                <li class="list-group-item"><strong>Longitud:</strong> {{ ciudad.longitud }}</li>
              </ul>
              <button class="btn btn-success w-100 mt-3" *ngIf="ciudad.latitud && ciudad.longitud" (click)="verEnMapa()">
                <i class="bi bi-geo-alt"></i> Ver en Google Maps
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class DetalleCiudadComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private ciudadService = inject(DestinoTuristicoService);

  nombreCiudad: string = '';
  ciudad: CiudadDto | null = null;
  loading = true;
  error = false;

  ngOnInit(): void {
    // Atrapamos el nombre que viene en la URL
    this.nombreCiudad = this.route.snapshot.paramMap.get('id') || '';

    if (this.nombreCiudad) {
      // Le pedimos al backend que busque los datos usando el nombre
      this.ciudadService.buscarCiudad({ nombreCiudad: this.nombreCiudad }).subscribe({
        next: (res) => {
          if (res.ciudades && res.ciudades.length > 0) {
            this.ciudad = res.ciudades[0]; // Mostramos la primera coincidencia
          } else {
            this.error = true;
          }
          this.loading = false;
        },
        error: (err) => {
          console.error(err);
          this.error = true;
          this.loading = false;
        }
      });
    }
  }

  verEnMapa(): void {
    if (this.ciudad?.latitud && this.ciudad?.longitud) {
      window.open(`https://www.google.com/maps/search/?api=1&query=${this.ciudad.latitud},${this.ciudad.longitud}`, '_blank');
    }
  }
}