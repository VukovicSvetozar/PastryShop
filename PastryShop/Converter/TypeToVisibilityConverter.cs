using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace PastryShop.Converter
{
    public class TypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? mode = parameter as string;

            switch (mode)
            {
                case "Bool":
                    if (value is bool boolVal)
                        return boolVal ? Visibility.Visible : Visibility.Collapsed;
                    break;

                case "BoolInverse":
                    if (value is bool boolValInverse)
                        return boolValInverse ? Visibility.Collapsed : Visibility.Visible;
                    break;

                case "Zero":
                    if (value is int count)
                        return count == 0 ? Visibility.Visible : Visibility.Collapsed;
                    break;

                case "StringEmpty":
                    if (value is string str)
                        return string.IsNullOrWhiteSpace(str) ? Visibility.Visible : Visibility.Collapsed;
                    break;

                case "StringNotEmpty":
                    if (value is string s)
                        return !string.IsNullOrWhiteSpace(s) ? Visibility.Visible : Visibility.Collapsed;
                    break;

                case "Null":
                    return value == null ? Visibility.Collapsed : Visibility.Visible;

                case "NullInverse":
                    return value == null ? Visibility.Visible : Visibility.Collapsed;

                default:
                    return value == null ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

    }
}