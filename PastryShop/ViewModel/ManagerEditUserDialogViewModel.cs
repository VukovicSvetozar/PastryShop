using System.Windows;
using System.Windows.Input;
using System.Globalization;
using PastryShop.Service;
using PastryShop.Data.DTO;
using PastryShop.Command;
using PastryShop.Enum;

namespace PastryShop.ViewModel
{
    public partial class ManagerEditUserDialogViewModel : ValidatableBaseViewModel
    {
        public EditUserManagementDTO User { get; set; }

        private readonly IUserService _userService;

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
                    if (User?.UserType == UserType.Manager)
                    {
                        ValidateDepartment();
                    }
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
                    if (User?.UserType == UserType.Cashier)
                    {
                        ValidateCashRegisterId();
                    }
                }
            }
        }

        private DateTime? _shiftStartDate;
        public DateTime? ShiftStartDate
        {
            get => _shiftStartDate;
            set
            {
                if (SetProperty(ref _shiftStartDate, value))
                {
                    if (User?.UserType == UserType.Cashier)
                    {
                        ValidateDates();
                    }
                }
            }
        }

        private DateTime? _shiftEndDate;
        public DateTime? ShiftEndDate
        {
            get => _shiftEndDate;
            set
            {
                if (SetProperty(ref _shiftEndDate, value))
                {
                    if (User?.UserType == UserType.Cashier)
                    {
                        ValidateDates();
                    }
                }
            }
        }

        public ICommand SaveCommand { get; }

        public ManagerEditUserDialogViewModel(EditUserManagementDTO user, IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            User = user ?? throw new ArgumentNullException(nameof(user));

            SalaryText = User.Salary.ToString(CultureInfo.InvariantCulture);
            Department = User.Department ?? string.Empty;
            CashRegisterIdText = User.CashRegisterId?.ToString() ?? string.Empty;
            ShiftStartDate = User.ShiftStart;
            ShiftEndDate = User.ShiftEnd;

            SaveCommand = new AsyncRelayCommand(_ => SaveUser());
        }

        public void Validate()
        {
            ValidateSalary();
            if (User?.UserType == UserType.Manager)
            {
                ValidateDepartment();
            }
            else if (User?.UserType == UserType.Cashier)
            {
                ValidateCashRegisterId();
                ValidateDates();
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
            ClearErrors(nameof(ShiftStartDate));
            ClearErrors(nameof(ShiftEndDate));

            if (ShiftStartDate.HasValue && ShiftStartDate.Value < DateTime.Now)
            {
                AddError(nameof(ShiftStartDate), GetLocalizedString("ValidateCannotBeInThePastMessage"));
            }
            if (ShiftStartDate.HasValue && ShiftEndDate.HasValue)
            {
                if (ShiftEndDate.Value <= ShiftStartDate.Value)
                {
                    AddError(nameof(ShiftEndDate), GetLocalizedString("ValidateShiftMustEndAfterItStartsMessage"));
                }
                else if (ShiftEndDate.Value < ShiftStartDate.Value.AddHours(3))
                {
                    AddError(nameof(ShiftEndDate), GetLocalizedString("ValidateShiftMustLastAtLeastOneHourMessage"));
                }
            }
        }

        private async Task SaveUser()
        {
            try
            {
                Validate();

                if (HasErrors)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningInvalidFieldValuesMessage"));
                    return;
                }

                User.Salary = decimal.Parse(SalaryText!, NumberStyles.Number, CultureInfo.InvariantCulture);
                User.Department = string.IsNullOrWhiteSpace(Department) ? null : Department;

                if (!string.IsNullOrWhiteSpace(CashRegisterIdText))
                {
                    User.CashRegisterId = int.Parse(CashRegisterIdText, NumberStyles.Integer, CultureInfo.InvariantCulture);
                }
                else
                {
                    User.CashRegisterId = null;
                }

                User.ShiftStart = ShiftStartDate;
                User.ShiftEnd = ShiftEndDate;

                await _userService.EditUser(User);

                CloseWindow();
            }
            catch (Exception ex)
            {
                ShowErrorDialog($"Greška prilikom ažuriranja: {ex.Message}");
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