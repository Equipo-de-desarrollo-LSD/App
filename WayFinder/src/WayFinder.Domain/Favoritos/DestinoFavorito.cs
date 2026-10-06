using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace WayFinder.Favoritos
{
    // Usamos CreationAuditedEntity para saber QUIÉN lo guardó (CreatorId) automáticamente.
    public class DestinoFavorito : CreationAuditedEntity<Guid>
    {
        // Solo guardamos el ID del destino. La info (foto, nombre) la buscamos después.
        public Guid DestinoTuristicoId { get; set; }

        // Constructor vacío (requerido por ORM)
        protected DestinoFavorito() { }

        // Constructor principal
        public DestinoFavorito(Guid id, Guid destinoTuristicoId)
            : base(id)
        {
            DestinoTuristicoId = destinoTuristicoId;
        }
    }
}