using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PastryShop.Converter
{
    public class ResourceKeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string resourceKey)
            {
                return Application.Current.TryFindResource(resourceKey) ?? resourceKey;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

    }
}