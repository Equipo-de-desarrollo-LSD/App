using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WayFinder.Calificaciones;
using WayFinder.DestinosTuristicosDTOs;
using Xunit;

namespace WayFinder
{
    public class GeoDbCitySearchService_IntegrationTests
    {
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
            return new GeoDbBuscarCiudadService(httpClient);
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
            var service = new GeoDbBuscarCiudadService(httpClient);
            var request = new BuscarCiudadRequestDto { NombreCiudad = "Rio" };
            var result = await service.SearchCitiesAsync(request);
            Assert.NotNull(result);
            Assert.Empty(result.Ciudades);
        }
    }
}
