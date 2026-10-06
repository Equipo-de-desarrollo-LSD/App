using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace WayFinder
{
    public class CalificacionDestino : AuditedAggregateRoot<Guid>, IUserOwned
    {
        public Guid DestinoId { get; set; }
        public Guid UserId { get; set; }
        public int Puntaje { get; set; }
        public string? Comentario { get; set; }

        public CalificacionDestino() { }

        public CalificacionDestino(Guid destinoId, Guid userId, int puntaje, string? comentario = null) : base(userId)
        {
            DestinoId = destinoId;
            UserId = userId;
            setPuntaje(puntaje);
            Comentario = string.IsNullOrWhiteSpace(comentario) ? null : comentario.Trim();
        }
        public void setPuntaje(int puntaje)
        {
            if (puntaje < 1 || puntaje > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(puntaje), "El puntaje debe estar entre 1 y 5.");
            }
            Puntaje = puntaje;
        }
        public void updateComentario(string? comentario, int?  puntaje = null)
        {
            if (puntaje.HasValue)
            {
                setPuntaje(puntaje.Value);
            }
            Comentario = string.IsNullOrWhiteSpace(comentario) ? null : comentario.Trim();
        }
    }
}