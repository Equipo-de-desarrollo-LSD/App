using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NSubstitute; // Librería para crear objetos falsos (Mocks)
using Xunit;       // Librería de pruebas
using WayFinder.DestinosTuristicos;
using WayFinder.DestinosTuristicosDTOs;
using WayFinder.Application.Tests; // Para poder usar el FakeHttpMessageHandler que creamos en el paso 1

namespace WayFinder.Application.Tests.DestinosTuristicos
{
    public class GeoDbBuscarCiudadService_Tests
    {
        [Fact]
        public async Task FiltrarCiudadesExternasAsync_SiApiRespondeBien_DebeRetornarListaDeCiudades()
        {
            // 1. ARRANGE (Preparar)
            // Simulamos el JSON que devolvería la API real
            var jsonRespuesta = @"
            {
                ""data"": [
                    {
                        ""name"": ""Buenos Aires"",
                        ""country"": ""Argentina"",
                        ""latitude"": -34.60,
                        ""longitude"": -58.38,
                        ""population"": 3000000
                    }
                ]
            }";

            // Preparamos la respuesta HTTP 200 OK con ese JSON
            var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonRespuesta)
            };

            // Creamos el cliente HTTP falso usando nuestra herramienta del Paso 1
            var fakeHandler = new FakeHttpMessageHandler(fakeResponse);
            var httpClient = new HttpClient(fakeHandler);

            // Simulamos la Fábrica (Factory) para que entregue nuestro cliente falso
            var mockFactory = Substitute.For<IHttpClientFactory>();
            mockFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

            // Instanciamos TU servicio con las piezas falsas
            var servicio = new GeoDbBuscarCiudadService(httpClient, mockFactory);

            // 2. ACT (Ejecutar)
            var request = new FiltrarCiudadesRequestDto
            {
                PaisCodigo = "AR",
                MinPoblacion = 100000,
                Limit = 5
            };
            var resultado = await servicio.FiltrarCiudadesExternasAsync(request);

            // 3. ASSERT (Verificar)
            Assert.NotNull(resultado);
            Assert.Single(resultado.Ciudades); // Debe haber 1 ciudad

            var ciudad = resultado.Ciudades[0];
            Assert.Equal("Buenos Aires", ciudad.Nombre);
            Assert.Equal("Argentina", ciudad.Pais);
            Assert.Equal(3000000, ciudad.PaisPoblacion);
        }

        [Fact]
        public async Task FiltrarCiudadesExternasAsync_SiApiFalla_DebeRetornarListaVacia()
        {
            // 1. ARRANGE
            // Simulamos Error 500 (Servidor caído)
            var fakeResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            var fakeHandler = new FakeHttpMessageHandler(fakeResponse);
            var httpClient = new HttpClient(fakeHandler);

            var mockFactory = Substitute.For<IHttpClientFactory>();
            mockFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

            var servicio = new GeoDbBuscarCiudadService(httpClient, mockFactory);

            // 2. ACT
            var resultado = await servicio.FiltrarCiudadesExternasAsync(new FiltrarCiudadesRequestDto());

            // 3. ASSERT
            Assert.NotNull(resultado);
            Assert.Empty(resultado.Ciudades); // No debe explotar, solo devolver 0 resultados
        }

        [Fact]
        public async Task FiltrarCiudadesExternasAsync_SiJsonTieneNulls_DebeUsarValoresPorDefecto()
        {
            // 1. ARRANGE
            // JSON "sucio" sin números
            var jsonIncompleto = @"
            {
                ""data"": [
                    {
                        ""name"": ""Ciudad Fantasma"",
                        ""country"": ""Desconocido"",
                        ""latitude"": null,
                        ""longitude"": null,
                        ""population"": null
                    }
                ]
            }";

            var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonIncompleto)
            };

            var fakeHandler = new FakeHttpMessageHandler(fakeResponse);
            var httpClient = new HttpClient(fakeHandler);

            var mockFactory = Substitute.For<IHttpClientFactory>();
            mockFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

            var servicio = new GeoDbBuscarCiudadService(httpClient, mockFactory);

            // 2. ACT
            var resultado = await servicio.FiltrarCiudadesExternasAsync(new FiltrarCiudadesRequestDto());

            // 3. ASSERT
            Assert.Single(resultado.Ciudades);
            var ciudad = resultado.Ciudades[0];

            // Verificamos que tu lógica '?? 0' funcionó
            Assert.Equal(0, ciudad.Latitud);
            Assert.Equal(0, ciudad.PaisPoblacion);
        }
    }
}