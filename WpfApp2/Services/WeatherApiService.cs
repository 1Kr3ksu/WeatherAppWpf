
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using WpfApp2.Models;

namespace WpfApp2.Services;

public sealed class WeatherApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public WeatherApiService(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<WeatherResponse> GetCurrentWeatherAsync(
        string city,
        CancellationToken cancellationToken = default)
    {
        ValidateInput(city);

        var url =
            "https://api.weatherapi.com/v1/current.json" +
            $"?key={Uri.EscapeDataString(_apiKey)}" +
            $"&q={Uri.EscapeDataString(city)}&lang=pl";

        using var response = await _httpClient.GetAsync(
            url, cancellationToken);

        var json = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"WeatherAPI zwróciło błąd {(int)response.StatusCode}: {json}");
        }

        return JsonSerializer.Deserialize<WeatherResponse>(
                   json, JsonOptions)
               ?? throw new InvalidOperationException(
                   "WeatherAPI zwróciło niepoprawną odpowiedź.");
    }

    public async Task<List<ForecastDay>> GetForecastAsync(
        string city,
        CancellationToken cancellationToken = default)
    {
        ValidateInput(city);

        var url =
            "https://api.weatherapi.com/v1/forecast.json" +
            $"?key={Uri.EscapeDataString(_apiKey)}" +
            $"&q={Uri.EscapeDataString(city)}" +
            "&days=3&lang=pl";

        using var response = await _httpClient.GetAsync(
            url, cancellationToken);

        var json = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"WeatherAPI: błąd {(int)response.StatusCode}: {json}");
        }

        var data = JsonSerializer.Deserialize<ForecastApiResponse>(
            json, JsonOptions);

        var forecastDays = data?.Forecast?.ForecastDays;


        if (forecastDays is null || forecastDays.Count == 0)
        {
            throw new InvalidOperationException(
                $"Nie znaleziono prognozy w odpowiedzi API. Odpowiedź: {json}");
        }

        return forecastDays.Select(day => new ForecastDay
        {
            Date = DateTime.Parse(
                day.Date,
                CultureInfo.InvariantCulture),

            MaxTempC = day.Day.MaxTempC,
            MinTempC = day.Day.MinTempC,
            Condition = day.Day.Condition.Text,
            Icon = day.Day.Condition.Icon.StartsWith("//")
                ? "https:" + day.Day.Condition.Icon
                : day.Day.Condition.Icon,
            ChanceOfRain = day.Day.ChanceOfRain,
            MaxWindKph = day.Day.MaxWindKph
        }).ToList();
    }

    private void ValidateInput(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException(
                "Nazwa miasta nie może być pusta.",
                nameof(city));
        }

        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            throw new InvalidOperationException(
                "Brak klucza API WeatherAPI.");
        }
    }

    private sealed class ForecastApiResponse
    {
        [JsonPropertyName("forecast")]
        public ForecastContainer? Forecast { get; set; }
    }

    private sealed class ForecastContainer
    {
        [JsonPropertyName("forecastday")]
        public List<ForecastDayResponse>? ForecastDays { get; set; }
    }

    private sealed class ForecastDayResponse
    {
        [JsonPropertyName("date")]
        public string Date { get; set; } = "";

        [JsonPropertyName("day")]
        public ForecastDayDetails Day { get; set; } = new();
    }

    private sealed class ForecastDayDetails
    {
        [JsonPropertyName("maxtemp_c")]
        public double MaxTempC { get; set; }

        [JsonPropertyName("mintemp_c")]
        public double MinTempC { get; set; }

        [JsonPropertyName("daily_chance_of_rain")]
        public int ChanceOfRain { get; set; }

        [JsonPropertyName("maxwind_kph")]
        public double MaxWindKph { get; set; }

        [JsonPropertyName("condition")]
        public ForecastCondition Condition { get; set; } = new();
    }

    private sealed class ForecastCondition
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = "";

        [JsonPropertyName("icon")]
        public string Icon { get; set; } = "";
    }
}
