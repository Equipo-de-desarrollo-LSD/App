using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Threading;
using WayFinder.DestinosTuristicosDTOs;
using WayFinder.Eventos;
using WayFinder.Workers;
using WayFinder.DestinosTuristicos;
using DestinoEntidad = WayFinder.DestinosTuristicos.DestinoTuristico;
using Xunit;

namespace WayFinder.Application.Tests.Eventos
{
    public class TestableTicketMasterEventosWorker : TicketMasterEventosWorker
    {
        public TestableTicketMasterEventosWorker(
            AbpAsyncTimer timer,
            IServiceScopeFactory serviceScopeFactory,
            IRepository<DestinoEntidad, Guid> destinoRepository,
            IEventosService eventosService,
            IConfiguration configuration)
            : base(timer, serviceScopeFactory, destinoRepository, eventosService, configuration)
        {
        }

        public async Task TestDoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            await DoWorkAsync(workerContext);
        }
    }

    public class TicketMasterEventosWorker_Tests
    {
        [Fact]
        public async Task DoWorkAsync_DebeRecorrerDestinosYGuardarEventos()
        {
            var timerMock = new Mock<AbpAsyncTimer>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var configurationMock = new Mock<IConfiguration>();
            
            var destinoRepositoryMock = new Mock<IRepository<DestinoEntidad, Guid>>();
            var eventosServiceMock = new Mock<IEventosService>();
            var eventoRepositoryMock = new Mock<IRepository<Evento, Guid>>();

            var configSectionMock = new Mock<IConfigurationSection>();
            configSectionMock.Setup(c => c.Value).Returns("86400000");
            configurationMock.Setup(c => c.GetSection("TicketMaster:WorkerPeriodMs")).Returns(configSectionMock.Object);

            var destinoId = Guid.NewGuid();
            var destinosFalsos = new List<DestinoEntidad>
            {
                new DestinoEntidad(destinoId)
                {
                    nombre = "Ciudad Gótica",
                    foto = "test.jpg",
                    Coordenadas = new WayFinder.DestinosTuristicos.Coordenadas(10, 20)
                }
            };
            
            destinoRepositoryMock.Setup(r => r.GetListAsync(false, default)).ReturnsAsync(destinosFalsos);

            var eventosTicketMaster = new WayFinder.DestinosTuristicosDTOs.BuscarEventosResultDto
            {
                Eventos = new List<WayFinder.DestinosTuristicosDTOs.Eventos.EventoDto>
                {
                    new WayFinder.DestinosTuristicosDTOs.Eventos.EventoDto { IdExterno = "Ext1", Nombre = "Evento Test" }
                }
            };
            
            eventosServiceMock
                .Setup(s => s.ObtenerEventosPorCoordenadasAsync(10, 20))
                .ReturnsAsync(eventosTicketMaster);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(IRepository<DestinoEntidad, Guid>))).Returns(destinoRepositoryMock.Object);
            serviceProviderMock.Setup(sp => sp.GetService(typeof(IEventosService))).Returns(eventosServiceMock.Object);
            serviceProviderMock.Setup(sp => sp.GetService(typeof(IRepository<Evento, Guid>))).Returns(eventoRepositoryMock.Object);

            var workerContext = new PeriodicBackgroundWorkerContext(serviceProviderMock.Object);

            var worker = new TestableTicketMasterEventosWorker(
                timerMock.Object,
                serviceScopeFactoryMock.Object,
                destinoRepositoryMock.Object,
                eventosServiceMock.Object,
                configurationMock.Object
            );

            await worker.TestDoWorkAsync(workerContext);

            destinoRepositoryMock.Verify(r => r.GetListAsync(false, default), Times.Once);
            eventosServiceMock.Verify(s => s.ObtenerEventosPorCoordenadasAsync(10, 20), Times.Once);
            eventoRepositoryMock.Verify(r => r.InsertAsync(It.Is<Evento>(e => e.Nombre == "Evento Test" && e.DestinoTuristicoId == destinoId), false, default), Times.Once);
            destinoRepositoryMock.Verify(r => r.UpdateAsync(It.Is<DestinoEntidad>(d => d.Id == destinoId), false, default), Times.Once);
        }

        [Fact]
        public async Task DoWorkAsync_EjecutarDosCiclos_DebeActualizarYNoDuplicarEventos()
        {
            var timerMock = new Mock<AbpAsyncTimer>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var configurationMock = new Mock<IConfiguration>();
            
            var destinoRepositoryMock = new Mock<IRepository<DestinoEntidad, Guid>>();
            var eventosServiceMock = new Mock<IEventosService>();
            var eventoRepositoryMock = new Mock<IRepository<Evento, Guid>>();

            var configSectionMock = new Mock<IConfigurationSection>();
            configSectionMock.Setup(c => c.Value).Returns("86400000");
            configurationMock.Setup(c => c.GetSection("TicketMaster:WorkerPeriodMs")).Returns(configSectionMock.Object);

            var destinoId = Guid.NewGuid();
            var destinosFalsos = new List<DestinoEntidad>
            {
                new DestinoEntidad(destinoId)
                {
                    nombre = "Ciudad Gótica",
                    foto = "test.jpg",
                    Coordenadas = new WayFinder.DestinosTuristicos.Coordenadas(10, 20)
                }
            };
            
            destinoRepositoryMock.Setup(r => r.GetListAsync(false, default)).ReturnsAsync(destinosFalsos);

            var eventosTicketMaster = new WayFinder.DestinosTuristicosDTOs.BuscarEventosResultDto
            {
                Eventos = new List<WayFinder.DestinosTuristicosDTOs.Eventos.EventoDto>
                {
                    new WayFinder.DestinosTuristicosDTOs.Eventos.EventoDto { IdExterno = "Ext1", Nombre = "Evento Test Original", UrlTicket = "http://test.com", FechaInicio = DateTime.Now, ImagenUrl = "img.jpg", Lugar = "Estadio" }
                }
            };
            
            eventosServiceMock
                .Setup(s => s.ObtenerEventosPorCoordenadasAsync(10, 20))
                .ReturnsAsync(eventosTicketMaster);

            Evento eventoGuardado = null;
            eventoRepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Evento, bool>>>(), true, default))
                .ReturnsAsync(() => eventoGuardado);

            eventoRepositoryMock
                .Setup(r => r.InsertAsync(It.IsAny<Evento>(), false, default))
                .Callback<Evento, bool, System.Threading.CancellationToken>((e, b, c) => eventoGuardado = e)
                .ReturnsAsync((Evento e, bool b, System.Threading.CancellationToken c) => e);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(IRepository<DestinoEntidad, Guid>))).Returns(destinoRepositoryMock.Object);
            serviceProviderMock.Setup(sp => sp.GetService(typeof(IEventosService))).Returns(eventosServiceMock.Object);
            serviceProviderMock.Setup(sp => sp.GetService(typeof(IRepository<Evento, Guid>))).Returns(eventoRepositoryMock.Object);

            var workerContext = new PeriodicBackgroundWorkerContext(serviceProviderMock.Object);

            var worker = new TestableTicketMasterEventosWorker(
                timerMock.Object,
                serviceScopeFactoryMock.Object,
                destinoRepositoryMock.Object,
                eventosServiceMock.Object,
                configurationMock.Object
            );

            // CICLO 1: Inserción inicial
            await worker.TestDoWorkAsync(workerContext);

            // CICLO 2: Sincronización periódica con evento actualizado en TicketMaster
            eventosTicketMaster.Eventos[0].Nombre = "Evento Test Actualizado";
            await worker.TestDoWorkAsync(workerContext);

            // Verificaciones: InsertAsync se debió llamar EXACTAMENTE 1 vez en total (en el primer ciclo)
            eventoRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Evento>(), false, default), Times.Once);

            // UpdateAsync se debió llamar en el segundo ciclo para actualizar el evento existente
            eventoRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Evento>(e => e.Nombre == "Evento Test Actualizado"), false, default), Times.Once);
        }
    }
}
