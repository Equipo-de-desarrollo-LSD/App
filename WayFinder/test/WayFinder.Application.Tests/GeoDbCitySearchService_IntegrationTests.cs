using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WayFinder.Calificacion;
using WayFinder.DestinosTuristicos;
using WayFinder.DestinosTuristicosDTOs;
using WayFinder.DestinoTuristico;
using WayFinder.Metricas;
using Xunit;
using WayFinder.Admin;

namespace WayFinder
{
    public class GeoDbCitySearchService_IntegrationTests : WayFinder.WayFinderTestBase<WayFinderApplicationTestModule>
    {
        private readonly IRepository<MetricaApi, Guid> _metricaRepository;

        public GeoDbCitySearchService_IntegrationTests()
        {
            // Obtenemos el servicio de métricas real del contenedor de pruebas
            _metricaRepository = GetRequiredService<IRepository<MetricaApi, Guid>>();
        }

        private class FailingHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                throw new HttpRequestException("Simulated network error");
            }
        }

        private GeoDbBuscarCiudadService CreateService()
        {
            var httpClient = new HttpClient();
            
            // Creamos un mock del Factory (necesario por tu rama 3.2)
            var mockFactory = Substitute.For<IHttpClientFactory>();

            // Pasamos los 3 parámetros: HttpClient, Factory (Mock) y Repositorio (Real)
            return new GeoDbBuscarCiudadService(httpClient, mockFactory, _metricaRepository);
        }
        
        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task SearchCitiesAsync_ReturnsResults_ForValidPartialName()
        {
            var service = CreateService();
            var request = new BuscarCiudadRequestDto { NombreCiudad = "Río" };
            var result = await service.SearchCitiesAsync(request);
            Assert.NotNull(result);
            Assert.NotEmpty(result.Ciudades);
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task SearchCitiesAsync_ReturnsEmpty_ForNoMatch()
        {
            var service = CreateService();
            var request = new BuscarCiudadRequestDto { NombreCiudad = "zzzzzzzzzz" };
            var result = await service.SearchCitiesAsync(request);
            Assert.NotNull(result);
            Assert.Empty(result.Ciudades);
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task SearchCitiesAsync_ReturnsEmpty_ForInvalidInput()
        {
            var service = CreateService();
            var request = new BuscarCiudadRequestDto { NombreCiudad = "" };
            var result = await service.SearchCitiesAsync(request);
            Assert.NotNull(result);
            Assert.Empty(result.Ciudades);
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task SearchCitiesAsync_HandlesNetworkError()
        {
            // Simula error de red usando un HttpClient con un handler que lanza excepción
            var httpClient = new HttpClient(new FailingHandler());
            
            // También aquí necesitamos el mock del factory para cumplir con el constructor
            var mockFactory = Substitute.For<IHttpClientFactory>();

            var service = new GeoDbBuscarCiudadService(httpClient, mockFactory, _metricaRepository);
            
            var request = new BuscarCiudadRequestDto { NombreCiudad = "Rio" };
            var result = await service.SearchCitiesAsync(request);
            Assert.NotNull(result);
            Assert.Empty(result.Ciudades);
        }
    }
}