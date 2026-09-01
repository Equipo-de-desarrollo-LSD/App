using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Threading;
using WayFinder.DestinosTuristicos;
using WayFinder.Eventos; // AGREGADO: Para reconocer la clase Evento

namespace WayFinder.Workers
{
    public class TicketMasterEventosWorker : AsyncPeriodicBackgroundWorkerBase
    {
        // Dependencias
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;
        private readonly IEventosService _eventosService;
        public TicketMasterEventosWorker(
            AbpAsyncTimer timer,
            IServiceScopeFactory serviceScopeFactory,
            IRepository<DestinoTuristico, Guid> destinoRepository,
            IEventosService eventosService)
            : base(timer, serviceScopeFactory)
        {
            _destinoRepository = destinoRepository;
            _eventosService = eventosService;
            // Configurar que se ejecute cada 24 horas (en milisegundos)
            Timer.Period = 86400000;
        }
        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var destinoRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<DestinoTuristico, Guid>>();
            var eventosService = workerContext.ServiceProvider.GetRequiredService<IEventosService>();
            
            // AGREGADO: Descomentado y apuntando a Evento
            var eventoRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<Evento, Guid>>();

            var destinos = await destinoRepository.GetListAsync();

            foreach (var destino in destinos)
            {
                if (destino.Coordenadas != null)
                {
                    try
                    {
                        var resultadoTicketMaster = await eventosService.ObtenerEventosPorCoordenadasAsync(
                            destino.Coordenadas.latitud,
                            destino.Coordenadas.longitud
                        );

                        if (resultadoTicketMaster != null && resultadoTicketMaster.Eventos != null)
                        {
                            foreach (var eventoExterno in resultadoTicketMaster.Eventos)
                            {
                                // AGREGADO: Mapeo correcto de propiedades
                                var nuevoEvento = new Evento(
                                    id: Guid.NewGuid(), // O GuidGenerator.Create() si inyectas IGuidGenerator
                                    destinoTuristicoId: destino.Id,
                                    idExterno: eventoExterno.IdExterno,
                                    nombre: eventoExterno.Nombre,
                                    urlTicket: eventoExterno.UrlTicket,
                                    fechaInicio: eventoExterno.FechaInicio,
                                    imagenUrl: eventoExterno.ImagenUrl,
                                    lugar: eventoExterno.Lugar
                                );
                                await eventoRepository.InsertAsync(nuevoEvento);
                            }
                            
                            destino.UltimaActualizacion = DateTime.Now;
                            await destinoRepository.UpdateAsync(destino);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Logger.LogWarning(...)
                    }
                }
            }
        }
    }
}
