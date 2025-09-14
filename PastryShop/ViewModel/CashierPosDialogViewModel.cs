using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using PastryShop.Command;
using PastryShop.Enum;
using PastryShop.Service;
using PastryShop.View;
using static PastryShop.ViewModel.CashierProductItemViewModel;

namespace PastryShop.ViewModel
{
    public partial class CashierPosDialogViewModel : ValidatableBaseViewModel
    {
        private readonly int _userId;
        private readonly IProductService _productService;
        private readonly IStockService _stockService;
        private readonly ICartService _cartService;

        private ObservableCollection<CashierProductItemViewModel> _allProducts = [];
        private ObservableCollection<CashierProductItemViewModel> _filteredProducts = [];

        public ObservableCollection<CashierProductItemViewModel> Products
        {
            get => _filteredProducts;
            set => SetProperty(ref _filteredProducts, value);
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterProducts();
                }
            }
        }

        private string _selectedFilter = "PosManagementAllProductsKey";
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (SetProperty(ref _selectedFilter, value))
                {
                    FilterProducts();
                }
            }
        }

        public ObservableCollection<string> FilterOptions { get; } = ["PosManagementAllProductsKey", "PosManagementBakeryKey", "PosManagementCakeKey",
             "PosManagementPastryKey", "PosManagementSweetKey", "PosManagementDrinkKey", "PosManagementAccessoryKey"];

        private bool _isAvailable = true;
        public bool IsAvailable
        {
            get => _isAvailable;
            set
            {
                if (SetProperty(ref _isAvailable, value))
                {
                    FilterProducts();
                }
            }
        }

        private bool _isFeatured;
        public bool IsFeatured
        {
            get => _isFeatured;
            set
            {
                if (SetProperty(ref _isFeatured, value))
                {
                    FilterProducts();
                }
            }
        }

        private bool _isDiscounted;
        public bool IsDiscounted
        {
            get => _isDiscounted;
            set
            {
                if (SetProperty(ref _isDiscounted, value))
                {
                    FilterProducts();
                }
            }
        }

        private object _currentView = new();
        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public ICommand AddToCartCommand { get; }
        public ICommand ToggleAvailableCommand { get; }
        public ICommand ToggleFeaturedCommand { get; }
        public ICommand ToggleDiscountedCommand { get; }

        public CashierPosDialogViewModel(int userId, IProductService productService, IStockService stockService, ICartService cartService)
        {
            _userId = userId;
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));

            AddToCartCommand = new AsyncRelayCommand(AddToCart);
            ToggleAvailableCommand = new RelayCommand(_ => IsAvailable = !IsAvailable);
            ToggleFeaturedCommand = new RelayCommand(_ => IsFeatured = !IsFeatured);
            ToggleDiscountedCommand = new RelayCommand(_ => IsDiscounted = !IsDiscounted);

            _ = InitializeAsync();

            _ = ShowCartView();
        }

        private async Task InitializeAsync()
        {
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsBasicInfo();
                _allProducts = new ObservableCollection<CashierProductItemViewModel>(products.Select(p => new CashierProductItemViewModel(p)));
                FilterProducts();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Greška tokom učitavanja proizvoda.", ex);
            }
        }

        private void FilterProducts()
        {
            var filtered = _allProducts;

            if (_selectedFilter == "PosManagementBakeryKey")
            {
                filtered = new ObservableCollection<CashierProductItemViewModel>(_allProducts.Where(p => p.ProductType == ProductType.Food && p.FoodType == FoodType.Bakery));
            }
            else if (_selectedFilter == "PosManagementCakeKey")
            {
                filtered = new ObservableCollection<CashierProductItemViewModel>(_allProducts.Where(p => p.ProductType == ProductType.Food && p.FoodType == FoodType.Cake));
            }
            else if (_selectedFilter == "PosManagementPastryKey")
            {
                filtered = new ObservableCollection<CashierProductItemViewModel>(_allProducts.Where(p => p.ProductType == ProductType.Food && p.FoodType == FoodType.Pastry));
            }
            else if (_selectedFilter == "PosManagementSweetKey")
            {
                filtered = new ObservableCollection<CashierProductItemViewModel>(_allProducts.Where(p => p.ProductType == ProductType.Food && p.FoodType == FoodType.Sweet));
            }
            else if (_selectedFilter == "PosManagementDrinkKey")
            {
                filtered = new ObservableCollection<CashierProductItemViewModel>(_allProducts.Where(p => p.ProductType == ProductType.Drink));
            }
            else if (_selectedFilter == "PosManagementAccessoryKey")
            {
                filtered = new ObservableCollection<CashierProductItemViewModel>(_allProducts.Where(p => p.ProductType == ProductType.Accessory));
            }

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                filtered = new ObservableCollection<CashierProductItemViewModel>(
                    filtered.Where(p =>
                        p.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase)));
            }

            if (IsDiscounted)
            {
                filtered = new ObservableCollection<CashierProductItemViewModel>(
                    filtered.Where(p => p.Discount > 0));
            }

            if (IsFeatured)
            {
                filtered = new ObservableCollection<CashierProductItemViewModel>(
                    filtered.Where(p => p.IsFeatured));
            }

            if (IsAvailable)
            {
                filtered = new ObservableCollection<CashierProductItemViewModel>(
                    filtered.Where(p => p.IsAvailable));
            }

            Products = filtered;
        }

        private async Task AddToCart(object? parameter)
        {
            if (parameter is CashierProductItemViewModel productVM)
            {
                if (!productVM.IsAvailable)
                {
                    productVM.AddError = AddToCartError.NotAvailable;
                    await Task.Delay(4000);
                    productVM.AddError = AddToCartError.None;
                    return;
                }

                if (productVM.Quantity == 0)
                {
                    productVM.AddError = AddToCartError.OutOfStock;
                    await Task.Delay(4000);
                    productVM.AddError = AddToCartError.None;
                    return;
                }

                bool already = _cartService.GetProducts().Any(p => p.Id == productVM.Id);
                if (already)
                {
                    productVM.AddError = AddToCartError.AlreadyInCart;
                    await Task.Delay(4000);
                    productVM.AddError = AddToCartError.None;
                    return;
                }

                await Task.Run(() => _cartService.AddProduct(productVM.Product));
            }
        }

        private async Task ShowCartView()
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var cartViewModel = new CashierCartDialogViewModel(_userId, _cartService, _productService, _stockService)
                    {
                        OnCartCleared = async () => await LoadProducts()
                    };
                    CurrentView = new CashierCartDialog { DataContext = cartViewModel };
                });
            });
        }

    }
}