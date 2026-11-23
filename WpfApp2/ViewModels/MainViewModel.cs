using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using WpfApp2.Models;
using WpfApp2.Services;
using System.Text.Json;

namespace WpfApp2.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly WeatherApiService _weatherApiService;
    private readonly DispatcherTimer _clockTimer;

    private string _city = string.Empty;
    private WeatherResponse? _weather;
    private bool _isLoading;
    private string _errorMessage = string.Empty;
    private DateTime? _lastLoadedAt;

    public MainViewModel(WeatherApiService weatherApiService)
    {
        _weatherApiService = weatherApiService;

        // Zegar aktualizuje się co 30 sekund
        _clockTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };

        _clockTimer.Tick += (sender, e) =>
        {
            if (HasLoadedWeather)
            {
                OnPropertyChanged(nameof(ApiTime));
                OnPropertyChanged(nameof(ApiDate));
            }
        };

        _clockTimer.Start();
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
            OnPropertyChanged(nameof(HasLoadedWeather));
        }
    }

    // Informacja, czy pogoda została już pobrana
    public bool HasLoadedWeather => Weather != null;

    // Data widoczna dopiero po pobraniu pogody
    public string ApiDate =>
        HasLoadedWeather
            ? DateTime.Now.ToString(
                "dddd, d MMMM yyyy",
                CultureInfo.CurrentCulture)
            : string.Empty;

    // Godzina widoczna dopiero po pobraniu pogody
    public string ApiTime =>
        HasLoadedWeather
            ? DateTime.Now.ToString(
                "HH:mm:ss",
                CultureInfo.CurrentCulture)
            : string.Empty;

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (!SetProperty(ref _isLoading, value))
                return;

            OnPropertyChanged(nameof(LoadStatus));
        }
    }

    public DateTime? LastLoadedAt
    {
        get => _lastLoadedAt;
        private set
        {
            if (!SetProperty(ref _lastLoadedAt, value))
                return;

            OnPropertyChanged(nameof(LoadStatus));
        }
    }

    // Godzina ostatniego udanego pobrania
    public string LoadStatus => IsLoading
        ? "Ładowanie"
        : LastLoadedAt is DateTime loadedAt
            ? $"Pobrano o {loadedAt:HH:mm:ss}"
            : string.Empty;

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
            // Opóźnienie ładowania 2 sekundy
            await Task.Delay(TimeSpan.FromSeconds(2));

            // Pobieranie aktualnej pogody
            var weather = await _weatherApiService
                .GetCurrentWeatherAsync(City);

            Weather = weather;

            LastLoadedAt = DateTime.Now;

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
            ErrorMessage = "Nie udało się pobrać pogody";
        }
        catch (InvalidOperationException)
        {
            ErrorMessage = "Nie udało się pobrać pogody";
        }
        catch (JsonException)
        {
            ErrorMessage = "Nie udało się pobrać pogody";
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
        {
            Forecast.Add(day);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

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
