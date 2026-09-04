import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms'; 
import { DestinoTuristicoService } from 'src/app/proxy/destino-turisticos/destino-turistico.service';
import { CiudadDto } from 'src/app/proxy/destinos-turisticos-dtos/models';
import { CalificacionesComponent } from 'src/app/calificaciones/calificaciones';

@Component({
  selector: 'app-detalle-ciudad',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, CalificacionesComponent],
  template: `
    <div class="container mt-4 pb-5">
      <button class="btn btn-outline-secondary mb-4 fw-bold" routerLink="/">
        <i class="bi bi-arrow-left"></i> Volver al inicio
      </button>

      <div *ngIf="loading" class="text-center my-5">
        <div class="spinner-border text-primary" role="status"></div>
        <p class="mt-2 fs-5 text-secondary">Cargando detalles de la ciudad...</p>
      </div>

      <div *ngIf="error" class="alert alert-danger shadow-sm">
        Ocurrió un error al cargar la ciudad. Intenta nuevamente.
      </div>

      <div *ngIf="!loading && !error && ciudad" class="card shadow-sm mb-4 border-0">
        <div class="card-header bg-primary text-white p-4 rounded-top">
          <h2 class="mb-0 fw-bold"><i class="bi bi-building"></i> {{ ciudad.nombre }}</h2>
        </div>
        <div class="card-body p-4 bg-light">
          <div class="row g-4">
            <div class="col-md-6">
              <div class="card border-0 shadow-sm h-100">
                <ul class="list-group list-group-flush fs-5">
                  <li class="list-group-item py-3"><i class="bi bi-geo-alt-fill text-danger me-2"></i><strong>País:</strong> {{ ciudad.pais }}</li>
                  <li class="list-group-item py-3"><i class="bi bi-map-fill text-success me-2"></i><strong>Región:</strong> {{ ciudad.region || 'No disponible' }}</li>
                  <li class="list-group-item py-3"><i class="bi bi-people-fill text-info me-2"></i><strong>Población:</strong> {{ (ciudad.poblacion | number) || 'No disponible' }}</li>
                </ul>
              </div>
            </div>
            <div class="col-md-6">
              <div class="card border-0 shadow-sm h-100">
                <ul class="list-group list-group-flush fs-5">
                  <li class="list-group-item py-3"><strong>Latitud:</strong> {{ ciudad.latitud }}</li>
                  <li class="list-group-item py-3"><strong>Longitud:</strong> {{ ciudad.longitud }}</li>
                </ul>
                <div class="p-3 mt-auto">
                  <button class="btn btn-success btn-lg w-100 fw-bold" *ngIf="ciudad.latitud && ciudad.longitud" (click)="verEnMapa()">
                    <i class="bi bi-geo-alt"></i> Ver en Google Maps
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div *ngIf="!loading && !error && ciudad">
         <app-calificaciones [destinoId]="destinoId" [ciudad]="ciudad"></app-calificaciones>
      </div>
  `
})
export class DetalleCiudadComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private ciudadService = inject(DestinoTuristicoService);

  nombreCiudad: string = '';
  // Cambiamos el tipo a "any" para poder inyectarle la Región y Población sin que TypeScript se queje
  ciudad: any = null; 
  loading = true;
  error = false;
  destinoId: string = ''; 

  ngOnInit(): void {
    this.nombreCiudad = this.route.snapshot.paramMap.get('id') || '';

    if (this.nombreCiudad) {
      // 1. Buscamos el resumen
      this.ciudadService.buscarCiudadesByRequest({ nombreCiudad: this.nombreCiudad }).subscribe({
        next: (res) => {
          if (res.ciudades && res.ciudades.length > 0) {
            const ciudadResumen = res.ciudades[0];
            
            // 🕵️‍♂️ LOG 1: Verificamos si la ciudad tiene un ID válido
            console.log("1. Ciudad encontrada (Resumen):", ciudadResumen);

            if (!ciudadResumen.id || ciudadResumen.id === 0) {
                console.warn("⚠️ ALERTA: El ID de la ciudad es 0 o no existe. Asegúrate de haber reiniciado tu backend en Visual Studio/Rider.");
            }

            // ⏱️ EL TRUCO: Esperamos 1.2 segundos para no saturar el límite gratuito de GeoDB (1 request/segundo)
            setTimeout(() => {
              this.ciudadService.getDetalleCiudad(ciudadResumen.id).subscribe({
                next: (detalleCompleto) => {
                  // 🕵️‍♂️ LOG 2: Vemos qué respondió exactamente el backend
                  console.log("2. Detalle completo recibido:", detalleCompleto);
                  
                  // Fusionamos los datos, atajando tanto 'region' como 'Region'
                  this.ciudad = {
                    ...ciudadResumen,
                    region: detalleCompleto?.region || detalleCompleto?.region,
                    poblacion: detalleCompleto?.poblacion || detalleCompleto?.poblacion
                  };
                  this.verificarSiEstaGuardadoLocally(this.ciudad.nombre);
                },
                error: (err) => {
                  // 🕵️‍♂️ LOG 3: Si la API explota, esto nos dirá por qué
                  console.error("3. Error al intentar obtener los detalles:", err);
                  this.ciudad = ciudadResumen; // Nos quedamos con lo básico
                  this.verificarSiEstaGuardadoLocally(this.ciudad.nombre);
                }
              });
            }, 1200); // 1200 milisegundos de espera

          } else {
            this.error = true;
            this.loading = false;
          }
        },
        error: (err) => {
          this.error = true;
          this.loading = false;
        }
      });
    }
  }
verificarSiEstaGuardadoLocally(nombre: string | undefined): void {
    if (!nombre) return;

    this.ciudadService.getList({ maxResultCount: 1000 }).subscribe({
      next: (res) => {
        // Corrección 1: Aseguramos que si res.items es undefined, use un array vacío
        const destinoGuardado = (res.items || []).find(d => d.nombre?.toLowerCase() === nombre.toLowerCase());
        
        if (destinoGuardado) {
          // Corrección 2: Le decimos que si el id es undefined, asigne un string vacío
          this.destinoId = destinoGuardado.id || '';
        } else {
          this.destinoId = '';
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  verEnMapa(): void {
    if (this.ciudad?.latitud && this.ciudad?.longitud) {
      window.open(`https://www.google.com/maps/search/?api=1&query=${this.ciudad.latitud},${this.ciudad.longitud}`, '_blank');
    }
  }
}