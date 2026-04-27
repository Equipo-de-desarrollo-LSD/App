import { Component, inject } from '@angular/core';
import { AuthService, LocalizationPipe } from '@abp/ng.core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  imports: [LocalizationPipe]
})
export class HomeComponent {
  // TODO: Reemplazar este Mock con la llamada al backend cuando esté listo el merge de Favoritos
  destinosPopulares = [
    {
      nombre: 'Buenos Aires',
      pais: 'Argentina',
      imagenUrl: 'https://images.unsplash.com/photo-1612294037345-b4eb04c96e1b?q=80&w=800&auto=format&fit=crop',
      favoritos: 1540,
      calificacion: 4.8
    },
    {
      nombre: 'Bariloche',
      pais: 'Argentina',
      imagenUrl: 'https://images.unsplash.com/photo-1522228115018-d838bcce5c3a?q=80&w=800&auto=format&fit=crop',
      favoritos: 1205,
      calificacion: 4.9
    },
    {
      nombre: 'Mendoza',
      pais: 'Argentina',
      imagenUrl: 'https://images.unsplash.com/photo-1584464491033-06628f3a6b7b?q=80&w=800&auto=format&fit=crop',
      favoritos: 980,
      calificacion: 4.7
    }
  ];
  private authService = inject(AuthService);

  get hasLoggedIn(): boolean {
    return this.authService.isAuthenticated
  }

  login() {
    this.authService.navigateToLogin();
  }
}

