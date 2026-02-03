using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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

        public GeoDbBuscarCiudadService(HttpClient httpClient, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClient;
            _httpClientFactory = httpClientFactory;
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

            try
            {
                var response = await _httpClient.SendAsync(httpRequest);
                if (!response.IsSuccessStatusCode)
                    return result;

                var json = await response.Content.ReadAsStringAsync();
                var geoDbResponse = JsonSerializer.Deserialize<GeoDbCitiesResponse>(json);
                if (geoDbResponse?.Data != null)
                {
                    foreach (var city in geoDbResponse.Data)
                    {
                        result.Ciudades.Add(new CiudadDto
                        {
                            Nombre = city.Name,
                            Pais = city.Country,
                        });
                    }
                }
            }
            catch
            {
                // Manejo de error: retorna lista vacía
            }
            return result;
        }

        private class GeoDbCitiesResponse
        {
            [JsonPropertyName("data")]
            public List<GeoDbCity> Data { get; set; }
        }

        private class GeoDbCity
        {
            //[JsonPropertyName("id")]
            //public int Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("country")]
            public string Country { get; set; }

            //[JsonPropertyName("region")]
            //public string Region { get; set; }
        }
        // Asegúrate de tener los usings necesarios (Newtonsoft, System.Net.Http, etc)
        public async Task<FiltrarCiudadesResultDto> FiltrarCiudadesExternasAsync(FiltrarCiudadesRequestDto input)
        {
            var resultado = new FiltrarCiudadesResultDto();

            // Aquí movemos toda la lógica de conexión que antes pusimos en el AppService
            var client = _httpClientFactory.CreateClient(); // O como lo estés instanciando ahí
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
                        // Mapeamos a CiudadDto y lo metemos en el resultado
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
    }
}
    
    /*
            // ⚠️ Advertencia: Revisa la nota al final sobre esta forma de usar HttpClient
            using (HttpClient client = new HttpClient())
            {
                // Configuración de Headers
                client.DefaultRequestHeaders.Add("X-RapidAPI-Key", apiKey);
                client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "wft-geo-db.p.rapidapi.com");

                // Endpoint con query params
               // string url = $"{baseUrl}/cities?namePrefix={Uri.EscapeDataString(request.NombreCiudad)}&limit=5";

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                { 

                    // 1. Leemos el resultado como un string de texto (JSON)
                    string jsonResult = await response.Content.ReadAsStringAsync();

                    var cityDtos = new List<CiudadDto>();

                    // 2. Parseamos el string JSON
                    using (JsonDocument doc = JsonDocument.Parse(jsonResult))
                    {
                        JsonElement root = doc.RootElement;

                        // 3. Buscamos la propiedad "data" que contiene el array de ciudades
                        if (root.TryGetProperty("data", out JsonElement dataArray))
                        {
                            // 4. Iteramos sobre cada elemento (ciudad) en el array "data"
                            foreach (JsonElement cityData in dataArray.EnumerateArray())
                            {
                                // 5. Extraemos las propiedades de cada ciudad y creamos el DTO
                                var dto = new CiudadDto
                                {
                                    // Usamos TryGetProperty para evitar errores si un campo no viene
                                    Nombre = cityData.TryGetProperty("city", out var city) ? city.GetString() : null,
                                    Pais = cityData.TryGetProperty("country", out var country) ? country.GetString() : null,
                                    // ... puedes añadir más propiedades aquí ...
                                    // Id = cityData.TryGetProperty("id", out var id) ? id.GetInt32() : 0,
                                    // Region = cityData.TryGetProperty("region", out var region) ? region.GetString() : null,
                                };
                                cityDtos.Add(dto);
                            }
                        }
                    }

                    // 6. Retornamos la lista de DTOs
                    return new BuscarCiudadResultDto { Ciudades = cityDtos };

                    // --- FIN DE CAMBIOS ---
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }

            // Si hubo un error o no se pudo completar, devolvemos una lista vacía
            return new BuscarCiudadResultDto { Ciudades = new List<CiudadDto>() };
        }
    }
}
   */
