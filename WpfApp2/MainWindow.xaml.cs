using System.Net.Http;
using System.Windows;
using Wpf.Ui.Controls;
using WpfApp2.Services;
using WpfApp2.ViewModels;

namespace WpfApp2;

public partial class MainWindow : FluentWindow
{
    public MainWindow()
    {
        InitializeComponent();

        var httpClient = new HttpClient();

        var weatherApiService = new WeatherApiService(
            httpClient,
            "3ec7bebea7ca4ccfa0670749250512");

        DataContext = new MainViewModel(weatherApiService);
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