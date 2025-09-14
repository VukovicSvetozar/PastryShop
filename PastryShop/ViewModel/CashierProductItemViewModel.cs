using PastryShop.Data.DTO;
using PastryShop.Enum;

namespace PastryShop.ViewModel
{
    public partial class CashierProductItemViewModel(InfoProductBasicDTO product) : BaseViewModel
    {
        public InfoProductBasicDTO Product { get; } = product;

        public enum AddToCartError { None, OutOfStock, NotAvailable, AlreadyInCart }

        private AddToCartError _addError = AddToCartError.None;
        public AddToCartError AddError
        {
            get => _addError;
            set
            {
                if (SetProperty(ref _addError, value))
                    OnPropertyChanged(nameof(IsErrorPopupOpen));
            }
        }

        public bool IsErrorPopupOpen => AddError != AddToCartError.None;

        public string Name => Product.Name;
        public string? ImagePath => Product.ImagePath;
        public string Description => Product.Description;
        public decimal Price => Product.Price;
        public int Quantity => Product.Quantity;
        public bool IsAvailable => Product.IsAvailable;
        public int Id => Product.Id;
        public ProductType ProductType => Product.ProductType;
        public FoodType? FoodType => Product.FoodType;
        public decimal Discount => Product.Discount;
        public bool IsFeatured => Product.IsFeatured;

    }
}