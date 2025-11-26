using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WayFinder.DestinosTuristicosDTOs
{
    public class PaisDto : AuditedEntityDto<Guid>
    {
        public string nombre { get; set; }
        public double poblacion { get; set; }
    }
}
