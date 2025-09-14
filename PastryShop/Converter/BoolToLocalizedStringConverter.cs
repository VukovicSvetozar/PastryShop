using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace PastryShop.Converter
{
    public class BoolToLocalizedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool b)
                return DependencyProperty.UnsetValue;

            string trueKey = "YesString";
            string falseKey = "NoString";

            if (parameter is string p && p.Contains(";"))
            {
                var parts = p.Split(';');
                if (parts.Length == 2)
                {
                    trueKey = parts[0];
                    falseKey = parts[1];
                }
            }

            var key = b ? trueKey : falseKey;
            var localized = Application.Current.TryFindResource(key) as string;
            return localized ?? (b ? "True" : "False");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

    }
}