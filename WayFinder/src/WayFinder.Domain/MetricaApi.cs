using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace WayFinder
{
    public class MetricaApi : Entity<Guid>
    {
        public string NombreServicio { get; set; } // Ejemplo: "GeoDB"
        public string Endpoint { get; set; }
        public DateTime FechaEjecucion { get; set; }
        public int StatusCode { get; set; }
        public long TiempoRespuestaMs { get; set; }

        public MetricaApi(Guid id, string nombreServicio, string endpoint, int statusCode, long tiempoRespuestaMs)
            : base(id)
        {
            NombreServicio = nombreServicio;
            Endpoint = endpoint;
            StatusCode = statusCode;
            TiempoRespuestaMs = tiempoRespuestaMs;
            FechaEjecucion = DateTime.Now;
        }
    }
}
