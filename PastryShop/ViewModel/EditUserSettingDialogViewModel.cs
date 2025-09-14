using System.Collections.ObjectModel;
using System.Windows.Input;
using PastryShop.Command;
using PastryShop.Enum;
using PastryShop.Service;
using PastryShop.Utility;

namespace PastryShop.ViewModel
{
    public class EditUserSettingDialogViewModel : ValidatableBaseViewModel
    {
        private readonly IUserService _userService;

        private ObservableCollection<Theme>? _themeOptions;
        public ObservableCollection<Theme>? ThemeOptions
        {
            get => _themeOptions;
            private set
            {
                _themeOptions = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Language>? _languageOptions;
        public ObservableCollection<Language>? LanguageOptions
        {
            get => _languageOptions;
            private set
            {
                _languageOptions = value;
                OnPropertyChanged();
            }
        }

        private Theme? _selectedTheme;
        public Theme? SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (_selectedTheme != value)
                {
                    _selectedTheme = value;
                    OnPropertyChanged(nameof(SelectedTheme));
                }
            }
        }

        private Language? _selectedLanguage;
        public Language? SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage != value)
                {
                    _selectedLanguage = value;
                    OnPropertyChanged(nameof(SelectedLanguage));
                }
            }
        }

        public ICommand SaveCommand { get; }

        public EditUserSettingDialogViewModel(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            ThemeOptions = new ObservableCollection<Theme>(System.Enum.GetValues(typeof(Theme)).Cast<Theme>());
            LanguageOptions = new ObservableCollection<Language>(System.Enum.GetValues(typeof(Language)).Cast<Language>());

            SelectedTheme = UserSession.GetCurrentUser().Theme;
            SelectedLanguage = UserSession.GetCurrentUser().Language;

            SaveCommand = new AsyncRelayCommand(_ => SaveSettings());
        }

        private async Task SaveSettings()
        {
            try
            {
                string themeString = SelectedTheme.ToString()!;
                string languageString = SelectedLanguage.ToString()!;

                var currentUser = UserSession.GetCurrentUser();
                bool isThemeChanged = currentUser.Theme.ToString() != themeString;
                bool isLanguageChanged = currentUser.Language.ToString() != languageString;

                if (isThemeChanged)
                {
                    ThemeHelper.ApplyTheme(currentUser.UserType.ToString(), themeString);
                    await _userService.ChangeTheme(currentUser.Id, themeString);
                }

                if (isLanguageChanged)
                {
                    LanguageHelper.ApplyLanguage(languageString);
                    await _userService.ChangeLanguage(currentUser.Id, languageString);

                    var prevSelectedLanguage = SelectedLanguage;
                    SelectedLanguage = null;
                    OnPropertyChanged(nameof(SelectedLanguage));

                    LanguageOptions = new ObservableCollection<Language>(System.Enum.GetValues(typeof(Language)).Cast<Language>());

                    SelectedLanguage = prevSelectedLanguage;
                    OnPropertyChanged(nameof(SelectedLanguage));

                    var prevSelectedTheme = SelectedTheme;
                    SelectedTheme = null;
                    OnPropertyChanged(nameof(SelectedTheme));

                    ThemeOptions = new ObservableCollection<Theme>(System.Enum.GetValues(typeof(Theme)).Cast<Theme>());

                    SelectedTheme = prevSelectedTheme;
                    OnPropertyChanged(nameof(SelectedTheme));
                }

                string messageKey = (isThemeChanged, isLanguageChanged) switch
                {
                    (true, true) => "EditUserSettingThemeAndLanguageChangedString",
                    (true, false) => "EditUserSettingThemeChangedString",
                    (false, true) => "EditUserSettingLanguageChangedString",
                    _ => "EditUserSettingNoChangesMadeString"
                };

                string message = GetLocalizedString(messageKey);

                ShowInfoDialog(message);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom podešavanja aplikacije: {ex.Message}", ex);
            }
        }

    }
}