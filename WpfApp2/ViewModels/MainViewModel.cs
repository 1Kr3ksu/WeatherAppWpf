using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
                return;

            OnPropertyChanged(nameof(ApiDate));
            OnPropertyChanged(nameof(ApiTime));
        }
    }

    public string ApiDate =>
        GetApiDate("dddd, d MMMM yyyy");

    public string ApiTime =>
        GetApiDate("HH:mm:ss");

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

    public ObservableCollection<ForecastDay> Forecast { get; } = new();

    public async Task LoadWeatherAsync()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(City))
        {
            ErrorMessage = "Wpisz nazwę miasta.";
            return;
        }

        IsLoading = true;

        try
        {
            // Pobierz aktualną pogodę.
            Weather = await _weatherApiService
                .GetCurrentWeatherAsync(City);

            // Pobierz prognozę na 3 dni.
            try
            {
                await LoadForecastAsync();
            }
            catch (HttpRequestException)
            {
                ErrorMessage =
                    "Pobrano aktualną pogodę, ale nie udało się pobrać prognozy.";
            }
            catch (InvalidOperationException)
            {
                ErrorMessage =
                    "Pobrano aktualną pogodę, ale dane prognozy są nieprawidłowe.";
            }
        }
        catch (HttpRequestException)
        {
            Weather = null;
            Forecast.Clear();

            ErrorMessage =
                "Nie znaleziono miasta lub wystąpił problem z połączeniem.";
        }
        catch (InvalidOperationException)
        {
            Weather = null;
            Forecast.Clear();

            ErrorMessage =
                "Nie udało się odczytać danych pogodowych.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadForecastAsync()
    {
        var days = await _weatherApiService.GetForecastAsync(City);

        Forecast.Clear();

        foreach (var day in days)
            Forecast.Add(day);

        ErrorMessage = $"Pobrano dni prognozy: {Forecast.Count}";
    
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private string GetApiDate(string format)
    {
        if (Weather is null ||
            !DateTime.TryParse(
                Weather.Location.Localtime,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var localTime))
        {
            return "--";
        }

        return localTime.ToString(
            format,
            CultureInfo.CurrentCulture);
    }

    private bool SetProperty<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);

        return true;
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}