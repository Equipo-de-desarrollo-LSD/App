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
        private readonly IHttpClientFactory _httpClientFactory; // De tu rama 3.2
        private readonly IRepository<MetricaApi, Guid> _metricaRepository; // De rama Main

        // Constructor fusionado: Recibe las 3 cosas
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

            // Iniciamos el cronómetro para la métrica (Código de Main)
            var stopwatch = Stopwatch.StartNew();
            var statusCode = 0;

            try
            {
                var response = await _httpClient.SendAsync(httpRequest);
                statusCode = (int)response.StatusCode; // Capturamos status real

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
                // Guardamos métrica (Código de Main)
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

        // --- AQUI AGREGAMOS TU MÉTODO DE LA RAMA 3.2 ---
        public async Task<FiltrarCiudadesResultDto> FiltrarCiudadesExternasAsync(FiltrarCiudadesRequestDto input)
        {
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

        // Clases privadas para deserializar la respuesta de GeoDB (Fusionadas)
        private class GeoDbCitiesResponse
        {
            [JsonPropertyName("data")]
            public List<GeoDbCity> Data { get; set; }
        }

        private class GeoDbCity
        {
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