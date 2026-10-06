using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace WayFinder.DestinosTuristicos
{
    public class ListaSeguimiento : FullAuditedEntity<Guid>
    {
        public Guid UsuarioId { get; set; }
        public Guid DestinoId { get; set; }
        public DateTime FechaCreacion { get; set; }

        protected ListaSeguimiento() { }

        public ListaSeguimiento(Guid id, Guid usuarioId, Guid destinoId) : base(id)
        {
            UsuarioId = usuarioId;
            DestinoId = destinoId;
            FechaCreacion = DateTime.Now;
        }
    }
}
