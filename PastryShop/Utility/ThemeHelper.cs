using System.Windows;

namespace PastryShop.Utility
{
    public static class ThemeHelper
    {
        public static event Action? ThemeChanged;

        public static void ApplyTheme(string userType, string theme)
        {
            string themeFile = theme switch
            {
                "Light" => $"{userType}LightTheme.xaml",
                "Dark" => $"{userType}DarkTheme.xaml",
                "Blue" => $"{userType}BlueTheme.xaml",
                _ => $"{userType}DarkTheme.xaml"
            };

            var uri = new Uri($"pack://application:,,,/PastryShop;component/Resource/Styles/{themeFile}", UriKind.Absolute);

            try
            {
                var newTheme = new ResourceDictionary { Source = uri };

                var themeDictionaries = Application.Current.Resources.MergedDictionaries
                    .Where(d => d.Source != null
                             && d.Source.OriginalString.Contains("/Resource/Styles/"))
                    .ToList();
                foreach (var dict in themeDictionaries)
                    Application.Current.Resources.MergedDictionaries.Remove(dict);

                Application.Current.Resources.MergedDictionaries.Add(newTheme);

                ThemeChanged?.Invoke();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška pri primjeni teme: {ex.Message}", new InvalidOperationException($"Greška pri primjeni teme: {ex.Message}", ex));
            }
        }

    }
}