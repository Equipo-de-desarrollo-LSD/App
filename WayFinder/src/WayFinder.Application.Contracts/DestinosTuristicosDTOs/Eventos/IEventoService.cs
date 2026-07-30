using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayFinder.DestinosTuristicosDTOs;

namespace WayFinder.DestinosTuristicos
{
    public interface IEventosService
    {
        Task<BuscarEventosResultDto> ObtenerEventosPorCoordenadasAsync(double latitud, double longitud);
    }
}
