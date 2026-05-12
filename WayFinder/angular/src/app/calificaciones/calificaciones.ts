import { Component, Input, OnChanges, SimpleChanges, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CalificacionService } from 'src/app/proxy/calificacion/calificacion.service';
import { CalificacionDto } from 'src/app/proxy/destinos-turisticos-dtos/models';
import { ConfigStateService } from '@abp/ng.core';

@Component({
  selector: 'app-calificaciones',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './calificaciones.html',
  styleUrls: ['./calificaciones.scss']
})
export class CalificacionesComponent implements OnChanges {
  @Input() destinoId!: string; 

  private calificacionService = inject(CalificacionService);
  private configState = inject(ConfigStateService); // NUEVO: Inyectamos el estado general

  currentUser = this.configState.getOne('currentUser'); // NUEVO: Obtenemos el usuario logueado
  miCalificacionPrevia: any = null;
  listaComentarios: CalificacionDto[] = [];
  promedio: number = 0;
  totalResenas: number = 0;

  // --- VARIABLES DEL FORMULARIO INTEGRADO ---
  mostrarFormulario: boolean = false;
  estrellas = [1, 2, 3, 4, 5];
  puntajeSeleccionado: number = 0;
  puntajeHover: number = 0; 
  textoComentario: string = '';
  
  // NUEVO: Variable para saber si estamos editando
  calificacionAEditarId: string | null = null; 

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['destinoId'] && this.destinoId) {
      this.cargarComentarios();
    }
  }

 cargarComentarios() {
    this.calificacionService.getCalificacionesPorDestino(this.destinoId).subscribe(comentarios => {
      this.listaComentarios = comentarios;
      this.totalResenas = comentarios.length;

      // NUEVO: Buscamos si en la lista hay un comentario que me pertenezca
      // Nota: Si tu DTO usa otro nombre en vez de 'creatorId' (ej: 'usuarioId'), cámbialo aquí
      this.miCalificacionPrevia = this.listaComentarios.find(c => c.userId === this.currentUser.id);
    });

    this.calificacionService.getPromedio(this.destinoId).subscribe(promedio => {
      this.promedio = promedio;
    });
  }

  // --- MÉTODOS DE LA INTERFAZ ---

  toggleFormulario() {
    this.mostrarFormulario = !this.mostrarFormulario;
    if (!this.mostrarFormulario) {
      this.limpiarFormulario(); // Limpiamos al cerrar
    }
  }

  limpiarFormulario() {
    this.puntajeSeleccionado = 0;
    this.textoComentario = '';
    this.calificacionAEditarId = null;
  }

  seleccionarEstrella(valor: number) {
    this.puntajeSeleccionado = valor;
  }

  hoverEstrella(valor: number) {
    this.puntajeHover = valor;
  }

  // NUEVO: Método para preparar el formulario para editar
  editarMiCalificacion(item: CalificacionDto) {
    this.calificacionAEditarId = item.id || null;
    this.puntajeSeleccionado = item.puntaje;
    this.textoComentario = item.comentario || '';
    this.mostrarFormulario = true;
    
    // Hacemos scroll suave hacia arriba para ver el formulario
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  guardarCalificacion() {
    if (this.puntajeSeleccionado === 0) return; 

    const input = {
      destinoId: this.destinoId,
      puntaje: this.puntajeSeleccionado, 
      comentario: this.textoComentario
    };
    
    if (this.calificacionAEditarId) {
      // MODO EDICIÓN (Usamos .update() de ABP)
      this.calificacionService.update(this.calificacionAEditarId, input as any).subscribe({
        next: () => {
          this.mostrarFormulario = false;
          this.limpiarFormulario();
          this.cargarComentarios(); 
        },
        error: (err) => console.error('Error al editar', err)
      });
    } else {
      // MODO CREACIÓN (Usamos .create() de ABP)
      this.calificacionService.create(input as any).subscribe({
        next: () => {
          this.mostrarFormulario = false;
          this.limpiarFormulario();
          this.cargarComentarios();
        },
        error: (err) => console.error('Error al guardar', err)
      });
    }
  }

  eliminarMiCalificacion(id: string) {
    if(confirm('¿Estás seguro de que deseas eliminar esta calificación?')) {
      this.calificacionService.delete(id).subscribe(() => {
        this.cargarComentarios();
      });
    }
  }

  getIconoEstrella(posicion: number, valorExacto: number): string {
    if (valorExacto >= posicion) {
      return 'bi-star-fill text-warning';
    } else if (valorExacto >= posicion - 0.5) {
      return 'bi-star-half text-warning';
    } else {
      return 'bi-star text-secondary';
    }
  }
}