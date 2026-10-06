import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
// Asegúrate de que esta ruta coincida con donde está tu PerfilService
import { PerfilService } from 'src/app/proxy/perfiles'; 
import { PerfilPublicoDto } from 'src/app/proxy/destinos-turisticos-dtos/perfiles'; // O la ruta correcta a tu DTO

@Component({
  selector: 'app-perfil-publico',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './perfil-publico.html',
  styleUrls: ['./perfil-publico.scss']
})
export class PerfilPublicoComponent implements OnInit {
  // Herramienta para leer la URL (el /:id)
  private route = inject(ActivatedRoute);
  // Tu servicio conectado al backend
  private perfilService = inject(PerfilService);

  perfil: PerfilPublicoDto | null = null;
  cargando = true;
  error = false;

  ngOnInit(): void {
    // Leemos el 'id' que viene en la URL
    const id = this.route.snapshot.paramMap.get('id');
    
    if (id) {
      // Llamamos al método que viste en Swagger (puede llamarse getPerfilPublico o similar)
      this.perfilService.getPerfilPublico(id).subscribe({
        next: (datos) => {
          this.perfil = datos;
          this.cargando = false;
        },
        error: (err) => {
          console.error('Error al cargar el perfil público', err);
          this.error = true;
          this.cargando = false;
        }
      });
    } else {
      this.error = true;
      this.cargando = false;
    }
  }

  volver(): void {
    window.history.back(); // Botón simple para volver a la pantalla anterior
  }
}