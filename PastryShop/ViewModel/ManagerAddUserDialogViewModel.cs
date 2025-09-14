using System.Windows;
using System.Windows.Input;
using System.Globalization;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Win32;
using PastryShop.Command;
using PastryShop.Data.DTO;
using PastryShop.Enum;
using PastryShop.Service;
using PastryShop.Utility;

namespace PastryShop.ViewModel
{
    public partial class ManagerAddUserDialogViewModel : ValidatableBaseViewModel
    {
        private readonly IUserService _userService;

        public AddManagerDTO ManagerDTO { get; set; } = new();
        public AddCashierDTO CashierDTO { get; set; } = new();

        private AddUserDTO? _currentUserDTO;
        public AddUserDTO? CurrentUserDTO
        {
            get => _currentUserDTO;
            set
            {
                _currentUserDTO = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<UserType> UserTypes { get; }

        private UserType? _selectedUserType;
        public UserType? SelectedUserType
        {
            get => _selectedUserType;
            set
            {
                if (SetProperty(ref _selectedUserType, value))
                {
                    ClearAllErrors();
                    UpdateVisibility();
                    ValidateSelectedUserType();
                }
            }
        }

        private bool _isManagerVisible;
        public bool IsManagerVisible
        {
            get => _isManagerVisible;
            set => SetProperty(ref _isManagerVisible, value);
        }

        private bool _isCashierVisible;
        public bool IsCashierVisible
        {
            get => _isCashierVisible;
            set => SetProperty(ref _isCashierVisible, value);
        }

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

        private string? _salaryText;
        public string? SalaryText
        {
            get => _salaryText;
            set
            {
                if (SetProperty(ref _salaryText, value))
                {
                    ValidateSalary();
                }
            }
        }

        private string? _department;
        public string? Department
        {
            get => _department;
            set
            {
                if (SetProperty(ref _department, value))
                {
                    ValidateDepartment();
                }
            }
        }

        private string? _cashRegisterIdText;
        public string? CashRegisterIdText
        {
            get => _cashRegisterIdText;
            set
            {
                if (SetProperty(ref _cashRegisterIdText, value))
                {
                    ValidateCashRegisterId();
                }
            }
        }

        private DateTime _shiftStart = DateTime.Now.AddHours(1);
        public DateTime ShiftStart
        {
            get => _shiftStart;
            set
            {
                if (SetProperty(ref _shiftStart, value))
                {
                    ValidateDates();
                }
            }
        }

        private DateTime _shiftEnd = DateTime.Now.AddHours(9);
        public DateTime ShiftEnd
        {
            get => _shiftEnd;
            set
            {
                if (SetProperty(ref _shiftEnd, value))
                {
                    ValidateDates();
                }
            }
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

        public ICommand AddCommand { get; }
        public ICommand BrowseImageCommand { get; }

        public ManagerAddUserDialogViewModel(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            UserTypes =
            [
                UserType.Manager,
                UserType.Cashier
            ];

            SelectedUserType = null;

            AddCommand = new AsyncRelayCommand(_ => AddUser());
            BrowseImageCommand = new AsyncRelayCommand(_ => BrowseImage());

        }

        private void UpdateVisibility()
        {
            IsManagerVisible = SelectedUserType == UserType.Manager;
            IsCashierVisible = SelectedUserType == UserType.Cashier;
            CurrentUserDTO = SelectedUserType == UserType.Manager ? ManagerDTO : CashierDTO;
        }

        public async Task Validate()
        {
            ValidateSelectedUserType();
            await ValidateUsername();
            ValidatePassword();
            ValidateFirstName();
            ValidateLastName();
            ValidatePhoneNumber();
            ValidateSalary();
            if (SelectedUserType == UserType.Manager)
            {
                ValidateDepartment();
            }
            else if (SelectedUserType == UserType.Cashier)
            {
                ValidateCashRegisterId();
                ValidateDates();
            }
        }

        private void ValidateSelectedUserType()
        {
            ClearErrors(nameof(SelectedUserType));
            if (SelectedUserType == null)
            {
                AddError(nameof(SelectedUserType), GetLocalizedString("ValidateUserTypeMustSelectMessage"));
            }
        }

        private async Task ValidateUsername()
        {
            ClearErrors(nameof(Username));

            const int minLength = 3;
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
            else if (await _userService.UsernameExists(Username))
            {
                AddError(nameof(Username), GetLocalizedString("ValidateUsernameAlreadyExistsMessage"));
            }
        }

        private void ValidatePassword()
        {
            PasswordError = string.Empty;

            const int minLength = 3;
            const int maxLength = 20;

            if (string.IsNullOrWhiteSpace(Password))
            {
                PasswordError = GetLocalizedString("ValidateCannotBeEmptyMessage");
            }
            else if (!ValidLettersAndDigitsRegex.IsMatch(Password))
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

        private void ValidateSalary()
        {
            ClearErrors(nameof(SalaryText));

            if (string.IsNullOrWhiteSpace(SalaryText))
            {
                AddError(nameof(SalaryText), GetLocalizedString("ValidateCannotBeEmptyMessage"));
            }
            else if (!decimal.TryParse(SalaryText, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsedSalary))
            {
                AddError(nameof(SalaryText), GetLocalizedString("ValidateMustBeNumberMessage"));
            }
            else if (parsedSalary < 0)
            {
                AddError(nameof(SalaryText), GetLocalizedString("ValidateCannotBeNegativeMessage"));
            }
        }

        private void ValidateDepartment()
        {
            ClearErrors(nameof(Department));

            if (!string.IsNullOrWhiteSpace(Department))
            {
                if (!ValidLettersDigitsAndSpaceRegex.IsMatch(Department))
                {
                    AddError(nameof(Department), GetLocalizedString("ValidateOnlyLettersDigitsAndSpaceMessage"));
                }
            }
        }

        private void ValidateCashRegisterId()
        {
            ClearErrors(nameof(CashRegisterIdText));

            if (!string.IsNullOrWhiteSpace(CashRegisterIdText))
            {
                if (!ValidDigitsRegex.IsMatch(CashRegisterIdText))
                {
                    AddError(nameof(CashRegisterIdText), GetLocalizedString("ValidateWholeNumberMessage"));
                }
                else if (!int.TryParse(CashRegisterIdText, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                {
                    AddError(nameof(CashRegisterIdText), GetLocalizedString("ValidateValidNumberMessage"));
                }
            }
        }

        private void ValidateDates()
        {
            ClearErrors(nameof(ShiftStart));
            ClearErrors(nameof(ShiftEnd));

            if (ShiftStart < DateTime.Now)
            {
                AddError(nameof(ShiftStart), GetLocalizedString("ValidateCannotBeInThePastMessage"));
            }
            if (ShiftEnd <= ShiftStart)
            {
                AddError(nameof(ShiftEnd), GetLocalizedString("ValidateShiftMustEndAfterItStartsMessage"));
            }
            else if (ShiftEnd < ShiftStart.AddHours(3))
            {
                AddError(nameof(ShiftEnd), GetLocalizedString("ValidateShiftMustLastAtLeastOneHourMessage"));
            }
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

        private async Task AddUser()
        {
            try
            {
                await Validate();

                if (HasErrors)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningInvalidFieldValuesMessage"));
                    return;
                }

                CurrentUserDTO!.Username = Username;
                CurrentUserDTO.FirstName = FirstName!;
                CurrentUserDTO.LastName = LastName!;
                CurrentUserDTO.PhoneNumber = PhoneNumber!;
                CurrentUserDTO.Salary = decimal.Parse(SalaryText!, NumberStyles.Number, CultureInfo.InvariantCulture);
                CurrentUserDTO.ImagePath = SelectedImagePath;

                if (SelectedUserType == UserType.Manager)
                {
                    ManagerDTO.Password = Password;
                    ManagerDTO.Department = string.IsNullOrWhiteSpace(Department) ? string.Empty : Department!;
                    await _userService.CreateUser(ManagerDTO);
                }
                else if (SelectedUserType == UserType.Cashier)
                {
                    CashierDTO.Password = Password;
                    CashierDTO.CashRegisterId = int.TryParse(CashRegisterIdText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int id)
                        ? id : 0;
                    CashierDTO.ShiftStart = ShiftStart;
                    CashierDTO.ShiftEnd = ShiftEnd;

                    await _userService.CreateUser(CashierDTO);
                }

                ShowInfoDialog(GetLocalizedString("DialogInfoUserAddedMessage"));

                CloseWindow();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška pri dodavanju korisnika: {ex.Message}", ex);
            }
        }

        private static void CloseWindow()
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

    }
}