using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WayFinder.NotificacionesDTOs
{
    public interface INotificacionAppService : IApplicationService
    {
        // Para listar las notificaciones del usuario actual
        Task<List<NotificacionDto>> GetListaAsync();

        // Marcar una como leída
        Task MarcarComoLeidaAsync(Guid id);

        // Marcar una como no leída
        Task MarcarComoNoLeidaAsync(Guid id);

        //// Extra: Útil para la UI (ej. el círculo rojo de notificaciones)
        //Task<int> GetCountNoLeidasAsync();
    }
}
