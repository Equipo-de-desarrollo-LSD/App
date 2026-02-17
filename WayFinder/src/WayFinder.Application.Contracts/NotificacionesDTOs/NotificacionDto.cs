using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WayFinder.NotificacionesDTOs
{
    public class NotificacionDto : EntityDto<Guid>
    {
        public Guid UsuarioId { get; set; }
        public Guid DestinoId { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaHora { get; set; }
        public bool Leido { get; set; }
    }
}
