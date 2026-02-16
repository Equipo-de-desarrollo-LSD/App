using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WayFinder.MetricasDTOs
{
    public class MetricaDto : EntityDto<Guid>
    {
        public string NombreServicio { get; set; } // Ejemplo: "GeoDB"
        public string Endpoint { get; set; }
        public DateTime FechaEjecucion { get; set; }
        public long TiempoRespuestaMs { get; set; }
    }
}
