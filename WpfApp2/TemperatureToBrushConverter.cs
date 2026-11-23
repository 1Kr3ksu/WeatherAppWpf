using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApp2;

public sealed class TemperatureToBrushConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (value is double temperature)
        {
            if (temperature < 0)
            {
                return new SolidColorBrush(
                    Color.FromRgb(180, 220, 255));
            }

            if (temperature > 25)
            {
                return new SolidColorBrush(
                    Color.FromRgb(255, 210, 150));
            }
        }

        return new SolidColorBrush(
            Color.FromRgb(235, 235, 235));
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}