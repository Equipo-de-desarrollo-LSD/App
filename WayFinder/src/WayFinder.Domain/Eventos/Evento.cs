using System;
using Volo.Abp.Domain.Entities;

namespace WayFinder.Eventos
{
    public class Evento : Entity<Guid>
    {
        public Guid DestinoTuristicoId { get; set; }
        
        public string IdExterno { get; set; }
        public string Nombre { get; set; }
        public string UrlTicket { get; set; }
        public DateTime FechaInicio { get; set; }
        public string ImagenUrl { get; set; }
        public string Lugar { get; set; }

        // Constructor vacío requerido por Entity Framework Core
        protected Evento()
        {
        }

        public Evento(
            Guid id, 
            Guid destinoTuristicoId, 
            string idExterno, 
            string nombre, 
            string urlTicket, 
            DateTime fechaInicio, 
            string imagenUrl, 
            string lugar) 
            : base(id)
        {
            DestinoTuristicoId = destinoTuristicoId;
            IdExterno = idExterno ?? string.Empty;
            Nombre = nombre ?? string.Empty;
            UrlTicket = urlTicket ?? string.Empty;
            FechaInicio = fechaInicio;
            ImagenUrl = imagenUrl ?? string.Empty;
            Lugar = lugar ?? string.Empty;
        }
    }
}
