using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PastryShop.Converter
{
    public class EnumToResourceKeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not System.Enum enumValue)
                return string.Empty;

            var enumName = System.Enum.GetName(enumValue.GetType(), enumValue) ?? string.Empty;

            var prefix = parameter as string ?? string.Empty;

            var resourceKey = $"{prefix}{enumName}Key";

            var localized = Application.Current.TryFindResource(resourceKey) as string;
            return localized ?? enumName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

    }
}