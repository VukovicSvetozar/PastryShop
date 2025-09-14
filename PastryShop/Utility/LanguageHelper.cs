using System.Windows;

namespace PastryShop.Utility
{
    public static class LanguageHelper
    {
        public static event Action? LanguageChanged;

        public static void ApplyLanguage(string language)
        {
            string languagePath = language switch
            {
                "English" => "/Resource/Languages/Strings.en.xaml",
                "Serbian" => "/Resource/Languages/Strings.sr.xaml",
                _ => "/Resource/Languages/Strings.en.xaml"
            };

            try
            {
                var newLang = new ResourceDictionary
                {
                    Source = new Uri(languagePath, UriKind.Relative)
                };

                var existingLangDictionaries = Application.Current.Resources.MergedDictionaries
                    .Where(d => d.Source != null && d.Source.OriginalString.Contains("Strings."))
                    .ToList();
                foreach (var dict in existingLangDictionaries)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(dict);
                }

                Application.Current.Resources.MergedDictionaries.Add(newLang);

                LanguageChanged?.Invoke();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška pri primjeni jezika: {ex.Message}", new InvalidOperationException($"Greška pri primjeni jezika: {ex.Message}", ex));
            }
        }

    }
}