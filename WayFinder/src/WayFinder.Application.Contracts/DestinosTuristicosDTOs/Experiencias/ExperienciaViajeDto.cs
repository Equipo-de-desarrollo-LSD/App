using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using WayFinder.DestinosTuristicos;

namespace WayFinder.DestinosTuristicos
{
    public class ExperienciaViajeDto : AuditedEntityDto<Guid>
    {
        public Guid DestinoTuristicoId { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public TipoExperiencia Sentimiento { get; set; }
        public string CreatorName { get; set; } // Opcional, si quieres mostrar quién escribió
    }
}
