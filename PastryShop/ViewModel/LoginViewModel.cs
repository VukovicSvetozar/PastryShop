using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using PastryShop.Command;
using PastryShop.Data.DTO;
using PastryShop.Enum;
using PastryShop.Service;
using PastryShop.Utility;
using PastryShop.View;
using PastryShop.View.Dialog;
using PastryShop.ViewModel.Dialog;

namespace PastryShop.ViewModel
{
    public partial class LoginViewModel : ValidatableBaseViewModel
    {

        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IStockService _stockService;
        private readonly ICartService _cartService;

        private ObservableCollection<Language> _availableLanguages = new(System.Enum.GetValues(typeof(Language)).Cast<Language>());
        public ObservableCollection<Language> AvailableLanguages
        {
            get => _availableLanguages;
            set
            {
                _availableLanguages = value;
                OnPropertyChanged();
            }
        }
        private Language _selectedLanguage = Language.English;
        public Language SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage != value)
                {
                    _selectedLanguage = value;
                    OnPropertyChanged(nameof(SelectedLanguage));
                    LanguageHelper.ApplyLanguage(_selectedLanguage.ToString());
                    AvailableLanguages = new ObservableCollection<Language>(System.Enum.GetValues(typeof(Language)).Cast<Language>());
                    Validate();
                }
            }
        }

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    ValidateUsername();
                }
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    ValidatePassword();
                }
            }
        }

        private string _passwordError = string.Empty;
        public string PasswordError
        {
            get => _passwordError;
            set => SetProperty(ref _passwordError, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand CancelCommand { get; }

        public LoginViewModel(IUserService userService, IProductService productService, IStockService stockService, ICartService cartService)
        {
            _userService = userService;
            _productService = productService;
            _stockService = stockService;
            _cartService = cartService;

            LanguageHelper.ApplyLanguage(_selectedLanguage.ToString());

            LoginCommand = new AsyncRelayCommand(Login);
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        public void Validate()
        {
            ValidateUsername();
            ValidatePassword();
        }

        private void ValidateUsername()
        {
            ClearErrors(nameof(Username));

            const int minLength = 3;
            const int maxLength = 20;

            if (string.IsNullOrWhiteSpace(Username))
            {
                AddError(nameof(Username), GetLocalizedString("ValidateUsernameEmptyMessage"));
            }
            else if (!ValidLettersAndDigitsRegex.IsMatch(Username))
            {
                AddError(nameof(Username), GetLocalizedString("ValidateUsernameInvalidCharactersMessage"));
            }
            else if (Username.Length < minLength)
            {
                AddError(nameof(Username), string.Format(GetLocalizedString("ValidateUsernameTooShortMessage"), minLength));
            }
            else if (Username.Length > maxLength)
            {
                AddError(nameof(Username), string.Format(GetLocalizedString("ValidateUsernameTooLongMessage"), maxLength));
            }
        }

        private void ValidatePassword()
        {
            PasswordError = string.Empty;

            const int minLength = 3;
            const int maxLength = 20;

            if (string.IsNullOrWhiteSpace(Password))
            {
                PasswordError = GetLocalizedString("ValidatePasswordEmptyMessage");
            }
            else if (!ValidLettersAndDigitsRegex.IsMatch(Password))
            {
                PasswordError = GetLocalizedString("ValidatePasswordInvalidCharactersMessage");
            }
            else if (Password.Length < minLength)
            {
                PasswordError = string.Format(GetLocalizedString("ValidatePasswordTooShortMessage"), minLength);
            }
            else if (Password.Length > maxLength)
            {
                PasswordError = string.Format(GetLocalizedString("ValidatePasswordTooLongMessage"), maxLength);
            }
        }

        private async Task Login(object? parameter)
        {
            Validate();

            if (HasErrors || !string.IsNullOrEmpty(PasswordError))
            {
                ShowInfoDialog("⚠", GetLocalizedString("DialogWarningInvalidFieldValuesMessage"));
                return;
            }

            var user = await _userService.AuthenticateUser(new LoginCredentialUserDTO
            {
                Username = Username,
                Password = Password
            });

            if (user != null)
            {
                if (!user.IsActive)
                {
                    ShowInfoDialog("⚠", GetLocalizedString("DialogWarningAccountNotActiveMessage"));
                    return;
                }

                UserSession.SetCurrentUser(user);

                if (user.ForcePasswordChange)
                {
                    if (!await PromptForPasswordChange(user.Id))
                    {
                        return;
                    }
                }

                await _userService.EditUserLastLogin(user.Id);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    string theme = user.Theme.ToString();
                    string userType = user.UserType.ToString();
                    ThemeHelper.ApplyTheme(userType, theme);
                });

                Application.Current.Dispatcher.Invoke(() =>
                {
                    string lang = user.Language.ToString();
                    LanguageHelper.ApplyLanguage(lang);
                });

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Window? nextPage = user.UserType switch
                    {
                        UserType.Manager => new ManagerPage(_userService, _productService, _stockService),
                        UserType.Cashier => new CashierPage(_userService, _productService, _stockService, _cartService),
                        _ => null
                    };

                    if (nextPage != null)
                    {
                        Application.Current.MainWindow = nextPage;
                        nextPage.Show();
                        CloseLoginWindow();
                    }
                    else
                    {
                        Logger.LogError("Nepoznat tip korisnika.", new ArgumentException("Nepoznat tip korisnika"));
                    }
                });
            }
            else
            {
                ShowInfoDialog("⚠", GetLocalizedString("DialogWarningIncorrectUsernameOrPasswordMessage"));
            }
        }

        private static Task Cancel()
        {
            CloseLoginWindow();
            return Task.CompletedTask;
        }

        private static void CloseLoginWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is LoginPage)
                {
                    window.Close();
                    break;
                }
            }
        }

        private async Task<bool> PromptForPasswordChange(int id)
        {
            var dialogViewModel = new LoginInputDialogViewModel
            {
                Message = GetLocalizedString("DialogInfoChangePasswordMessage"),
                PlaceHolderText = GetLocalizedString("DialogInfoEnterNewPasswordMessage"),
                ErrorMessage = GetLocalizedString("DialogInfoFieldCannoteEmptyMessage")
            };

            var dialog = new LoginInputDialog
            {
                DataContext = dialogViewModel
            };

            if (dialog.ShowDialog() == true)
            {
                var dto = new EditResetOrChangePasswordDTO
                {
                    UserId = id,
                    NewPassword = dialogViewModel.Input,
                    ForcePasswordChange = false
                };

                await _userService.UpdateUserPassword(dto);

                ShowInfoDialog("✅", GetLocalizedString("DialogInfoPasswordChangedsuccessfullyMessage"));
                return true;
            }

            ShowInfoDialog("⚠", GetLocalizedString("DialogWarningPasswordNotChangedMessage"));
            Application.Current.Shutdown();
            return false;
        }

        public static void ShowInfoDialog(string icon, string message)
        {
            var infoDialogViewModel = new LoginInfoDialogViewModel
            {
                IconText = icon,
                MessageText = message
            };

            var infoDialog = new LoginInfoDialog
            {
                DataContext = infoDialogViewModel
            };

            infoDialog.ShowDialog();
        }

    }
}