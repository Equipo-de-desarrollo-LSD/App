using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;
using WayFinder.DestinosTuristicos;

namespace WayFinder.Notificaciones
{
    // Cada vez que una entidad se actualiza, el framework dispara un evento.
    public class DestinoUpdateHandler : ILocalEventHandler<EntityUpdatedEventData<DestinoTuristico>>, ITransientDependency
    {
        private readonly IRepository<Notificacion, Guid> _notificacionRepository;
        private readonly IRepository<ListaSeguimiento, Guid> _listaSeguimientoRepository;

        public DestinoUpdateHandler(
            IRepository<Notificacion, Guid> notificacionRepository,
            IRepository<ListaSeguimiento, Guid> listaSeguimientoRepository)
        {
            _notificacionRepository = notificacionRepository;
            _listaSeguimientoRepository = listaSeguimientoRepository;
        }

        public async Task HandleEventAsync(EntityUpdatedEventData<DestinoTuristico> eventData)
        {
            var destino = eventData.Entity;

            // Buscamos usuarios que tengan este destino en su ListaSeguimiento
            var interesados = await _listaSeguimientoRepository.GetListAsync(x => x.DestinoId == destino.Id);

            foreach (var seguimiento in interesados)
            {
                await _notificacionRepository.InsertAsync(new Notificacion(
                    Guid.NewGuid(),
                    seguimiento.UsuarioId,
                    destino.Id,
                    "Actualización de Destino",
                    $"El destino {destino.nombre} tiene nuevos cambios relevantes."
                ));
            }
        }
    }
}
