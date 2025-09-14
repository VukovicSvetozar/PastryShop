using System.Windows.Input;
using PastryShop.Command;
using PastryShop.Data.DTO;

namespace PastryShop.ViewModel
{
    public partial class CashierCartItemViewModel : BaseViewModel
    {
        public InfoProductBasicDTO Product { get; }

        public string Name => Product.Name;
        public decimal Price => Product.Price;
        public string ImagePath => Product?.ImagePath ?? string.Empty;

        private int _quantityProduct = 1;
        public int QuantityProduct
        {
            get => _quantityProduct;
            set
            {
                if (_quantityProduct != value)
                {
                    _quantityProduct = value;
                    OnPropertyChanged(nameof(QuantityProduct));
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        private bool _isMaxQuantityWarningVisible;
        public bool IsMaxQuantityWarningVisible
        {
            get => _isMaxQuantityWarningVisible;
            set
            {
                if (_isMaxQuantityWarningVisible != value)
                {
                    _isMaxQuantityWarningVisible = value;
                    OnPropertyChanged(nameof(IsMaxQuantityWarningVisible));
                }
            }
        }

        public decimal TotalPrice => Price * QuantityProduct;

        public ICommand IncreaseCommand { get; }
        public ICommand DecreaseCommand { get; }

        public CashierCartItemViewModel(InfoProductBasicDTO product)
        {
            Product = product;
            IncreaseCommand = new AsyncRelayCommand(_ => Task.Run(IncreaseQuantity));
            DecreaseCommand = new AsyncRelayCommand(_ => Task.Run(DecreaseQuantity));
        }

        private async void IncreaseQuantity()
        {
            if (QuantityProduct < Product.Quantity)
            {
                QuantityProduct++;
                IsMaxQuantityWarningVisible = false;
            }
            else
            {
                IsMaxQuantityWarningVisible = true;
                await Task.Delay(4000);
                IsMaxQuantityWarningVisible = false;
            }
        }

        private void DecreaseQuantity()
        {
            if (QuantityProduct > 1)
            {
                QuantityProduct--;
            }
        }

    }
}