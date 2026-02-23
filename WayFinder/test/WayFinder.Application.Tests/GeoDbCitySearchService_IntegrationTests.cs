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
            var mockFactory = Substitute.For<IHttpClientFactory>();
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
            var httpClient = new HttpClient(new FailingHandler());
            var mockFactory = Substitute.For<IHttpClientFactory>();
            var service = new GeoDbBuscarCiudadService(httpClient, mockFactory, _metricaRepository);

            var request = new BuscarCiudadRequestDto { NombreCiudad = "Rio" };
            var result = await service.SearchCitiesAsync(request);
            Assert.NotNull(result);
            Assert.Empty(result.Ciudades);
        }

        // --- TEST DEL PUNTO 3.3 MODIFICADO ---
        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task ObtenerDetalleCiudadAsync_DeberiaTraerInformacionCompleta()
        {
            // 1. Arrange
            var service = CreateService();

            // Paso A: Buscamos la ciudad
            var busqueda = await service.SearchCitiesAsync(new BuscarCiudadRequestDto { NombreCiudad = "Paris" });
            var ciudad = busqueda.Ciudades.First();
            var idCiudad = ciudad.Id;

            // DEBUG: Verificamos que el ID no sea 0 antes de seguir
            Console.WriteLine($"DEBUG: ID obtenido en la búsqueda: {idCiudad}");
            Assert.True(idCiudad > 0, "El ID de la ciudad debería ser mayor a 0");

            // --- PASO CLAVE: Esperar para evitar el Rate Limit de la API gratuita ---
            Console.WriteLine("DEBUG: Esperando 3 segundos para evitar bloqueo de la API...");
            await Task.Delay(3000);
            // -----------------------------------------------------------------------

            // 2. Act
            var detalle = await service.ObtenerDetalleCiudadAsync(idCiudad);

            // 3. Assert
            Assert.NotNull(detalle);
            Assert.Equal(ciudad.Nombre, detalle.Nombre);
            Assert.False(string.IsNullOrEmpty(detalle.ZonaHoraria));
            Assert.True(detalle.Poblacion > 0);
            Assert.NotNull(detalle.Coordenadas);

            // Verificamos que el mapeo de Coordenadas funcione (no sean 0,0 por error)
            Assert.NotEqual(0, detalle.Coordenadas.latitud);
            Assert.NotEqual(0, detalle.Coordenadas.longitud);

            System.Diagnostics.Debug.WriteLine($"Ciudad: {detalle.Nombre}, Población: {detalle.Poblacion}");
            Console.WriteLine($"DEBUG: Test finalizado con éxito para {detalle.Nombre}");
        }
    }
}