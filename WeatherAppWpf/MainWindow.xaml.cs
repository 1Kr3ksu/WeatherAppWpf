using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WeatherAppWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double windSpeed; // pole klasy do przechowania prędkości wiatru

        public MainWindow()
        {
            InitializeComponent();
           string citySelect= CityComboBox.Text;
            string dateText = DateTextBlock.Text;
            string timeText = TimeTextBlock.Text;
            string temperature = TemperatureTextBlock.Text;
            string condition = ConditionTextBlock.Text;
            string humidity = HumidityTextBlock.Text;
            double.TryParse(WindTextBlock.Text, out windSpeed);
        }
    }
}