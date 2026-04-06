using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace WayFinder.Perfiles
{
    [Authorize] // Solo usuarios logueados pueden entrar aquí
    public class PerfilAppService : WayFinderAppService, IPerfilAppService
    {
        // Necesitamos dos repositorios:
        // 1. Tabla nueva (para la foto y preferencias)
        private readonly IRepository<PerfilUsuario, Guid> _perfilRepository;

        // 2.Gestor de usuarios de ABP (para nombre, apellido, email)
        private readonly IdentityUserManager _userManager;

        public PerfilAppService(
            IRepository<PerfilUsuario, Guid> perfilRepository,
            IdentityUserManager userManager)
        {
            _perfilRepository = perfilRepository;
            _userManager = userManager;
        }

        public async Task<PerfilDto> GetMiPerfilAsync()
        {
            // 1. Obtenemos el ID del usuario actual
            var userId = CurrentUser.GetId();

            // 2. Buscamos los datos básicos en la tabla de ABP Users
            var user = await _userManager.GetByIdAsync(userId);

            // 3. Buscamos los datos extra en tabla PerfilesUsuarios
            // (Puede ser null si es la primera vez que entra)
            var perfil = await _perfilRepository.FindAsync(userId);

            // 4. Combinamos todo en el DTO para enviarlo al Frontend
            return new PerfilDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Nombre = user.Name,
                Apellido = user.Surname,

                // Si nunca guardó perfil, mandamos null
                Foto = perfil?.Foto,
                Preferencias = perfil?.Preferencias
            };
        }

        public async Task UpdateMiPerfilAsync(ActualizarPerfilDto input)
        {
            var userId = CurrentUser.GetId();

            // Actualizar tabla ABP Users 
            var user = await _userManager.GetByIdAsync(userId);

            user.Name = input.Nombre;
            user.Surname = input.Apellido;

            // Si cambió el email, ABP tiene un método especial para validarlo
            if (user.Email != input.Email)
            {
                await _userManager.SetEmailAsync(user, input.Email);
            }

            await _userManager.UpdateAsync(user);

            // Actualizar tabla PerfilesUsuarios
            var perfil = await _perfilRepository.FindAsync(userId);

            if (perfil == null)
            {
                // Si no existe la fila, la CREAMOS (Insert)
                perfil = new PerfilUsuario(userId);
                perfil.Foto = input.Foto;
                perfil.Preferencias = input.Preferencias;

                await _perfilRepository.InsertAsync(perfil);
            }
            else
            {
                // Si ya existe, la ACTUALIZAMOS (Update)
                perfil.Foto = input.Foto;
                perfil.Preferencias = input.Preferencias;

                await _perfilRepository.UpdateAsync(perfil);
            }
        }
            public async Task EliminarMiCuentaAsync()
            {
            // 1. Obtenemos el ID del usuario que está logueado
            var userId = CurrentUser.GetId();

            // 2. Buscamos si tiene datos en nuestra tabla personalizada (PerfilUsuario)
            var perfil = await _perfilRepository.FindAsync(userId);
            if (perfil != null)
            {
                // Borramos sus datos extra (Foto, Preferencias)
                await _perfilRepository.DeleteAsync(perfil);
            }

            // 3. Buscamos su usuario principal de ABP (Login, Password, Email)
            var user = await _userManager.GetByIdAsync(userId);

            // 4. Eliminamos la cuenta por completo
            // Nota: ABP usa "Soft Delete", así que no rompe la base de datos, 
            // solo lo marca como IsDeleted = true.
            await _userManager.DeleteAsync(user);
        }
    }
}
