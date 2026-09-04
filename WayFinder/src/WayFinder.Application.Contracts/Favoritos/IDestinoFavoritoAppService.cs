using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WayFinder.Favoritos
{
    public interface IDestinoFavoritoAppService : IApplicationService
    {
        // 6.1 Agregar favorito
        Task CreateAsync(CreateDestinoFavoritoDto input);

        // 6.2 Eliminar favorito (por ID del destino, para que sea más fácil desde la UI)
        Task DeleteByDestinoIdAsync(Guid destinoId);

        // 6.3 Listar mis favoritos (Paginado y filtrado estándar)
        Task<PagedResultDto<DestinoFavoritoDto>> GetListAsync(PagedAndSortedResultRequestDto input);

        // Extra: Saber si un destino ya es favorito (útil para pintar el corazón lleno o vacío)
        Task<bool> IsFavoritoAsync(Guid destinoId);

    }
}
