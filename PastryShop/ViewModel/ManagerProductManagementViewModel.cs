using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using PastryShop.Command;
using PastryShop.Data.DTO;
using PastryShop.Service;
using PastryShop.View;
using PastryShop.Enum;
using PastryShop.Utility;
using PastryShop.View.Dialog;
using PastryShop.ViewModel.Dialog;

namespace PastryShop.ViewModel
{
    public partial class ManagerProductManagementViewModel : ValidatableBaseViewModel
    {
        private readonly IProductService _productService;
        private readonly IStockService _stockService;
        private readonly int _userId;

        private ObservableCollection<InfoProductBasicDTO> _allProducts = [];
        private ObservableCollection<InfoProductBasicDTO> _filteredProducts = [];

        public ObservableCollection<InfoProductBasicDTO> Products
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

        private string _selectedFilter = "ProductManagementAllProductsKey";
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

        private string _idColumnHeader = string.Empty;
        public string IDColumnHeader
        {
            get => _idColumnHeader;
            set { _idColumnHeader = value; OnPropertyChanged(); }
        }

        private string _nameColumnHeader = string.Empty;
        public string NameColumnHeader
        {
            get => _nameColumnHeader;
            set { _nameColumnHeader = value; OnPropertyChanged(); }
        }

        private string _productTypeColumnHeader = string.Empty;
        public string ProductTypeColumnHeader
        {
            get => _productTypeColumnHeader;
            set { _productTypeColumnHeader = value; OnPropertyChanged(); }
        }

        private string _quantityColumnHeader = string.Empty;
        public string QuantityColumnHeader
        {
            get => _quantityColumnHeader;
            set { _quantityColumnHeader = value; OnPropertyChanged(); }
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

        public ObservableCollection<string> FilterOptions { get; } = ["ProductManagementAllProductsKey", "ProductManagementBakeryKey", "ProductManagementCakeKey",
                "ProductManagementPastryKey", "ProductManagementSweetKey", "ProductManagementDrinkKey", "ProductManagementAccessoryKey"];

        public ObservableCollection<FilterOption> AdvancedFilterOptions { get; } =
            [
                new() { Name = "ProductManagementDiscountedKey" },
                new() { Name = "ProductManagementFeaturedKey" },
                new() { Name = "ProductManagementAvailableKey" }
            ];

        public ICommand AddNewProductCommand { get; }
        public ICommand EditProductDataCommand { get; }
        public ICommand EditProductProfileCommand { get; }
        public ICommand ChangeProductAvailabilityCommand { get; }
        public ICommand ShowProductDetailsCommand { get; }
        public ICommand ManagementStockCommand { get; }

        public ManagerProductManagementViewModel(int userId, IProductService productService, IStockService stockService)
        {
            _userId = userId;
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));

            SelectedLanguage = UserSession.GetCurrentUser().Language;
            UpdateColumnHeaders();

            AddNewProductCommand = new AsyncRelayCommand(_ => ShowAddProductDialog());
            ShowProductDetailsCommand = new AsyncRelayCommand(ShowProductDetailsDialog);
            EditProductDataCommand = new AsyncRelayCommand(ShowEditProductDataDialog);
            EditProductProfileCommand = new AsyncRelayCommand(ShowEditProductProfileDialog);
            ChangeProductAvailabilityCommand = new AsyncRelayCommand(ChangeProductAvailability);
            ManagementStockCommand = new AsyncRelayCommand(ShowStockDialog);

            foreach (var filterOption in AdvancedFilterOptions)
            {
                filterOption.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(FilterOption.IsSelected))
                    {
                        FilterProducts();
                    }
                };
            }

            _ = InitializeAsync();
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
                _allProducts = new ObservableCollection<InfoProductBasicDTO>(products);
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

            if (_selectedFilter == "ProductManagementBakeryKey")
            {
                filtered = new ObservableCollection<InfoProductBasicDTO>(_allProducts.Where(p => p.ProductType == ProductType.Food && p.FoodType == FoodType.Bakery));
            }
            else if (_selectedFilter == "ProductManagementCakeKey")
            {
                filtered = new ObservableCollection<InfoProductBasicDTO>(_allProducts.Where(p => p.ProductType == ProductType.Food && p.FoodType == FoodType.Cake));
            }
            else if (_selectedFilter == "ProductManagementPastryKey")
            {
                filtered = new ObservableCollection<InfoProductBasicDTO>(_allProducts.Where(p => p.ProductType == ProductType.Food && p.FoodType == FoodType.Pastry));
            }
            else if (_selectedFilter == "ProductManagementSweetKey")
            {
                filtered = new ObservableCollection<InfoProductBasicDTO>(_allProducts.Where(p => p.ProductType == ProductType.Food && p.FoodType == FoodType.Sweet));
            }
            else if (_selectedFilter == "ProductManagementDrinkKey")
            {
                filtered = new ObservableCollection<InfoProductBasicDTO>(_allProducts.Where(p => p.ProductType == ProductType.Drink));
            }
            else if (_selectedFilter == "ProductManagementAccessoryKey")
            {
                filtered = new ObservableCollection<InfoProductBasicDTO>(_allProducts.Where(p => p.ProductType == ProductType.Accessory));
            }

            if (AdvancedFilterOptions.FirstOrDefault(o => o.Name == "ProductManagementDiscountedKey" && o.IsSelected) != null)
            {
                filtered = new ObservableCollection<InfoProductBasicDTO>(filtered.Where(p => p.Discount > 0));
            }

            if (AdvancedFilterOptions.FirstOrDefault(o => o.Name == "ProductManagementFeaturedKey" && o.IsSelected) != null)
            {
                filtered = new ObservableCollection<InfoProductBasicDTO>(filtered.Where(p => p.IsFeatured));
            }

            if (AdvancedFilterOptions.FirstOrDefault(o => o.Name == "ProductManagementAvailableKey" && o.IsSelected) != null)
            {
                filtered = new ObservableCollection<InfoProductBasicDTO>(filtered.Where(p => p.IsAvailable));
            }

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                filtered = new ObservableCollection<InfoProductBasicDTO>(
                    filtered.Where(p =>
                        p.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                        p.ProductType.ToString().Contains(_searchText, StringComparison.OrdinalIgnoreCase)));
            }

            Products = filtered;
        }

        private async Task ShowAddProductDialog()
        {
            var dialog = new ManagerAddProductDialog(_productService);

            if (dialog.ShowDialog() == true)
            {
                await LoadProducts();
            }
        }

        private async Task ShowProductDetailsDialog(object? parameter)
        {
            if (parameter is InfoProductBasicDTO product)
            {
                try
                {
                    var productDetails = await _productService.GetProductDetailsById(product.Id);

                    if (productDetails == null)
                    {
                        ShowErrorDialog(GetLocalizedString("DialogInfoProductNotFoundMessage"));
                        return;
                    }

                    var dialog = new ManagerShowProductDialog(productDetails);
                    dialog.ShowDialog();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Greška prilikom učitavanja detalja proizvoda: {ex.Message}", ex);
                }
            }
            else
            {
                Logger.LogError("Neispravan unos proizvoda.", new ArgumentException("Neispravan unos proizvoda"));
            }
        }

        private async Task ShowEditProductDataDialog(object? parameter)
        {
            if (parameter is InfoProductBasicDTO product)
            {
                if (!product.IsAvailable)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningProductCurrentlyUnavailableMessage"));
                    return;
                }

                try
                {
                    var editProductDataDto = await _productService.GetProductDataForEditById(product.Id);

                    if (editProductDataDto == null)
                    {
                        ShowErrorDialog(GetLocalizedString("DialogInfoProductNotFoundMessage"));
                        return;
                    }

                    var dialogViewModel = new ManagerEditProductDataDialogViewModel(editProductDataDto, _productService);

                    var dialog = new ManagerEditProductDataDialog
                    {
                        DataContext = dialogViewModel
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        await LoadProducts();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Greška prilikom ažuriranja podataka: {ex.Message}", ex);
                }
            }
            else
            {
                Logger.LogError("Neispravan unos proizvoda.", new ArgumentException("Neispravan unos proizvoda"));
            }
        }

        private async Task ShowEditProductProfileDialog(object? parameter)
        {
            if (parameter is InfoProductBasicDTO product)
            {
                if (!product.IsAvailable)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningProductCurrentlyUnavailableMessage"));
                    return;
                }

                try
                {
                    var editProductProfileDto = await _productService.GetProductProfileForEditById(product.Id);
                    if (editProductProfileDto == null)
                    {
                        ShowErrorDialog(GetLocalizedString("DialogInfoProductNotFoundMessage"));
                        return;
                    }

                    var dialogViewModel = new ManagerEditProductProfileDialogViewModel(editProductProfileDto, _productService);

                    var dialog = new ManagerEditProductProfileDialog
                    {
                        DataContext = dialogViewModel
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        await LoadProducts();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Greška prilikom ažuriranja podataka: {ex.Message}", ex);
                }
            }
            else
            {
                Logger.LogError("Neispravan unos proizvoda.", new ArgumentException("Neispravan unos proizvoda"));
            }
        }

        private async Task ChangeProductAvailability(object? parameter)
        {
            if (parameter is InfoProductBasicDTO product)
            {
                bool newAvailability = !product.IsAvailable;

                var availKey = newAvailability
                   ? "DialogInfoProductChangeAvailabilityAvailableMessage"
                   : "DialogInfoProductChangeAvailabilityUnvailableMessage";
                var availText = GetLocalizedString(availKey);
                var messageTemplate = GetLocalizedString("DialogInfoProductChangeAvailabilityMessage");
                var message = string.Format(messageTemplate, availText);

                var dialogViewModel = new BaseChoiceDialogViewModel
                {
                    Message = message
                };

                var dialog = new BaseChoiceDialog
                {
                    DataContext = dialogViewModel
                };

                if (dialog.ShowDialog() == true)
                {
                    await _productService.ChangeProductAvailability(product.Id, newAvailability);
                    await LoadProducts();
                    ShowInfoDialog(GetLocalizedString("DialogInfoProductChangeAvailabilitySuccessfullyMessage"));
                }
            }
        }

        private async Task ShowStockDialog(object? parameter)
        {
            if (parameter is InfoProductBasicDTO product)
            {
                if (!product.IsAvailable)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningProductCurrentlyUnavailableMessage"));
                    return;
                }

                try
                {
                    var dialogViewModel = new ManagerStockManagementDialogViewModel(_userId, product.Id, product.Name, _stockService);

                    var dialog = new ManagerStockManagementDialog
                    {
                        DataContext = dialogViewModel
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        await LoadProducts();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Greška prilikom prikaza podataka: {ex.Message}", ex);
                }
            }
            else
            {
                Logger.LogError("Neispravan unos proizvoda.", new ArgumentException("Neispravan unos proizvoda"));
            }
        }

        private void UpdateColumnHeaders()
        {
            if (SelectedLanguage == Language.English)
            {
                IDColumnHeader = "ID";
                NameColumnHeader = "Name";
                ProductTypeColumnHeader = "Product Type";
                QuantityColumnHeader = "Quantity";
                ActionsColumnHeader = "Actions";
            }
            else if (SelectedLanguage == Language.Serbian)
            {
                IDColumnHeader = "ID";
                NameColumnHeader = "Naziv";
                ProductTypeColumnHeader = "Tip Proizvoda";
                QuantityColumnHeader = "Količina";
                ActionsColumnHeader = "Akcije";
            }
        }

    }
}