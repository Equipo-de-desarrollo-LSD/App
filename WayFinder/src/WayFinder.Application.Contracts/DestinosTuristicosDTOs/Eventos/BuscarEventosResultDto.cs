using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayFinder.DestinosTuristicosDTOs.Eventos;

namespace WayFinder.DestinosTuristicosDTOs
{
    public class BuscarEventosResultDto
    {
        public List<EventoDto> Eventos { get; set; } = new List<EventoDto>();
        public int TotalResultados { get; set; }
    }
}