using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WayFinder.DestinosTuristicosDTOs
{
    public class DestinoTuristicoDto : AuditedEntityDto<Guid>
    {
        public Guid Id { get; set; }
        public string nombre { get; set; }
        public string foto { get; set; }
        public DateTime ultimaActualizacion { get; set; }
        public PaisDto pais { get; set; }
        public CoordenadasDto coordenadas { get; set; }
    }
}
