using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WayFinder.DestinosTuristicosDTOs
{
    public class CoordenadasDto : AuditedEntityDto<Guid>
    {
        public double latitud { get; set; }
        public double longitud { get; set; }
    }
}
