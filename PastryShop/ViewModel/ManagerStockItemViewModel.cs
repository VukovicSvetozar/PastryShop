using System.Globalization;
using PastryShop.Data.DTO;

namespace PastryShop.ViewModel
{
    public partial class StockItemViewModel : ValidatableBaseViewModel
    {
        public InfoStockDTO Model { get; }

        public int Id
        {
            get => Model.Id;
            set
            {
                Model.Id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public int ProductId => Model.ProductId;

        public DateTime AddedDate
        {
            get => Model.AddedDate;
            set
            {
                Model.AddedDate = value;
                OnPropertyChanged(nameof(AddedDate));
            }
        }

        public bool IsActive
        {
            get => Model.IsActive;
            set
            {
                Model.IsActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }

        public bool IsWarning
        {
            get => Model.IsWarning;
            set
            {
                if (Model.IsWarning != value)
                {
                    Model.IsWarning = value;
                    OnPropertyChanged(nameof(IsWarning));
                }
            }
        }

        public int Quantity
        {
            get => Model.Quantity;
            set
            {
                Model.Quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }

        private string _quantityText = string.Empty;
        public string QuantityText
        {
            get => _quantityText;
            set
            {
                if (SetProperty(ref _quantityText, value))
                    ValidateQuantity();
            }
        }

        private DateTime? _expirationDate;
        public DateTime? ExpirationDate
        {
            get => _expirationDate;
            set
            {
                if (SetProperty(ref _expirationDate, value))
                    ValidateExpirationDate();
            }
        }

        private string? _expirationWarningDaysText;
        public string? ExpirationWarningDaysText
        {
            get => _expirationWarningDaysText;
            set
            {
                if (SetProperty(ref _expirationWarningDaysText, value))
                    ValidateExpirationWarningDays();
            }
        }

        public StockItemViewModel(InfoStockDTO model)
        {
            Model = model;
            QuantityText = model.Quantity.ToString(CultureInfo.InvariantCulture);
            ExpirationDate = model.ExpirationDate;
            ExpirationWarningDaysText = model.ExpirationWarningDays?.ToString(CultureInfo.InvariantCulture);
        }

        private void ValidateQuantity()
        {
            ClearErrors(nameof(QuantityText));

            if (string.IsNullOrWhiteSpace(QuantityText))
            {
                AddError(nameof(QuantityText), GetLocalizedString("ValidateCannotBeEmptyMessage"));
            }
            else if (!ValidDigitsRegex.IsMatch(QuantityText))
            {
                AddError(nameof(QuantityText), GetLocalizedString("ValidateWholeNumberMessage"));
            }
            else if (int.Parse(QuantityText) < 0)
            {
                AddError(nameof(QuantityText), GetLocalizedString("ValidateCannotBeNegativeMessage"));
            }
            else if (int.Parse(QuantityText) == 0)
            {
                AddError(nameof(QuantityText), GetLocalizedString("ValidateCannotBeZeroMessage"));
            }
        }

        private void ValidateExpirationDate()
        {
            ClearErrors(nameof(ExpirationDate));

            if (!ExpirationDate.HasValue)
                return;

            const int minExpirationDays = 1;
            if (ExpirationDate.HasValue && ExpirationDate.Value.Date < DateTime.Today.AddDays(minExpirationDays))
            {
                AddError(nameof(ExpirationDate), string.Format(GetLocalizedString("ValidateExpirationDateLeastDaysMessage"), minExpirationDays));
            }
        }

        private void ValidateExpirationWarningDays()
        {
            ClearErrors(nameof(ExpirationWarningDaysText));

            if (string.IsNullOrWhiteSpace(ExpirationWarningDaysText))
            {
                return;
            }
            if (!ValidDigitsRegex.IsMatch(ExpirationWarningDaysText))
            {
                AddError(nameof(ExpirationWarningDaysText), GetLocalizedString("ValidateWholeNumberMessage"));
            }
            else if (int.Parse(ExpirationWarningDaysText) < 0)
            {
                AddError(nameof(ExpirationWarningDaysText), GetLocalizedString("ValidateCannotBeNegativeMessage"));
            }
        }

        public void Validate()
        {
            ValidateQuantity();
            ValidateExpirationDate();
            ValidateExpirationWarningDays();
        }

        public bool TryCommit()
        {
            if (!int.TryParse(QuantityText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var quantity))
                return false;
            Model.Quantity = quantity;

            Model.ExpirationDate = ExpirationDate;

            if (string.IsNullOrWhiteSpace(ExpirationWarningDaysText))
            {
                Model.ExpirationWarningDays = null;
            }
            else if (int.TryParse(ExpirationWarningDaysText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var days))
            {
                Model.ExpirationWarningDays = days;
            }
            else
            {
                return false;
            }

            return true;
        }

    }
}