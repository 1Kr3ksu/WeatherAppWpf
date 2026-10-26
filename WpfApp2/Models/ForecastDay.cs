
using System;
using System.Globalization;

namespace WpfApp2.Models;

public sealed class ForecastDay
{
    public DateTime Date { get; set; }

    public double MaxTempC { get; set; }

    public double MinTempC { get; set; }

    public string Condition { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public int ChanceOfRain { get; set; }

    public double MaxWindKph { get; set; }

    public string DayName => Date.ToString(
        "dddd",
        new CultureInfo("pl-PL"));

    public string DateText => Date.ToString("dd.MM");

    public string TemperatureText =>
        $"{MaxTempC:0}° / {MinTempC:0}°C";
}
