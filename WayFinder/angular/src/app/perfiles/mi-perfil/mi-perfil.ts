import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { PerfilService } from 'src/app/proxy/perfiles';


//import { PerfilService } from '@proxy/perfiles/perfil.service'; 


@Component({
  selector: 'app-mi-perfil',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './mi-perfil.html',
  styleUrl: './mi-perfil.scss'
})

export class MiPerfil implements OnInit {
  perfilForm: FormGroup;
  estaGuardando = false;

  constructor(
    private fb: FormBuilder,
    private perfilService: PerfilService
  ) {
    // 1. Armamos el formulario vacío con sus validaciones
    this.perfilForm = this.fb.group({
      nombre: ['', Validators.required],
      apellido: ['', Validators.required],
      foto: [''],
      preferencias: ['']
    });
  }

  ngOnInit(): void {
    // 2. Apenas se abre la pantalla, mandamos a buscar los datos del usuario
    this.cargarPerfil();
  }

  cargarPerfil() {
    this.perfilService.getMiPerfil().subscribe({
      next: (perfil) => {
        // 3. Cuando el backend responde, rellenamos el formulario con sus datos
        this.perfilForm.patchValue({
          nombre: perfil.nombre,
          apellido: perfil.apellido,
          foto: perfil.foto,
          preferencias: perfil.preferencias
        });
      },
      error: (err) => console.error('Error al cargar el perfil', err)
    });
  }

  guardarCambios() {
    if (this.perfilForm.invalid) return;

    this.estaGuardando = true;
    const datosActualizados = this.perfilForm.value; 

    // 4. Enviamos los datos nuevos al backend para que los guarde
    this.perfilService.updateMiPerfil(datosActualizados).subscribe({
      next: () => {
        alert('¡Tus datos se actualizaron con éxito!');
        this.estaGuardando = false;
      },
      error: (err) => {
        console.error('Error al guardar', err);
        alert('Hubo un error al guardar los cambios.');
        this.estaGuardando = false;
      }
    });
  }
}












// import { Component } from '@angular/core';

// @Component({
//   selector: 'app-mi-perfil',
//   imports: [],
//   templateUrl: './mi-perfil.html',
//   styleUrl: './mi-perfil.scss'
// })
// export class MiPerfil {

// }
