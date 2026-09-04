using System;
using Volo.Abp.Application.Dtos;

namespace WayFinder.Perfiles
{
    public class PerfilDto : EntityDto<Guid>
    // Este DTO representa la información del perfil de un usuario que se mostrará en la aplicación.
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        public string? Foto { get; set; }
        public string? Preferencias { get; set; }
    }
}