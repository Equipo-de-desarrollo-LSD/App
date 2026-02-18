using System.ComponentModel.DataAnnotations;

namespace WayFinder.Perfiles
{
    public class ActualizarPerfilDto
    // Este DTO se usará para actualizar tanto los datos de Identity
    // (nombre, apellido, email) como los datos específicos de tu perfil (foto, preferencias)
    {
        // Datos de Identity (Tabla AbpUsers)
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Datos de Tu Perfil (Tabla PerfilesUsuarios)
        public string? Foto { get; set; }
        public string? Preferencias { get; set; }
    }
}