using System.Collections.Concurrent;
using System.Text;

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

    public class LocalGeocoder : IGeocoderService
    {
        // Словарь для поиска по ЛЮБОМУ названию (кириллица, латиница, альтернативы)
        private readonly ConcurrentDictionary<string, CityData> _citiesByName;
        private readonly ILogger<LocalGeocoder> _logger;
        private readonly string _dataFilePath;

        public LocalGeocoder(ILogger<LocalGeocoder> logger)
        {
            _logger = logger;
            _citiesByName = new ConcurrentDictionary<string, CityData>(StringComparer.OrdinalIgnoreCase);
            _dataFilePath = Path.Combine(AppContext.BaseDirectory, "Data", "cities15000.txt");
            LoadCities();
        }

        private void LoadCities()
        {
            if (!File.Exists(_dataFilePath))
            {
                _logger.LogError("❌ ФАЙЛ БАЗЫ ДАННЫХ НЕ НАЙДЕН: {Path}", _dataFilePath);
                _logger.LogError("Скачайте файл с: https://download.geonames.org/export/dump/cities15000.zip");
                return;
            }

            _logger.LogInformation("📁 Загрузка базы данных из: {Path}", _dataFilePath);

            try
            {
                // ВАЖНО: файл в кодировке UTF-8!
                var lines = File.ReadAllLines(_dataFilePath, Encoding.UTF8);
                int loaded = 0;
                int namesIndexed = 0;

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;

                    var parts = line.Split('\t');
                    if (parts.Length < 12) continue;

                    try
                    {
                        var city = new CityData
                        {
                            Name = parts[1].Trim(),        // Основное название
                            AsciiName = parts[2].Trim(),   // Латиница
                            Latitude = double.Parse(parts[4].Replace('.', ',')),
                            Longitude = double.Parse(parts[5].Replace('.', ',')),
                            CountryCode = parts[8].Trim(),
                            Admin1Code = parts.Length > 10 ? parts[10].Trim() : string.Empty,
                            Population = parts.Length > 14 ? long.Parse(parts[14]) : 0
                        };

                        // 1. Индексируем основное название (поле 2)
                        if (!string.IsNullOrWhiteSpace(city.Name))
                        {
                            _citiesByName[city.Name] = city;
                            namesIndexed++;
                        }

                        // 2. Индексируем латинское название (поле 3)
                        if (!string.IsNullOrWhiteSpace(city.AsciiName) &&
                            city.AsciiName != city.Name)
                        {
                            _citiesByName[city.AsciiName] = city;
                            namesIndexed++;
                        }

                        // 3. ИНДЕКСИРУЕМ АЛЬТЕРНАТИВНЫЕ НАЗВАНИЯ (поле 4) - здесь кириллица!
                        if (parts.Length > 3 && !string.IsNullOrWhiteSpace(parts[3]))
                        {
                            var altNames = parts[3].Split(',');
                            foreach (var altName in altNames)
                            {
                                var trimmed = altName.Trim();
                                if (!string.IsNullOrWhiteSpace(trimmed) &&
                                    !_citiesByName.ContainsKey(trimmed))
                                {
                                    _citiesByName[trimmed] = city;
                                    namesIndexed++;
                                }
                            }
                        }

                        loaded++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Пропущена строка: {Error}", ex.Message);
                    }
                }

                _logger.LogInformation("✅ Загружено {CityCount} городов, проиндексировано {NameCount} названий", loaded, namesIndexed);
                _logger.LogInformation("🔍 Примеры поиска: Москва={Moscow}, Казань={Kazan}, Санкт-Петербург={SPb}",
                    _citiesByName.ContainsKey("Москва") ? "✓" : "✗",
                    _citiesByName.ContainsKey("Казань") ? "✓" : "✗",
                    _citiesByName.ContainsKey("Санкт-Петербург") ? "✓" : "✗");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Ошибка при загрузке базы данных");
            }
        }

        public Task<GeoLocation?> GetCoordinatesAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return Task.FromResult<GeoLocation?>(null);

            var cityName = city.Trim();
            _logger.LogInformation("🔍 Поиск города: '{City}'", cityName);

            // Ищем ПРЯМО по введённому названию (кириллица или латиница)
            if (_citiesByName.TryGetValue(cityName, out var cityData))
            {
                _logger.LogInformation("✅ Найдено: {Name} ({Lat}, {Lon})",
                    cityData.Name, cityData.Latitude, cityData.Longitude);
                return Task.FromResult(CreateGeoLocation(cityData));
            }

            // Если не найдено — пробуем поиск без учёта регистра и лишних пробелов
            var normalized = cityName.Trim().ToLowerInvariant();
            var similar = _citiesByName.FirstOrDefault(x =>
                x.Key.Trim().ToLowerInvariant() == normalized
            );

            if (similar.Value != null)
            {
                _logger.LogInformation("✅ Найдено (нормализованный поиск): {Name}", similar.Value.Name);
                return Task.FromResult(CreateGeoLocation(similar.Value));
            }

            _logger.LogWarning("❌ Город НЕ НАЙДЕН: '{City}'", cityName);
            _logger.LogWarning("💡 Подсказка: проверьте написание. Доступные варианты для Москвы: 'Москва', 'Moscow', 'Moskva'");
            return Task.FromResult<GeoLocation?>(null);
        }

        private GeoLocation? CreateGeoLocation(CityData cityData)
        {
            return new GeoLocation
            {
                City = !string.IsNullOrWhiteSpace(cityData.Name) && cityData.Name != "null"
                    ? cityData.Name
                    : cityData.AsciiName,
                Latitude = cityData.Latitude,
                Longitude = cityData.Longitude,
                Country = GetCountryName(cityData.CountryCode),
                State = cityData.Admin1Code
            };
        }

        private string GetCountryName(string countryCode)
        {
            var countries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "RU", "Россия" },
                { "UA", "Украина" },
                { "BY", "Беларусь" },
                { "KZ", "Казахстан" },
                { "US", "США" },
                { "GB", "Великобритания" },
                { "DE", "Германия" },
                { "FR", "Франция" },
                { "CN", "Китай" },
                { "JP", "Япония" }
            };

            return countries.TryGetValue(countryCode, out var name)
                ? name
                : countryCode;
        }
    }

    internal class CityData
    {
        public string Name { get; set; } = string.Empty;        // Основное название
        public string AsciiName { get; set; } = string.Empty;   // Латиница
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string CountryCode { get; set; } = string.Empty;
        public string Admin1Code { get; set; } = string.Empty;
        public long Population { get; set; }
    }
}