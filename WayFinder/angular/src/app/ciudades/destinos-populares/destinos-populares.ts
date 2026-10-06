import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DestinoTuristicoService } from 'src/app/proxy/destino-turisticos/destino-turistico.service';

@Component({
  selector: 'app-destinos-populares',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './destinos-populares.html',
  styleUrls: ['./destinos-populares.scss']
})
export class DestinosPopularesComponent implements OnInit {
  private destinoService = inject(DestinoTuristicoService);

  destinosPopulares: any[] = [];
  loading = true;
  errorMsg = '';
  imagenesReales: { [nombreCiudad: string]: string } = {};

  ngOnInit(): void {
    this.cargarPopulares();
  }

  cargarPopulares(): void {
    // Usamos el endpoint GET general por ahora. ABP suele pedir un objeto vacío {} para los parámetros de paginación por defecto.
    this.destinoService.getList({maxResultCount: 100, skipCount: 0}).subscribe({
      next: (res) => {
        this.destinosPopulares = res.items || [];
        this.destinosPopulares.forEach(destino => this.obtenerFotoReal(destino.nombre));
        this.loading = false;
      },
      error: (err) => {
        console.error('Error:', err);
        this.errorMsg = 'Hubo un error al cargar los destinos populares.';
        this.loading = false;
      }
    });
  }

  obtenerFotoReal(nombreCiudad: string | null | undefined): void {
    if (!nombreCiudad || this.imagenesReales[nombreCiudad]) return;
    fetch(`https://es.wikipedia.org/api/rest_v1/page/summary/${encodeURIComponent(nombreCiudad)}`)
      .then(res => res.json())
      .then(data => {
        if (data.thumbnail?.source) this.imagenesReales[nombreCiudad] = data.thumbnail.source;
      })
      .catch(() => {});
  }

  getCityImage(city: any): string {
    const seed = encodeURIComponent(city.nombre || 'popular');
    return `https://picsum.photos/seed/${seed}/400/250`;
  }
}