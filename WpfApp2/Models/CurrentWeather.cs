namespace WpfApp2.Models;

public sealed class CurrentWeather
{
    public double Temp_C { get; set; }
    public Condition Condition { get; set; } = new();
    public double Feelslike_C { get; set; }
    public int Humidity { get; set; }
    public double Wind_Kph { get; set; }
    public double Pressure_Mb { get; set; }
    public int Is_Day { get; set; }
}

public sealed class Condition
{
    public string Text { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}
