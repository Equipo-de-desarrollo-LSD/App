using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WayFinder.DestinosTuristicosDTOs
{
    public interface IListaSeguimientoAppService : IApplicationService
    {
        // El usuario "asigna" o sigue el destino
        Task SeguirDestinoAsync(Guid destinoId);

        // El usuario deja de seguir el destino
        Task DejarDeSeguirDestinoAsync(Guid destinoId);
    }
}
