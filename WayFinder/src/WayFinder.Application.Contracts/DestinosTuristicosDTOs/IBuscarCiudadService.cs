using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayFinder.DestinosTuristicosDTOs
{
    public interface IBuscarCiudadService
    {
        Task<BuscarCiudadResultDto> SearchCitiesAsync(BuscarCiudadRequestDto request);
        
    }
}

