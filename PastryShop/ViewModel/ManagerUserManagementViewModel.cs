using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using PastryShop.Command;
using PastryShop.Data.DTO;
using PastryShop.Service;
using PastryShop.View;
using PastryShop.Enum;
using PastryShop.Utility;

namespace PastryShop.ViewModel
{
    public partial class ManagerUserManagementViewModel : ValidatableBaseViewModel
    {
        private readonly IUserService _userService;

        private ObservableCollection<InfoUserBasicDTO> _allUsers = [];
        private ObservableCollection<InfoUserBasicDTO> _filteredUsers = [];

        public ObservableCollection<InfoUserBasicDTO> Users
        {
            get => _filteredUsers;
            set => SetProperty(ref _filteredUsers, value);
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterUsers();
                }
            }
        }

        private string _selectedFilter = "UserManagementActiveKey";
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (SetProperty(ref _selectedFilter, value))
                {
                    FilterUsers();
                }
            }
        }

        private string _idColumnHeader = string.Empty;
        public string IDColumnHeader
        {
            get => _idColumnHeader;
            set { _idColumnHeader = value; OnPropertyChanged(); }
        }

        private string _usernameColumnHeader = string.Empty;
        public string UsernameColumnHeader
        {
            get => _usernameColumnHeader;
            set { _usernameColumnHeader = value; OnPropertyChanged(); }
        }

        private string _userTypeColumnHeader = string.Empty;
        public string UserTypeColumnHeader
        {
            get => _userTypeColumnHeader;
            set { _userTypeColumnHeader = value; OnPropertyChanged(); }
        }

        private string _actionsColumnHeader = string.Empty;
        public string ActionsColumnHeader
        {
            get => _actionsColumnHeader;
            set { _actionsColumnHeader = value; OnPropertyChanged(); }
        }

        private Language _selectedLanguage;
        public Language SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage != value)
                {
                    _selectedLanguage = value;
                    OnPropertyChanged();
                    UpdateColumnHeaders();
                }
            }
        }

        public ObservableCollection<string> FilterOptions { get; } = ["UserManagementAllUsersKey", "UserManagementActiveKey", "UserManagementInactiveKey"];

        public ICommand AddNewUserCommand { get; }
        public ICommand ShowUserDetailsCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand ChangeUserStatusCommand { get; }
        public ICommand ResetUserPasswordCommand { get; }
        public ICommand ChangeTypeUserCommand { get; }

        public ManagerUserManagementViewModel(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            SelectedLanguage = UserSession.GetCurrentUser().Language;
            UpdateColumnHeaders();

            AddNewUserCommand = new AsyncRelayCommand(_ => ShowAddUserDialog());
            ShowUserDetailsCommand = new AsyncRelayCommand(ShowUserDetails);
            EditUserCommand = new AsyncRelayCommand(EditUser);
            ChangeUserStatusCommand = new AsyncRelayCommand(ChangeStatus);
            ResetUserPasswordCommand = new AsyncRelayCommand(ResetUserPassword);
            ChangeTypeUserCommand = new AsyncRelayCommand(ChangeTypeUser);

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadUsers();
        }

        private async Task LoadUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersBasicInfo();
                _allUsers = new ObservableCollection<InfoUserBasicDTO>(users);
                FilterUsers();
            }
            catch (Exception ex)
            {
                Logger.LogError("Greška tokom učitavanja korisnika.", ex);
            }
        }

        private void FilterUsers()
        {
            var filtered = _allUsers;

            if (_selectedFilter == "UserManagementActiveKey")
            {
                filtered = new ObservableCollection<InfoUserBasicDTO>(_allUsers.Where(u => u.IsActive));
            }
            else if (_selectedFilter == "UserManagementInactiveKey")
            {
                filtered = new ObservableCollection<InfoUserBasicDTO>(_allUsers.Where(u => !u.IsActive));
            }

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                filtered = new ObservableCollection<InfoUserBasicDTO>(
                    filtered.Where(u =>
                        u.Username.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                        u.UserType.ToString().Contains(_searchText, StringComparison.OrdinalIgnoreCase)));
            }

            Users = filtered;
        }

        private async Task ShowAddUserDialog()
        {
            var dialog = new ManagerAddUserDialog(_userService);

            if (dialog.ShowDialog() == true)
            {
                await LoadUsers();
            }
        }

        private async Task ShowUserDetails(object? parameter)
        {
            if (parameter is InfoUserBasicDTO user)
            {
                try
                {
                    var userDetails = await _userService.GetUserDetailsById(user.Id);

                    if (userDetails == null)
                    {
                        ShowErrorDialog(GetLocalizedString("DialogInfoUserNotFoundMessage"));
                        return;
                    }

                    var dialog = new ManagerShowUserDialog(userDetails);
                    dialog.ShowDialog();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Greška prilikom učitavanja detalja korisnika: {ex.Message}", ex);
                }
            }
            else
            {
                Logger.LogError("Neispravan unos korisnika.", new ArgumentException("Neispravan unos korisnika"));
            }
        }

        private async Task EditUser(object? parameter)
        {
            if (parameter is InfoUserBasicDTO user)
            {
                try
                {
                    var editUserDto = await _userService.GetEditUserById(user.Id);
                    if (editUserDto == null)
                    {
                        return;
                    }

                    var dialogViewModel = new ManagerEditUserDialogViewModel(editUserDto, _userService);

                    var dialog = new ManagerEditUserDialog
                    {
                        DataContext = dialogViewModel
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        ShowInfoDialog(GetLocalizedString("DialogInfoDataUpdatedSuccessfullyMessage"));
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Greška prilikom ažuriranja podataka: {ex.Message}", ex);
                    Logger.LogError($"Greška prilikom ažuriranja podataka: {ex.Message}", ex);

                }
            }
            else
            {
                Logger.LogError("Neispravan unos korisnika.", new ArgumentException("Neispravan unos korisnika"));
            }
        }

        private async Task ChangeStatus(object? parameter)
        {
            if (parameter is InfoUserBasicDTO user)
            {
                if (user.Id == UserSession.GetCurrentUser().Id)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningYouCannotDeactivateYourAccountMessage"));
                    return;
                }
                if (user.IsActive)
                {
                    var dialog = CreateChoiceDialog(string.Format(GetLocalizedString("DialogChoiceDeactivateUserAccountMessage"), user.Username));

                    if (dialog.ShowDialog() == true)
                    {
                        await Task.Run(() => _userService.DeactivateUser(user.Id));
                        _ = LoadUsers();
                        ShowInfoDialog(GetLocalizedString("DialogInfoAccountDeactivatedSuccessfullyMessage"));
                    }
                }
                else
                {
                    var dialog = CreateChoiceDialog(string.Format(GetLocalizedString("DialogChoiceActivateUserAccountMessage"), user.Username));

                    if (dialog.ShowDialog() == true)
                    {
                        await Task.Run(() => _userService.ActivateUser(user.Id));
                        _ = LoadUsers();
                        ShowInfoDialog(GetLocalizedString("DialogInfoAccountActivatedSuccessfullyMessage"));
                    }
                }
            }
        }

        private async Task ResetUserPassword(object? parameter)
        {
            if (parameter is InfoUserBasicDTO user)
            {
                if (user.Id == UserSession.GetCurrentUser().Id)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningYouCannotResetYourPasswordMessage"));
                    return;
                }
                if (!user.IsActive)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningAccountNotActiveMessage"));
                    return;
                }

                var dialog = CreateChoiceDialog(string.Format(GetLocalizedString("DialogChoiceResetUserPasswordMessage"), user.Username));

                if (dialog.ShowDialog() == true)
                {
                    string temporaryPassword = GenerateTemporaryPassword();

                    var dto = new EditResetOrChangePasswordDTO
                    {
                        UserId = user.Id,
                        NewPassword = temporaryPassword,
                        ForcePasswordChange = true
                    };

                    await _userService.UpdateUserPassword(dto);
                    ShowInfoDialog(string.Format(GetLocalizedString("DialogInfoTemporaryPasswordMessage"), temporaryPassword));
                }
            }
        }

        private async Task ChangeTypeUser(object? parameter)
        {
            if (parameter is InfoUserBasicDTO user)
            {
                if (user.Id == UserSession.GetCurrentUser().Id)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningYouCannotChangeYourAccountRoleMessage"));
                    return;
                }
                if (!user.IsActive)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningAccountNotActiveMessage"));
                    return;
                }

                var dialog = CreateChoiceDialog(string.Format(GetLocalizedString("DialogChoiceChangeUserRoleMessage"), user.Username));

                if (dialog.ShowDialog() == true)
                {
                    UserType newUserType = user.UserType == UserType.Manager ? UserType.Cashier : UserType.Manager;

                    var dto = new EditChangeUserTypeDTO
                    {
                        UserId = user.Id,
                        NewUserType = newUserType,
                        Department = newUserType == UserType.Manager ? "Not Assigned" : string.Empty,
                        CashRegisterId = newUserType == UserType.Cashier ? 1 : null,
                        ShiftStart = newUserType == UserType.Cashier ? DateTime.Today.AddHours(9) : null,
                        ShiftEnd = newUserType == UserType.Cashier ? DateTime.Today.AddDays(7).AddHours(17) : null
                    };

                    await _userService.ChangeUserType(dto);
                    _ = LoadUsers();
                    ShowInfoDialog(GetLocalizedString("DialogInfoUserRoleChangedSuccessfullyMessage"));
                }
            }
        }

        private static string GenerateTemporaryPassword(int length = 8)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                                        .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void UpdateColumnHeaders()
        {
            if (SelectedLanguage == Language.English)
            {
                IDColumnHeader = "ID";
                UsernameColumnHeader = "Username";
                UserTypeColumnHeader = "User Type";
                ActionsColumnHeader = "Actions";
            }
            else if (SelectedLanguage == Language.Serbian)
            {
                IDColumnHeader = "ID";
                UsernameColumnHeader = "Korisničko Ime";
                UserTypeColumnHeader = "Tip Korisnika";
                ActionsColumnHeader = "Akcije";
            }
        }

    }
}
