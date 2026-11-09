using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui.Controls;
using WpfApp2.Services;
using WpfApp2.ViewModels;

namespace WpfApp2;

public partial class MainWindow : FluentWindow
{
    private readonly DispatcherTimer _weatherRefreshTimer;
    private readonly DispatcherTimer _clockTimer;

    public MainWindow()
    {
        InitializeComponent();

        var httpClient = new HttpClient();

        var weatherApiService = new WeatherApiService(
            httpClient,
            "3ec7bebea7ca4ccfa0670749250512");

        DataContext = new MainViewModel(weatherApiService);

        // Timer odświeżania pogody
        _weatherRefreshTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };

        _weatherRefreshTimer.Tick += WeatherRefreshTimer_Tick;
        _weatherRefreshTimer.Start();

        // Timer zegara
        _clockTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };

        _clockTimer.Tick += (sender, e) =>
        {
            TimeTextBlock.Text = DateTime.Now.ToString("HH:mm:ss");
        };

        _clockTimer.Start();

        TimeTextBlock.Text = DateTime.Now.ToString("HH:mm:ss");
    }

    private async void WeatherRefreshTimer_Tick(
        object? sender,
        EventArgs e)
    {
        if (DataContext is MainViewModel viewModel && !viewModel.IsLoading)
        {
            await viewModel.LoadWeatherAsync();
        }
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel)
        {
            return;
        }

        RefreshButton.IsEnabled = false;

        try
        {
            await viewModel.LoadWeatherAsync();
        }
        finally
        {
            RefreshButton.IsEnabled = true;
        }
    }
}