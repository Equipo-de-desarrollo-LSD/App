using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WayFinder.DestinosTuristicos;
using Xunit;

namespace WayFinder.Eventos
{
    public class TicketMasterEventosServiceTests
    {
        [Fact]
        public async Task ObtenerEventos_ShouldFormatCoordinatesWithDots()
        {
            // ==========================================
            // 1. ARRANGE (Preparar el escenario)
            // ==========================================

            // Simular un JSON de respuesta vacío pero exitoso
            var jsonRespuestaMock = "{ \"_embedded\": { \"events\": [] } }";

            // Mockear el motor del HttpClient para que intercepte la llamada
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(jsonRespuestaMock),
               })
               .Verifiable(); // Indicamos que después queremos verificar que esto realmente se ejecutó

            // Crear el HttpClient falso inyectándole nuestro motor mockeado
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new System.Uri("https://app.ticketmaster.com/")
            };

            // Mockear IConfiguration si tu servicio lee la API Key de ahí
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(x => x["TicketMaster:ApiKey"]).Returns("API_KEY_FALSA");

            // Instanciar TU servicio pasándole los fakes
            // (Ajustá los parámetros según lo que pida tu constructor real)
            var servicio = new TicketMasterEventosService(httpClient, configMock.Object);

            // ==========================================
            // 2. ACT (Ejecutar el método a probar)
            // ==========================================

            // Le pasamos coordenadas de Concepción del Uruguay (-32.48, -58.23)
            double latitud = -32.48;
            double longitud = -58.23;

            await servicio.ObtenerEventosPorCoordenadasAsync(latitud, longitud);

            // ==========================================
            // 3. ASSERT (Verificar que pasó lo esperado)
            // ==========================================

            // Le pedimos a nuestro Mock que verifique cómo fue la solicitud HTTP que intentó salir
            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1), // Se tuvo que llamar exactamente 1 vez
               ItExpr.Is<HttpRequestMessage>(req =>
                   // ¡EL CORAZÓN DEL TEST! Verificamos que la URL contenga puntos y no comas
                   req.RequestUri.ToString().Contains("latlong=-32.48,-58.23")
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task ObtenerEventos_ShouldMapEventsCorrectly()
        {
            // ==========================================
            // 1. ARRANGE (Preparar el escenario)
            // ==========================================

            // JSON simplificado que imita la estructura real de TicketMaster
            var jsonExitoso = @"
    {
        ""_embedded"": {
            ""events"": [
                {
                    ""id"": ""evt-123"",
                    ""name"": ""Recital de Rock"",
                    ""url"": ""https://ticketmaster.com/evento/123"",
                    ""dates"": {
                        ""start"": {
                            ""localDate"": ""2026-10-15""
                        }
                    }
                }
            ]
        }
    }";

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonExitoso)
                });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new System.Uri("https://app.ticketmaster.com/")
            };

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(x => x["TicketMaster:ApiKey"]).Returns("API_KEY_FALSA");

            var servicio = new TicketMasterEventosService(httpClient, configMock.Object);

            // ==========================================
            // 2. ACT (Ejecutar)
            // ==========================================

            // IMPORTANTE: Usá el nombre exacto de tu método acá
            var resultado = await servicio.ObtenerEventosPorCoordenadasAsync(-32.48, -58.23);

            // ==========================================
            // 3. ASSERT (Verificar)
            // ==========================================

            Assert.NotNull(resultado);

            // Tenés que acceder a la propiedad que contiene los eventos dentro de tu DTO
            Assert.Single(resultado.Eventos); 
            // Y para extraer el evento:
            var evento = resultado.Eventos[0];

            Assert.Equal("Recital de Rock", evento.Nombre);
            Assert.Equal("https://ticketmaster.com/evento/123", evento.UrlTicket);
        }

        [Fact]
        public async Task ObtenerEventos_ShouldThrowException()
        {
            // ==========================================
            // 1. ARRANGE (Preparar el escenario)
            // ==========================================

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    // ACÁ ESTÁ LA MAGIA: Simulamos que TicketMaster se cayó y devuelve un error 500
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Content = new StringContent("Internal Server Error")
                });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new System.Uri("https://app.ticketmaster.com/")
            };

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(x => x["TicketMaster:ApiKey"]).Returns("API_KEY_FALSA");

            var servicio = new TicketMasterEventosService(httpClient, configMock.Object);

            // ==========================================
            // 2 & 3. ACT y ASSERT (Ejecutar y Verificar)
            // ==========================================

            // Le decimos a xUnit: "Ejecutá esto y verificá que devuelva una UserFriendlyException"
            var excepcion = await Assert.ThrowsAsync<Volo.Abp.UserFriendlyException>(() =>
                servicio.ObtenerEventosPorCoordenadasAsync(-32.48, -58.23)
            );

            // (Opcional) Podés verificar que el mensaje de error sea el que vos escribiste en el servicio
            Assert.NotNull(excepcion);
            // Assert.Contains("TicketMaster", excepcion.Message); // Descomentá esto si querés probar parte del texto del error
        }
    }
}
