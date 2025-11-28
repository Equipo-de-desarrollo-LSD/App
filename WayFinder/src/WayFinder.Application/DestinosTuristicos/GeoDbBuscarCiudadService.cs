using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WayFinder.DestinosTuristicosDTOs;
using System.Text.Json.Serialization;

namespace WayFinder.DestinosTuristicos
{
    public class GeoDbBuscarCiudadService : IBuscarCiudadService
    {
        private const string ApiKey = "99e8cc679dmsh984e4eac00f43f7p1cdaabjsnb5ae10c85bc6";
        private const string BaseUrl = "https://wft-geo-db.p.rapidapi.com/v1/geo";
        private const string Host = "wft-geo-db.p.rapidapi.com";
        private readonly HttpClient _httpClient;

        public GeoDbBuscarCiudadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
                            Latitud = city.Latitude ?? 0,
                            Longitud = city.Longitude ?? 0,
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

            [JsonPropertyName("latitude")]
            public double? Latitude { get; set; }

            [JsonPropertyName("longitude")]
            public double? Longitude { get; set; }


            //[JsonPropertyName("region")]
            //public string Region { get; set; }
        }
    }
}
    
    