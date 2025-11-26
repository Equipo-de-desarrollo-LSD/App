using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Entities;
using WayFinder;

namespace WayFinder.Calificaciones
{
    // Implementamos IUserOwned para que ABP sepa que esta entidad "pertenece" a un usuario
    public class Calificacion : FullAuditedAggregateRoot<Guid>, IUserOwned
    {
        public string? Comentario { get; set; }
        public int Puntaje { get; set; } // ej. 1 a 5 estrellas
        public Guid DestinoId { get; set; }
        public Guid UserId { get; set; }
    
        public Calificacion() { }

        public Calificacion(Guid id, Guid destinoId, Guid userId, int puntaje, string comentario) : base(id)
        {
            DestinoId = destinoId;
            UserId = userId; 
            Puntaje = puntaje;
            Comentario = comentario;
        }

    }
}
