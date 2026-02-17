using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace WayFinder.DestinosTuristicos
{
    // 
    public class ExperienciaViaje : AuditedEntity<Guid>
    {
        public Guid DestinoTuristicoId { get; set; } // Conexión con tu entidad actual
        public string Titulo { get; set; }
        public string Contenido { get; set; } // El relato largo
        public TipoExperiencia Sentimiento { get; set; } // El filtro obligatorio
    }
}
