using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace PastryShop.Converter
{
    public class WidthToVisibilityConverter : IValueConverter
    {
        public double Threshold { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double actual)
            {

                return actual >= Threshold
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

    }
}