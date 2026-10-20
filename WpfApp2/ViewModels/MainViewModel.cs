using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Runtime.CompilerServices;
using WpfApp2.Models;
using WpfApp2.Services;
namespace WpfApp2.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly WeatherApiService _weatherApiService;
    private string _city = string.Empty;
    private WeatherResponse? _weather;
    private bool _isLoading;
    private string _errorMessage = string.Empty;

    public MainViewModel(WeatherApiService weatherApiService)
    {
        _weatherApiService = weatherApiService;

    }
    public string City
    {
        get => _city;
        set => SetProperty(ref _city, value);
    }

    public WeatherResponse? Weather
    {
        get => _weather;
        set
        {
            if (!SetProperty(ref _weather, value))
            {
                return;
            }

            OnPropertyChanged(nameof(ApiDate));
            OnPropertyChanged(nameof(ApiTime));
        }
    }

    public string ApiDate => GetApiDate("dddd, d MMMM yyyy");

    public string ApiTime => GetApiDate("HH:mm:ss");

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }
    public async Task LoadWeatherAsync()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(City))
        {
            ErrorMessage = "Wpisz nazwę miasta.";
            return;
        }

        IsLoading = true;
        Weather = null;

        try
        {
            Weather = await _weatherApiService.GetCurrentWeatherAsync(City);
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Nie znaleziono miasta lub wystąpił problem z połączeniem.";
        }
        catch (InvalidOperationException)
        {
            ErrorMessage = "Nie udało się odczytać danych pogodowych.";
        }
        finally
        {
            IsLoading = false;
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    private string GetApiDate(string format)
    {
        if (Weather is null || !DateTime.TryParse(
                Weather.Location.Localtime,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var localTime))
        {
            return "--";
        }

        return localTime.ToString(format, CultureInfo.CurrentCulture);
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
