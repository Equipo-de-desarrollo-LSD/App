using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using WayFinder.DestinosTuristicos;

namespace WayFinder.DestinosTuristicos
{
    public class GetExperienciasInput : PagedAndSortedResultRequestDto
    {
        public Guid? DestinoTuristicoId { get; set; } // Para ver experiencias de UN destino (4.4)
        public TipoExperiencia? Sentimiento { get; set; } // Filtro Positiva/Negativa (4.5)
        public string Filter { get; set; } // Buscador de palabras clave (4.6)
    }
}
