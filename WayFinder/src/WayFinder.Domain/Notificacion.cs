using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace WayFinder
{
    public class Notificacion : FullAuditedEntity<Guid>
    {
        public Guid UsuarioId { get; set; } // FK al Usuario
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaHora { get; set; }
        public bool Leido { get; set; }
        public Guid DestinoId { get; set; } // Relación con DestinoTuristico

        // Constructor para ABP
        protected Notificacion() { }

        public Notificacion(Guid id, Guid usuarioId, Guid destinoId, string titulo, string mensaje) : base(id)
        {
            UsuarioId = usuarioId;
            DestinoId = destinoId;
            Titulo = titulo;
            Mensaje = mensaje;
            FechaHora = DateTime.Now;
            Leido = false;
        }
    }
}
