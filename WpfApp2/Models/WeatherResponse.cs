
namespace WpfApp2.Models;

public sealed class WeatherResponse
{
    public Location Location { get; set; } = new();
    public CurrentWeather Current { get; set; } = new();
}
