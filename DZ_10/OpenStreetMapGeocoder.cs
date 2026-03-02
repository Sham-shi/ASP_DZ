using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace DZ_10
{
    public interface IGeocoderService
    {
        Task<GeoLocation?> GetCoordinatesAsync(string city);
    }

    public class GeoLocation
    {
        public string City { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Country { get; set; } = string.Empty;
        public string? State { get; set; }
    }

    public class OpenStreetMapGeocoder : IGeocoderService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenStreetMapGeocoder> _logger;

        public OpenStreetMapGeocoder(HttpClient httpClient, ILogger<OpenStreetMapGeocoder> logger)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("MyGeoApp/1.0 (your-email@example.com)");
            _logger = logger;
        }

        public async Task<GeoLocation?> GetCoordinatesAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return null;

            try
            {
                var encodedCity = Uri.EscapeDataString(city);
                // ✅ Важно: добавляем параметр `format=json` и `limit=1`
                var url = $"search?q={encodedCity}&format=json&limit=1";

                var response = await _httpClient.GetStringAsync(url);
                var locations = JsonSerializer.Deserialize<List<GeocodeResponse>>(response);

                if (locations == null || locations.Count == 0)
                {
                    _logger.LogWarning("Не найдены координаты для города: {City}", city);
                    return null;
                }

                var location = locations[0];

                return new GeoLocation
                {
                    City = location.DisplayName.Split(',')[0].Trim(),
                    Latitude = double.Parse(location.Lat),
                    Longitude = double.Parse(location.Lon),
                    Country = location.Address?.Country ?? "Неизвестно",
                    State = location.Address?.State
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при геокодировании города: {City}", city);
                return null;
            }
        }

        private class GeocodeResponse
        {
            [JsonPropertyName("display_name")]
            public string DisplayName { get; set; } = string.Empty;

            [JsonPropertyName("lat")]
            public string Lat { get; set; } = string.Empty;

            [JsonPropertyName("lon")]
            public string Lon { get; set; } = string.Empty;

            [JsonPropertyName("address")]
            public AddressDetails? Address { get; set; }
        }

        private class AddressDetails
        {
            [JsonPropertyName("country")]
            public string? Country { get; set; }

            [JsonPropertyName("state")]
            public string? State { get; set; }
        }
    }
}