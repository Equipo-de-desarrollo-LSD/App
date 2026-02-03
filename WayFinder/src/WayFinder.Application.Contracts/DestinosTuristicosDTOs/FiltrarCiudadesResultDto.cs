using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WayFinder.DestinosTuristicosDTOs
{
    public class FiltrarCiudadesResultDto
    {
        // Reutilizamos CiudadDto porque los datos de la ciudad son los mismos
        public List<CiudadDto> Ciudades { get; set; } = new List<CiudadDto>();
    }
}
