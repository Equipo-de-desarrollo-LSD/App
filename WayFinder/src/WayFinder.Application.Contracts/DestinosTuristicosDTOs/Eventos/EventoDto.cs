using System;

namespace WayFinder.DestinosTuristicosDTOs.Eventos
{
    public class EventoDto
    {
        public string IdExterno { get; set; }
        public string Nombre { get; set; }
        public string UrlTicket { get; set; }
        public DateTime FechaInicio { get; set; }
        public string ImagenUrl { get; set; }
        public string Lugar { get; set; } // Ej: El estadio o teatro
    }
}
