using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using PastryShop.Command;
using PastryShop.Data.DTO;
using PastryShop.Enum;
using PastryShop.Service;
using PastryShop.Utility;

namespace PastryShop.ViewModel
{
    public partial class ManagerStockManagementDialogViewModel : ValidatableBaseViewModel
    {
        private readonly IStockService _stockService;

        private readonly int _userId;
        private readonly int _productId;

        public ObservableCollection<StockItemViewModel> Stocks { get; set; } = [];
        private readonly Dictionary<int, InfoStockDTO> _originalStocks = [];

        private ICollectionView _stocksView;
        public ICollectionView StocksView
        {
            get => _stocksView;
            private set
            {
                _stocksView = value;
                OnPropertyChanged(nameof(StocksView));
            }
        }

        private StockItemViewModel? _selectedStock;
        public StockItemViewModel? SelectedStock
        {
            get => _selectedStock;
            set
            {
                _selectedStock = value;
                OnPropertyChanged(nameof(SelectedStock));
                OnPropertyChanged(nameof(DetailMessage));
            }
        }

        private bool _isActive = true;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (SetProperty(ref _isActive, value))
                {
                    StocksView.Refresh();
                    OnPropertyChanged(nameof(TotalQuantity));
                    OnPropertyChanged(nameof(DetailMessage));
                }
            }
        }

        private string _productName;
        public string ProductName
        {
            get => _productName;
            set
            {
                if (_productName != value)
                {
                    _productName = value;
                    OnPropertyChanged(nameof(ProductName));
                }
            }
        }

        public int TotalQuantity => Stocks.Where(stock => stock.IsActive).Sum(stock => stock.Quantity);

        public string DetailMessage
        {
            get
            {
                if (!StocksView.Cast<StockItemViewModel>().Any())
                    return GetLocalizedString("ProductManagementStockNoStockAvailableString");
                if (SelectedStock == null)
                    return GetLocalizedString("ProductManagementStockStockOverviewString");
                return string.Empty;
            }
        }

        public ICommand ToggleActiveCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public ICommand DiscardChangesCommand { get; }
        public ICommand DeleteStockCommand { get; }
        public ICommand AddNewStockCommand { get; }
        public ICommand CloseCommand { get; } =
            new AsyncRelayCommand(parameter =>
            {
                if (parameter is Window window)
                {
                    window.DialogResult = true;
                }
                return Task.CompletedTask;
            });

        public ManagerStockManagementDialogViewModel(int userId, int productId, string productName, IStockService stockService)
        {
            _userId = userId;
            _productId = productId;
            _productName = productName;
            _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));

            _stocksView = CollectionViewSource.GetDefaultView(Stocks);
            StocksView = _stocksView;

            StocksView.Filter = FilterActiveStocks;

            ToggleActiveCommand = new AsyncRelayCommand(_ => ToggleActive());
            SaveChangesCommand = new AsyncRelayCommand(_ => SaveChanges());
            DiscardChangesCommand = new AsyncRelayCommand(_ => DiscardChanges());
            DeleteStockCommand = new AsyncRelayCommand(stock => DeleteStock(stock as StockItemViewModel));
            AddNewStockCommand = new AsyncRelayCommand(_ => AddNewStock());

            _ = InitializeStocksAsync();
        }

        private bool FilterActiveStocks(object item)
        {
            if (item is StockItemViewModel vm)
                return vm.IsActive == IsActive;
            return false;
        }

        private async Task InitializeStocksAsync()
        {
            var stockEntries = await _stockService.GetAllStocksByProductId(_productId);
            if (stockEntries == null)
                return;

            Stocks.Clear();
            _originalStocks.Clear();

            foreach (var stock in stockEntries)
            {
                var vm = new StockItemViewModel(stock);

                Stocks.Add(vm);

                if (stock.Id != 0)
                    _originalStocks[stock.Id] = CloneStock(stock);
            }

            StocksView.Refresh();
            OnPropertyChanged(nameof(TotalQuantity));
            OnPropertyChanged(nameof(DetailMessage));

            SelectedStock = null;
        }

        private void ValidateAll()
        {
            ClearAllErrors();

            foreach (var itemVm in Stocks.Where(vm => vm.IsActive))
            {
                itemVm.Validate();
                if (itemVm.HasErrors)
                {
                    foreach (var prop in new[] { nameof(itemVm.QuantityText), nameof(itemVm.ExpirationDate), nameof(itemVm.ExpirationWarningDaysText) })
                    {
                        foreach (string err in itemVm.GetErrors(prop))
                            AddError(prop, err);
                    }
                }
            }
        }

        private Task ToggleActive()
        {
            IsActive = !IsActive;
            return Task.CompletedTask;
        }

        private async Task SaveChanges()
        {
            var dialog = CreateChoiceDialog(GetLocalizedString("DialogChoiceStockSaveChangesMessage"));

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    ValidateAll();

                    if (HasErrors || Stocks.Where(vm => vm.IsActive).Any(vm => vm.HasErrors))
                    {
                        ShowErrorDialog(GetLocalizedString("DialogWarningInvalidFieldValuesMessage"));
                        return;
                    }

                    foreach (var vm in Stocks)
                        vm.TryCommit();

                    foreach (var vm in Stocks)
                    {
                        var model = vm.Model;
                        if (model.Id == 0)
                        {
                            var newId = await _stockService.CreateStock(ConvertToAddStockDTO(vm));
                            vm.Id = newId;
                            _originalStocks[newId] = CloneStock(model);
                        }
                        else if (_originalStocks.TryGetValue(model.Id, out var original) && !AreStocksEqual(model, original))
                        {
                            await _stockService.UpdateStockData(ConvertToEditStockDTO(vm));
                            _originalStocks[model.Id] = CloneStock(model);
                        }
                    }

                    UpdateWarnings();
                    StocksView.Refresh();
                    OnPropertyChanged(nameof(TotalQuantity));
                    ShowInfoDialog(GetLocalizedString("DialogInfoStockSuccessfullySavedMessage"));
                }
                catch (Exception ex)
                {
                    Logger.LogError("Greška prilikom čuvanja promjena.", ex);
                }
            }
        }

        private async Task DiscardChanges()
        {
            var dialog = CreateChoiceDialog(GetLocalizedString("DialogChoiceStockDiscardChangesMessage"));

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var refreshedStocks = await _stockService.GetAllStocksByProductId(Stocks.FirstOrDefault()?.ProductId ?? _productId);
                    Stocks.Clear();

                    foreach (var stock in refreshedStocks)
                    {
                        Stocks.Add(new StockItemViewModel(stock));
                    }

                    StocksView.Refresh();
                    OnPropertyChanged(nameof(TotalQuantity));
                    ShowInfoDialog(GetLocalizedString("DialogInfoStockChangesDiscardedMessage"));
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Greška prilikom odbacivanja promena: {ex.Message}", ex);
                }
            }
        }

        private async Task DeleteStock(StockItemViewModel? stock)
        {
            if (stock == null)
            {
                ShowErrorDialog(GetLocalizedString("DialogWarningStockMustSelectItemMessage"));
                return;
            }

            var dialog = CreateChoiceDialog(string.Format(GetLocalizedString("DialogChoiceStockDeleteItemMessage"), stock.Id));

            if (dialog.ShowDialog() == true)
            {
                Stocks.Remove(stock);
                SelectedStock = Stocks.FirstOrDefault();
                StocksView.Refresh();

                OnPropertyChanged(nameof(TotalQuantity));
                OnPropertyChanged(nameof(DetailMessage));

                ShowInfoDialog(GetLocalizedString("DialogInfoStockDeleteSuccessfulMessage"));

                await Task.CompletedTask;
            }
        }

        private async Task AddNewStock()
        {
            var newStock = new InfoStockDTO
            {
                Id = 0,
                ProductId = _productId,
                Quantity = 0,
                AddedDate = DateTime.Now,
                IsActive = true,
                IsWarning = false
            };

            var vm = new StockItemViewModel(newStock);
            Stocks.Add(vm);
            SelectedStock = vm;
            StocksView.Refresh();

            OnPropertyChanged(nameof(TotalQuantity));
            OnPropertyChanged(nameof(DetailMessage));

            await Task.CompletedTask;
        }

        private AddStockDTO ConvertToAddStockDTO(StockItemViewModel stock) => new()
        {
            ProductId = stock.Model.ProductId,
            UserId = _userId,
            Quantity = stock.Model.Quantity,
            ExpirationDate = stock.Model.ExpirationDate,
            ExpirationWarningDays = stock.Model.ExpirationWarningDays ?? 0
        };

        private EditStockDTO ConvertToEditStockDTO(StockItemViewModel vm)
        {
            var dto = vm.Model;

            bool isWarning = false;
            if (dto.ExpirationDate.HasValue && dto.ExpirationWarningDays.HasValue)
            {
                var daysLeft = (dto.ExpirationDate.Value.Date - DateTime.Today).Days;
                isWarning = daysLeft >= 0 && daysLeft <= dto.ExpirationWarningDays.Value;
            }

            return new EditStockDTO
            {
                Id = dto.Id,
                UserId = _userId,
                Quantity = dto.Quantity,
                ExpirationDate = dto.ExpirationDate,
                ExpirationWarningDays = dto.ExpirationWarningDays ?? 0,
                IsActive = dto.IsActive,
                IsWarning = isWarning,
                TransactionType = TransactionType.Adjustment
            };
        }

        private static InfoStockDTO CloneStock(InfoStockDTO stock)
        {
            return new InfoStockDTO
            {
                Id = stock.Id,
                ProductId = stock.ProductId,
                Quantity = stock.Quantity,
                ExpirationDate = stock.ExpirationDate,
                ExpirationWarningDays = stock.ExpirationWarningDays,
                IsActive = stock.IsActive,
                IsWarning = stock.IsWarning,
                AddedDate = stock.AddedDate
            };
        }

        private static bool AreStocksEqual(InfoStockDTO a, InfoStockDTO b)
        {
            return a.Quantity == b.Quantity &&
                   a.ExpirationDate == b.ExpirationDate &&
                   a.ExpirationWarningDays == b.ExpirationWarningDays &&
                   a.IsActive == b.IsActive &&
                   a.IsWarning == b.IsWarning &&
                   a.AddedDate == b.AddedDate;
        }

        private void UpdateWarnings()
        {
            foreach (var vm in Stocks)
            {
                if (vm.ExpirationDate.HasValue
                    && vm.Model.ExpirationWarningDays.HasValue
                    && vm.IsActive)
                {
                    int daysLeft = (vm.ExpirationDate.Value.Date - DateTime.Today).Days;
                    bool warning = daysLeft >= 0
                                   && daysLeft <= vm.Model.ExpirationWarningDays.Value;
                    vm.IsWarning = warning;
                }
                else
                {
                    vm.IsWarning = false;
                }
            }

            StocksView.Refresh();
            OnPropertyChanged(nameof(Stocks));
        }

    }
}