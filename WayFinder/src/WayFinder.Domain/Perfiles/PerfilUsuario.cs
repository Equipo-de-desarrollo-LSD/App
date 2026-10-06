using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace WayFinder.Perfiles
{
    public class PerfilUsuario : FullAuditedEntity<Guid>
    {
        // Usaremos el mismo ID que el usuario de ABP para conectarlos fácil

        public string? Foto { get; set; } // URL de la foto o base64

        public string? Preferencias { get; set; } // Texto libre 

        // Constructor vacío necesario para EF Core
        public PerfilUsuario()
        {
        }

        public PerfilUsuario(Guid id) : base(id)
        {
        }
    }
}
