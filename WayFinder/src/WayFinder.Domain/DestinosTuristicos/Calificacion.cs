using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Entities; // <-- ¡Necesitas este using para IUserOwned!

namespace WayFinder.DestinosTuristicos
{
    // Implementamos IUserOwned para que ABP sepa que esta entidad "pertenece" a un usuario
    public class Calificacion : AuditedAggregateRoot<Guid>, IUserOwned
    {
        public string Comentario { get; set; }
        public int Puntaje { get; set; } // ej. 1 a 5 estrellas

        // Clave foránea para vincularla al destino
        public Guid DestinoId { get; set; }

        // --- IMPLEMENTACIÓN DE IUserOwned ---
        // Esta es la propiedad que te pide la consigna
        // ABP la usará para filtrar automáticamente
        public Guid UserId { get; set; }

        // Constructor
        protected Calificacion() { }

        public Calificacion(Guid id, Guid destinoId, Guid userId, int puntaje, string comentario) : base(id)
        {
            DestinoId = destinoId;
            UserId = userId; // <-- Se asigna el dueño
            Puntaje = puntaje;
            Comentario = comentario;
        }
    }
}
