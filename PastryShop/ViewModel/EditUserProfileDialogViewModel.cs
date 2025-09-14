using System.Windows.Input;
using System.IO;
using Microsoft.Win32;
using PastryShop.Service;
using PastryShop.Data.DTO;
using PastryShop.Command;
using PastryShop.Utility;

namespace PastryShop.ViewModel
{
    public partial class EditUserProfileDialogViewModel : ValidatableBaseViewModel
    {
        private readonly IUserService _userService;

        public event EventHandler? ResetCommandExecuted;

        public EditUserProfileDTO UserProfile { get; set; }

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    _ = ValidateUsername();
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

        private string? _firstName;
        public string? FirstName
        {
            get => _firstName;
            set
            {
                if (SetProperty(ref _firstName, value))
                {
                    ValidateFirstName();
                }
            }
        }

        private string? _lastName;
        public string? LastName
        {
            get => _lastName;
            set
            {
                if (SetProperty(ref _lastName, value))
                {
                    ValidateLastName();
                }
            }
        }

        private string? _phoneNumber;
        public string? PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (SetProperty(ref _phoneNumber, value))
                {
                    ValidatePhoneNumber();
                }
            }
        }

        private string? _address;
        public string? Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        private string? _selectedImagePath;
        public string? SelectedImagePath
        {
            get => _selectedImagePath;
            set
            {
                _selectedImagePath = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand BrowseImageCommand { get; }

        public EditUserProfileDialogViewModel(EditUserProfileDTO userProfile, IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            UserProfile = userProfile ?? throw new ArgumentNullException(nameof(userProfile));

            Username = UserProfile.Username;
            FirstName = UserProfile.FirstName;
            LastName = UserProfile.LastName;
            PhoneNumber = UserProfile.PhoneNumber;
            Address = UserProfile.Address;
            SelectedImagePath = UserProfile.ImagePath;

            SaveCommand = new AsyncRelayCommand(_ => SaveUserProfile());
            ResetCommand = new RelayCommand(_ => ResetFields());
            BrowseImageCommand = new AsyncRelayCommand(_ => BrowseImage());
        }

        public async Task Validate()
        {
            await ValidateUsername();
            ValidatePassword();
            ValidateFirstName();
            ValidateLastName();
            ValidatePhoneNumber();
        }

        private async Task ValidateUsername()
        {
            ClearErrors(nameof(Username));

            const int minLength = 1;
            const int maxLength = 20;

            if (string.IsNullOrWhiteSpace(Username))
            {
                AddError(nameof(Username), GetLocalizedString("ValidateCannotBeEmptyMessage"));
            }
            else if (!ValidLettersAndDigitsRegex.IsMatch(Username))
            {
                AddError(nameof(Username), GetLocalizedString("ValidateContainsInvalidCharactersMessage"));
            }
            else if (Username.Length < minLength)
            {
                AddError(nameof(Username), string.Format(GetLocalizedString("ValidateLeastCharactersLongMessage"), minLength));
            }
            else if (Username.Length > maxLength)
            {
                AddError(nameof(Username), string.Format(GetLocalizedString("ValidateMoreThanCharactersLongMessage"), maxLength));
            }
            else if (await _userService.UsernameExists(Username) && !Username.Equals(UserProfile.Username, StringComparison.OrdinalIgnoreCase))
            {
                AddError(nameof(Username), GetLocalizedString("ValidateUsernameAlreadyExistsMessage"));
            }
        }

        private void ValidatePassword()
        {
            PasswordError = string.Empty;

            const int minLength = 1;
            const int maxLength = 20;

            if (string.IsNullOrWhiteSpace(Password))
            {
                return;
            }
            if (!ValidLettersAndDigitsRegex.IsMatch(Password))
            {
                PasswordError = GetLocalizedString("ValidateContainsInvalidCharactersMessage");
            }
            else if (Password.Length < minLength)
            {
                PasswordError = string.Format(GetLocalizedString("ValidateLeastCharactersLongMessage"), minLength);
            }
            else if (Password.Length > maxLength)
            {
                PasswordError = string.Format(GetLocalizedString("ValidateMoreThanCharactersLongMessage"), maxLength);
            }
        }
        private void ValidateFirstName()
        {
            ClearErrors(nameof(FirstName));

            if (string.IsNullOrWhiteSpace(FirstName))
            {
                AddError(nameof(FirstName), GetLocalizedString("ValidateCannotBeEmptyMessage"));
            }
            else if (!ValidLettersDigitsAndSpaceRegex.IsMatch(FirstName))
            {
                AddError(nameof(FirstName), GetLocalizedString("ValidateOnlyLettersDigitsAndSpaceMessage"));
            }
        }

        private void ValidateLastName()
        {
            ClearErrors(nameof(LastName));

            if (string.IsNullOrWhiteSpace(LastName))
            {
                AddError(nameof(LastName), GetLocalizedString("ValidateCannotBeEmptyMessage"));
            }
            else if (!ValidLettersDigitsAndSpaceRegex.IsMatch(LastName))
            {
                AddError(nameof(LastName), GetLocalizedString("ValidateOnlyLettersDigitsAndSpaceMessage"));
            }
        }

        private void ValidatePhoneNumber()
        {
            ClearErrors(nameof(PhoneNumber));

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                AddError(nameof(PhoneNumber), GetLocalizedString("ValidateCannotBeEmptyMessage"));
            }
            else if (!ValidPhoneNumberRegex.IsMatch(PhoneNumber))
            {
                AddError(nameof(PhoneNumber), GetLocalizedString("ValidateIncorrectFormatMessage"));
            }
        }

        private async Task SaveUserProfile()
        {
            try
            {
                await Validate();

                if (HasErrors)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningInvalidFieldValuesMessage"));
                    return;
                }

                UserProfile!.Username = Username;
                UserProfile.Password = Password;
                UserProfile.FirstName = FirstName!;
                UserProfile.LastName = LastName!;
                UserProfile.PhoneNumber = PhoneNumber!;
                UserProfile.Address = Address;
                UserProfile.ImagePath = SelectedImagePath;

                await _userService.EditUserProfile(UserProfile);

                var currentUser = UserSession.GetCurrentUser();
                var updatedUser = UpdateCurrentUserFromProfile(UserProfile, currentUser);
                UserSession.SetCurrentUser(updatedUser);

                ShowInfoDialog(GetLocalizedString("DialogInfoProductEditProfileSuccessfullyMessage"));
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom ažuriranja: {ex.Message}", ex);
            }
        }

        private void ResetFields()
        {
            Username = UserProfile.Username;
            Password = string.Empty;
            FirstName = UserProfile.FirstName;
            LastName = UserProfile.LastName;
            PhoneNumber = UserProfile.PhoneNumber;
            Address = UserProfile.Address;
            SelectedImagePath = UserProfile.ImagePath;

            PasswordError = string.Empty;

            ClearErrors(nameof(Username));
            ClearErrors(nameof(FirstName));
            ClearErrors(nameof(LastName));
            ClearErrors(nameof(PhoneNumber));

            ResetCommandExecuted?.Invoke(this, EventArgs.Empty);
        }

        private async Task BrowseImage()
        {
            await Task.Run(() =>
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png",
                    Title = "Select Profile Image"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        string destinationFolder = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "PastryShop",
                            "Resource",
                            "Images",
                            "Users");

                        if (!Directory.Exists(destinationFolder))
                            Directory.CreateDirectory(destinationFolder);

                        string fileName = Path.GetFileName(openFileDialog.FileName);
                        string destinationPath = Path.Combine(destinationFolder, fileName);

                        if (File.Exists(destinationPath))
                        {
                            string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                            string extension = Path.GetExtension(fileName);
                            fileName = $"{nameWithoutExt}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                            destinationPath = Path.Combine(destinationFolder, fileName);
                        }

                        File.Copy(openFileDialog.FileName, destinationPath, true);

                        SelectedImagePath = fileName;
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Logger.LogError("Nemate dozvolu za kopiranje fajla. " + ex.Message, ex);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Došlo je do greške: " + ex.Message, ex);
                    }
                }
            });
        }

        private static LoginAuthenticatedUserDTO UpdateCurrentUserFromProfile(EditUserProfileDTO profile, LoginAuthenticatedUserDTO currentUser)
        {
            return new LoginAuthenticatedUserDTO
            {
                Id = currentUser.Id,
                Username = profile.Username,
                ImagePath = profile.ImagePath,
                UserType = currentUser.UserType,
                Theme = currentUser.Theme,
                Language = currentUser.Language,
                IsActive = currentUser.IsActive,
                ForcePasswordChange = currentUser.ForcePasswordChange
            };
        }

    }
}