using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WayFinder.Favoritos
{
    public class DestinoFavoritoDto : EntityDto<Guid>
    {
        public Guid DestinoTuristicoId { get; set; }

        // Agregamos estos campos para facilitar la visualización en la lista
        public string NombreDestino { get; set; }
        public string FotoDestino { get; set; }
        public string PaisDestino { get; set; }
    }
}
