using System.Windows;
using System.Windows.Input;
using System.Globalization;
using PastryShop.Service;
using PastryShop.Data.DTO;
using PastryShop.Command;
using PastryShop.Utility;

namespace PastryShop.ViewModel
{
    public partial class ManagerEditProductDataDialogViewModel : ValidatableBaseViewModel
    {
        private readonly IProductService _productService;

        public EditProductDataDTO ProductData { get; set; }

        private string? _priceText;
        public string? PriceText
        {
            get => _priceText;
            set
            {
                if (SetProperty(ref _priceText, value))
                {
                    ValidatePrice();
                }
            }
        }

        private string? _discountText;
        public string? DiscountText
        {
            get => _discountText;
            set
            {
                if (SetProperty(ref _discountText, value))
                {
                    ValidateDiscount();
                }
            }
        }

        public ICommand SaveCommand { get; }

        public ManagerEditProductDataDialogViewModel(EditProductDataDTO productData, IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            ProductData = productData ?? throw new ArgumentNullException(nameof(productData));

            PriceText = ProductData.Price.ToString(CultureInfo.InvariantCulture);
            DiscountText = ProductData.Discount.ToString(CultureInfo.InvariantCulture);

            SaveCommand = new AsyncRelayCommand(_ => SaveProductData());
        }

        public void Validate()
        {
            ValidatePrice();
            ValidateDiscount();
        }

        private void ValidatePrice()
        {
            ClearErrors(nameof(PriceText));

            if (string.IsNullOrWhiteSpace(PriceText))
            {
                AddError(nameof(PriceText), GetLocalizedString("ValidateCannotBeEmptyMessage"));
            }
            else if (!decimal.TryParse(PriceText, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsedPrice))
            {
                AddError(nameof(PriceText), GetLocalizedString("ValidateMustBeNumberMessage"));
            }
            else if (parsedPrice < 0)
            {
                AddError(nameof(PriceText), GetLocalizedString("ValidateCannotBeNegativeMessage"));
            }
        }

        private void ValidateDiscount()
        {
            ClearErrors(nameof(DiscountText));

            if (string.IsNullOrWhiteSpace(DiscountText))
            {
                AddError(nameof(DiscountText), GetLocalizedString("ValidateCannotBeEmptyMessage"));

            }
            else if (!decimal.TryParse(DiscountText, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsedDiscount))
            {
                AddError(nameof(DiscountText), GetLocalizedString("ValidateMustBeNumberMessage"));
            }
            else if (parsedDiscount < 0)
            {
                AddError(nameof(DiscountText), GetLocalizedString("ValidateCannotBeNegativeMessage"));
            }
            else if (parsedDiscount > 99)
            {
                AddError(nameof(DiscountText), GetLocalizedString("ValidateValueLessThanOneHundredMessage"));
            }
        }

        private async Task SaveProductData()
        {
            try
            {
                Validate();

                if (HasErrors)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningInvalidFieldValuesMessage"));
                    return;
                }

                ProductData.Price = decimal.Parse(PriceText!, CultureInfo.InvariantCulture);
                ProductData.Discount = decimal.Parse(DiscountText!, CultureInfo.InvariantCulture);

                await _productService.EditProductData(ProductData);

                ShowInfoDialog(GetLocalizedString("DialogInfoProductEditDataSuccessfullyMessage"));

                CloseWindow();

            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom ažuriranja: {ex.Message}", ex);
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