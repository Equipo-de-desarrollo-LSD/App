using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WayFinder.DestinosTuristicosDTOs;
using WayFinder.DestinosTuristicosDTOs.Eventos;
using System.Globalization;

namespace WayFinder.DestinosTuristicos
{
    public class TicketMasterEventosService : IEventosService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        // Inyectamos HttpClient para las peticiones y Configuration para leer el appsettings.json
        public TicketMasterEventosService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<BuscarEventosResultDto> ObtenerEventosPorCoordenadasAsync(double latitud, double longitud)
        {
            try
            {
                // Inicializamos el DTO con la lista vacía para evitar nulos
                var result = new BuscarEventosResultDto { Eventos = new List<EventoDto>() };

                // 1. Leemos las credenciales y le agregamos .Trim()
                var apiKey = _configuration["TicketMaster:ApiKey"]?.Trim();
                var baseUrl = _configuration["TicketMaster:BaseUrl"]?.Trim();

                // Validamos que la baseUrl termine con "/"
                if (!string.IsNullOrEmpty(baseUrl) && !baseUrl.EndsWith("/"))
                {
                    baseUrl += "/";
                }

                // 2. Coordenadas con punto decimal y limpias
                string latStr = latitud.ToString(CultureInfo.InvariantCulture).Trim();
                string lonStr = longitud.ToString(CultureInfo.InvariantCulture).Trim();

                // 3. Armamos la URL esencial
                var url = $"{baseUrl}events.json?apikey={apiKey}&latlong={latStr},{lonStr}";

                // 4. Hacemos la petición
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorDeApi = await response.Content.ReadAsStringAsync();
                    throw new Exception($"HTTP {response.StatusCode} - {errorDeApi}");
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(jsonString);
                var embedded = json["_embedded"];

                if (embedded != null && embedded["events"] != null)
                {
                    foreach (var eventoJson in embedded["events"])
                    {
                        var evento = new EventoDto
                        {
                            IdExterno = eventoJson["id"]?.ToString(),
                            Nombre = eventoJson["name"]?.ToString(),
                            UrlTicket = eventoJson["url"]?.ToString()
                        };

                        var fechaStr = eventoJson["dates"]?["start"]?["dateTime"]?.ToString();
                        if (DateTime.TryParse(fechaStr, out DateTime fechaParsed))
                        {
                            evento.FechaInicio = fechaParsed;
                        }

                        var primerImagen = eventoJson["images"]?.FirstOrDefault();
                        if (primerImagen != null)
                        {
                            evento.ImagenUrl = primerImagen["url"]?.ToString();
                        }

                        var primerLugar = eventoJson["_embedded"]?["venues"]?.FirstOrDefault();
                        if (primerLugar != null)
                        {
                            evento.Lugar = primerLugar["name"]?.ToString();
                        }

                        result.Eventos.Add(evento);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                // ESTO ES LO CLAVE: Atrapamos cualquier error interno de C# y lo mostramos en Swagger
                throw new Volo.Abp.UserFriendlyException($"CRASH INTERNO EN C#: {ex.Message} | Stack: {ex.StackTrace}");
            }
        }
    }
}
