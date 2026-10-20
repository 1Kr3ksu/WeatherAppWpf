using System.Net.Http;
using System.Text.Json;
using WpfApp2.Models;

namespace WpfApp2.Services;

public sealed class WeatherApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public WeatherApiService(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<WeatherResponse> GetCurrentWeatherAsync(
        string city,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("Nazwa miasta nie może być pusta.", nameof(city));
        }

        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            throw new InvalidOperationException("Brak klucza API WeatherAPI.");
        }

        var url = $"https://api.weatherapi.com/v1/current.json" +
                  $"?key={Uri.EscapeDataString(_apiKey)}" +
                  $"&q={Uri.EscapeDataString(city)}&lang=pl";

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"WeatherAPI zwróciło błąd {(int)response.StatusCode}: {json}");
        }

        return JsonSerializer.Deserialize<WeatherResponse>(
                   json,
                   new JsonSerializerOptions(JsonSerializerDefaults.Web))
               ?? throw new InvalidOperationException(
                   "WeatherAPI zwróciło pustą lub niepoprawną odpowiedź.");
    }
}
