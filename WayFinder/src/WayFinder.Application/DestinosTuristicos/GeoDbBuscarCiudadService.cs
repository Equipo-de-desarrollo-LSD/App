using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WayFinder.DestinosTuristicosDTOs;

namespace WayFinder.DestinosTuristicos
{
    public class GeoDbBuscarCiudadService : IBuscarCiudadService
    {
        private const string ApiKey = "99e8cc679dmsh984e4eac00f43f7p1cdaabjsnb5ae10c85bc6";
        private const string BaseUrl = "https://wft-geo-db.p.rapidapi.com/v1/geo";
        private const string Host = "wft-geo-db.p.rapidapi.com";

        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRepository<MetricaApi, Guid> _metricaRepository;

        public GeoDbBuscarCiudadService(
            HttpClient httpClient,
            IHttpClientFactory httpClientFactory,
            IRepository<MetricaApi, Guid> metricaRepository)
        {
            _httpClient = httpClient;
            _httpClientFactory = httpClientFactory;
            _metricaRepository = metricaRepository;
        }

        public async Task<BuscarCiudadResultDto> SearchCitiesAsync(BuscarCiudadRequestDto request)
        {
            var result = new BuscarCiudadResultDto();
            if (string.IsNullOrWhiteSpace(request?.NombreCiudad))
                return result;

            var url = $"{BaseUrl}/cities?namePrefix={Uri.EscapeDataString(request.NombreCiudad)}&limit=5";
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequest.Headers.Add("X-RapidAPI-Key", ApiKey);
            httpRequest.Headers.Add("X-RapidAPI-Host", Host);

            var stopwatch = Stopwatch.StartNew();
            var statusCode = 0;

            try
            {
                var response = await _httpClient.SendAsync(httpRequest);
                statusCode = (int)response.StatusCode;

                if (!response.IsSuccessStatusCode)
                    return result;

                var json = await response.Content.ReadAsStringAsync();
                var geoDbResponse = System.Text.Json.JsonSerializer.Deserialize<GeoDbCitiesResponse>(json);
                if (geoDbResponse?.Data != null)
                {
                    foreach (var city in geoDbResponse.Data)
                    {
                        result.Ciudades.Add(new CiudadDto
                        {
                            Id = city.Id, // <--- 1. NUEVO: Agregamos esto para guardar el ID
                            Nombre = city.Name,
                            Pais = city.Country,
                            Latitud = city.Latitude ?? 0,
                            Longitud = city.Longitude ?? 0,
                        });
                    }
                }
            }
            catch (Exception)
            {
                statusCode = 500;
            }
            finally
            {
                stopwatch.Stop();
                await _metricaRepository.InsertAsync(new MetricaApi(
                    Guid.NewGuid(),
                    "GeoDB",
                    "/v1/geo/cities",
                    statusCode,
                    stopwatch.ElapsedMilliseconds
                ));
            }
            return result;
        }

        public async Task<FiltrarCiudadesResultDto> FiltrarCiudadesExternasAsync(FiltrarCiudadesRequestDto input)
        {
            // ... (Este método déjalo igual que como lo tenías) ...
            var resultado = new FiltrarCiudadesResultDto();
            var client = _httpClientFactory.CreateClient();
            var url = "http://geodb-free-service.wirefreethought.com/v1/geo/cities?";

            var parameters = new List<string>();
            if (!string.IsNullOrEmpty(input.PaisCodigo)) parameters.Add($"countryIds={input.PaisCodigo}");
            if (input.MinPoblacion.HasValue) parameters.Add($"minPopulation={input.MinPoblacion}");
            parameters.Add($"limit={input.Limit}");
            parameters.Add("sort=-population");
            parameters.Add("offset=0");
            parameters.Add("hateoasMode=false");

            var response = await client.GetAsync(url + string.Join("&", parameters));

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(jsonString);
                var data = json["data"];

                if (data != null)
                {
                    foreach (var item in data)
                    {
                        resultado.Ciudades.Add(new CiudadDto
                        {
                            Nombre = item["name"]?.ToString(),
                            Pais = item["country"]?.ToString(),
                            Latitud = (double?)item["latitude"] ?? 0,
                            Longitud = (double?)item["longitude"] ?? 0,
                            PaisPoblacion = (double?)item["population"] ?? 0
                        });
                    }
                }
            }
            return resultado;
        }

        // --- 2. NUEVO: AQUI AGREGAMOS EL MÉTODO DEL PUNTO 3.3 ---
        public async Task<DetalleCiudadDto> ObtenerDetalleCiudadAsync(int cityId)
        {
            var url = $"{BaseUrl}/cities/{cityId}";

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequest.Headers.Add("X-RapidAPI-Key", ApiKey);
            httpRequest.Headers.Add("X-RapidAPI-Host", Host);

            try
            {
                var response = await _httpClient.SendAsync(httpRequest);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(jsonString);
                    var data = json["data"];

                    if (data != null)
                    {
                        return new DetalleCiudadDto
                        {
                            GeoDbId = data["id"]?.ToString(),
                            WikiDataId = data["wikiDataId"]?.ToString(),

                            Nombre = data["name"]?.ToString(),
                            Pais = data["country"]?.ToString(),
                            Region = data["region"]?.ToString(),

                            Poblacion = (int)(data["population"] ?? 0),
                            ElevacionMetros = (double?)data["elevationMeters"],
                            ZonaHoraria = data["timezone"]?.ToString(),

                            Coordenadas = new CoordenadasDto
                            {
                                latitud = (double)(data["latitude"] ?? 0),
                                longitud = (double)(data["longitude"] ?? 0)
                            }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo detalles: {ex.Message}");
            }

            return null;
        }
        // ---------------------------------------------------------

        private class GeoDbCitiesResponse
        {
            [JsonPropertyName("data")]
            public List<GeoDbCity> Data { get; set; }
        }

        private class GeoDbCity
        {
            // --- 3. NUEVO: Agregamos ID para poder leerlo ---
            [JsonPropertyName("id")]
            public int Id { get; set; }
            // -----------------------------------------------

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("country")]
            public string Country { get; set; }

            [JsonPropertyName("latitude")]
            public double? Latitude { get; set; }

            [JsonPropertyName("longitude")]
            public double? Longitude { get; set; }
        }
    }
}