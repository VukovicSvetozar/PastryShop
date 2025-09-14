using System.Windows.Input;
using System.Collections.ObjectModel;
using PastryShop.Service;
using PastryShop.Data.DTO;
using PastryShop.Command;
using PastryShop.Enum;
using PastryShop.Utility;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;

namespace PastryShop.ViewModel
{
    public partial class ManagerReportDialogViewModel : ValidatableBaseViewModel
    {
        private readonly IProductService _productService;
        private readonly IStockService _stockService;

        private ObservableCollection<InfoProductTopSalesStatsDTO> _allProducts = [];
        public ObservableCollection<InfoProductTopSalesStatsDTO> Products
        {
            get => _allProducts;
            set => SetProperty(ref _allProducts, value);
        }

        private List<InfoWeeklyOrderStatsDTO> _userWeeklyStats = [];

        public ObservableCollection<ISeries> WeeklyOrdersAndItemsSeries { get; }
        public ObservableCollection<ISeries> WeeklyRevenueSeries { get; }
        public string[] WeeklyLabels { get; private set; }

        private Axis[] _xAxes = [];
        public Axis[] XAxes
        {
            get => _xAxes;
            private set => SetProperty(ref _xAxes, value);
        }

        private Axis[] _ordersYAxes = [];
        public Axis[] OrdersYAxes
        {
            get => _ordersYAxes;
            private set => SetProperty(ref _ordersYAxes, value);
        }

        private Axis[] _revenueYAxes = [];
        public Axis[] RevenueYAxes
        {
            get => _revenueYAxes;
            private set => SetProperty(ref _revenueYAxes, value);
        }

        private int _totalOrders;
        public int TotalOrders
        {
            get => _totalOrders;
            private set { _totalOrders = value; OnPropertyChanged(); }
        }

        private int _totalItems;
        public int TotalItems
        {
            get => _totalItems;
            private set { _totalItems = value; OnPropertyChanged(); }
        }

        private decimal _totalRevenue;
        public decimal TotalRevenue
        {
            get => _totalRevenue;
            private set { _totalRevenue = value; OnPropertyChanged(); }
        }

        private int _totalCompletedOrders;
        public int TotalCompletedOrders
        {
            get => _totalCompletedOrders;
            private set { _totalCompletedOrders = value; OnPropertyChanged(); }
        }

        private int _totalCancelledOrders;
        public int TotalCancelledOrders
        {
            get => _totalCancelledOrders;
            private set { _totalCancelledOrders = value; OnPropertyChanged(); }
        }

        private int _totalOnHoldOrders;
        public int TotalOnHoldOrders
        {
            get => _totalOnHoldOrders;
            private set { _totalOnHoldOrders = value; OnPropertyChanged(); }
        }

        private int _totalFoodItems;
        public int TotalFoodItems
        {
            get => _totalFoodItems;
            private set { _totalFoodItems = value; OnPropertyChanged(); }
        }

        private int _totalDrinkItems;
        public int TotalDrinkItems
        {
            get => _totalDrinkItems;
            private set { _totalDrinkItems = value; OnPropertyChanged(); }
        }

        private int _totalAccessoryItems;
        public int TotalAccessoryItems
        {
            get => _totalAccessoryItems;
            private set { _totalAccessoryItems = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> AllProductNames { get; } = [];

        private string? _selectedProductName;
        public string? SelectedProductName
        {
            get => _selectedProductName;
            set
            {
                if (SetProperty(ref _selectedProductName, value))
                {
                    ClearAllErrors();
                    ValidateSelectedProductName();
                }
            }
        }

        private ObservableCollection<InfoStockWarningDTO> _stockWarnings = [];
        public ObservableCollection<InfoStockWarningDTO> StockWarnings
        {
            get => _stockWarnings;
            set => SetProperty(ref _stockWarnings, value);
        }

        private ObservableCollection<InfoStockTransactionDTO> _stockTransactions = [];
        public ObservableCollection<InfoStockTransactionDTO> StockTransactions
        {
            get => _stockTransactions;
            set => SetProperty(ref _stockTransactions, value);
        }

        private ObservableCollection<InfoStockModificationDTO> _stockModifications = [];
        public ObservableCollection<InfoStockModificationDTO> StockModifications
        {
            get => _stockModifications;
            set => SetProperty(ref _stockModifications, value);
        }

        private ObservableCollection<InfoStockSummaryDTO> _stockSummaries = [];
        public ObservableCollection<InfoStockSummaryDTO> StockSummaries
        {
            get => _stockSummaries;
            set => SetProperty(ref _stockSummaries, value);
        }

        private DateTime? _filterProductDateFrom;
        public DateTime? FilterProductDateFrom
        {
            get => _filterProductDateFrom;
            set
            {
                if (SetProperty(ref _filterProductDateFrom, value))
                {
                    ValidateProductDates();
                }
            }
        }

        private DateTime? _filterProductDateTo;
        public DateTime? FilterProductDateTo
        {
            get => _filterProductDateTo;
            set
            {
                if (SetProperty(ref _filterProductDateTo, value))
                {
                    ValidateProductDates();
                }
            }
        }

        private DateTime? _filterStockDateFrom;
        public DateTime? FilterStockDateFrom
        {
            get => _filterStockDateFrom;
            set
            {
                if (SetProperty(ref _filterStockDateFrom, value))
                {
                    ValidateStockDates();
                }
            }
        }

        private DateTime? _filterStockDateTo;
        public DateTime? FilterStockDateTo
        {
            get => _filterStockDateTo;
            set
            {
                if (SetProperty(ref _filterStockDateTo, value))
                {
                    ValidateStockDates();
                }
            }
        }

        public ICommand SearchProductReportCommand { get; }
        public ICommand SearchStockReportCommand { get; }

        public ManagerReportDialogViewModel(IProductService productService, IStockService stockService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));

            SelectedLanguage = UserSession.GetCurrentUser().Language;
            UpdateColumnHeaders();

            SearchProductReportCommand = new AsyncRelayCommand(_ => SearchProductReport());
            SearchStockReportCommand = new AsyncRelayCommand(_ => SearchStockReport());

            WeeklyOrdersAndItemsSeries = [];
            WeeklyRevenueSeries = [];
            WeeklyLabels = [];

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadProductNames();
            await LoadWarnings();
            await LoadProductStats();
            BuildWeeklyOrdersItemsSeries();
            BuildWeeklyRevenueSeries();
        }

        private async Task LoadProductNames()
        {
            AllProductNames.Clear();
            var names = await _productService.GetAllProductNames();
            foreach (var n in names)
                AllProductNames.Add(n);
        }

        private async Task LoadWarnings()
        {
            var stockWarningsList = await _stockService.GetWarningStocks();
            StockWarnings = new ObservableCollection<InfoStockWarningDTO>(stockWarningsList);
        }

        private async Task LoadProductStats()
        {
            var products = await _productService.GetBestSellersProducts(6);
            _allProducts = new ObservableCollection<InfoProductTopSalesStatsDTO>(products);
            Products = new ObservableCollection<InfoProductTopSalesStatsDTO>(products);

            var orderStats = await _productService.GetOrderStats(FilterProductDateFrom, FilterProductDateTo);

            TotalOrders = orderStats.TotalOrders;
            TotalItems = orderStats.TotalItems;
            TotalRevenue = orderStats.TotalRevenue;
            TotalOrders = orderStats.TotalOrders;
            TotalCompletedOrders = orderStats.TotalCompletedOrders;
            TotalCancelledOrders = orderStats.TotalCancelledOrders;
            TotalOnHoldOrders = orderStats.TotalOnHoldOrders;
            TotalItems = orderStats.TotalItems;
            TotalRevenue = orderStats.TotalRevenue;
            TotalFoodItems = orderStats.FoodItems;
            TotalDrinkItems = orderStats.DrinkItems;
            TotalAccessoryItems = orderStats.AccessoryItems;

            _userWeeklyStats = await _productService.GetUserWeeklyStats(null, 6);
        }

        private void BuildWeeklyOrdersItemsSeries()
        {
            WeeklyOrdersAndItemsSeries.Clear();

            WeeklyLabels = _userWeeklyStats
                .Select(s => s.WeekStart.ToString("dd MMM"))
                .ToArray();
            OnPropertyChanged(nameof(WeeklyLabels));

            var ordersStrokePaint = GetPaint("LineOrdersStrokePaint");
            var itemsStrokePaint = GetPaint("LineItemsStrokePaint");
            var labelPaint = GetPaint("AxisLabelsPaint");

            XAxes =
            [
                new Axis
                {
                    Labels = WeeklyLabels,
                    LabelsPaint = labelPaint
                }
            ];

            OrdersYAxes =
            [
                new Axis
                {
                    Labeler = value => value.ToString(),
                    LabelsPaint = labelPaint
                }
            ];

            WeeklyOrdersAndItemsSeries.Add(new LineSeries<int>
            {
                Name = GetLocalizedString("ReportItemsTitleString"),
                Values = _userWeeklyStats.Select(s => s.TotalItems).ToArray(),
                Fill = null,
                Stroke = itemsStrokePaint,
                GeometrySize = 14,
                GeometryStroke = itemsStrokePaint
            });

            WeeklyOrdersAndItemsSeries.Add(new LineSeries<int>
            {
                Name = GetLocalizedString("ReportOrdersTitleString"),
                Values = _userWeeklyStats.Select(s => s.TotalOrders).ToArray(),
                Fill = null,
                Stroke = ordersStrokePaint,
                GeometrySize = 14,
                GeometryStroke = ordersStrokePaint
            });

        }

        private void BuildWeeklyRevenueSeries()
        {
            WeeklyRevenueSeries.Clear();

            var fillPaint = GetPaint("ColumnFillPaint");
            var sliceStrokePaint = GetPaint("SliceStrokePaint");
            var labelPaint = GetPaint("AxisLabelsPaint");

            WeeklyRevenueSeries.Add(new ColumnSeries<decimal>
            {
                Name = GetLocalizedString("ReportRevenueTitleString"),
                Values = _userWeeklyStats.Select(s => s.TotalRevenue).ToArray(),
                Fill = fillPaint,
                Stroke = sliceStrokePaint
            });

            XAxes =
            [
                new Axis
                {
                    Labels   = WeeklyLabels,
                    MinLimit = -0.5,
                    MaxLimit = WeeklyLabels.Length - 0.5,
                    LabelsPaint = labelPaint
                }
            ];

            RevenueYAxes =
            [
                new Axis
                {
                    MinStep = 1,
                    Labeler = value => $"{(decimal)value:N0} KM",
                    LabelsPaint = labelPaint
                }
            ];

        }

        private async Task SearchProductReport()
        {
            ValidateProduct();

            if (HasErrors)
            {
                ShowErrorDialog(GetLocalizedString("DialogWarningInvalidFieldValuesMessage"));
                return;
            }

            await LoadProductStats();
        }

        private async Task SearchStockReport()
        {
            ValidateStock();

            if (HasErrors)
            {
                ShowErrorDialog(GetLocalizedString("DialogWarningInvalidFieldValuesMessage"));
                return;
            }

            var stockTransactionsList = await _stockService.GetStockTransactionsByProductName(SelectedProductName!, FilterStockDateFrom, FilterStockDateTo);
            StockTransactions = new ObservableCollection<InfoStockTransactionDTO>(stockTransactionsList);

            var stockModificationsList = await _stockService.GetStockModificationsByProductName(SelectedProductName!, FilterStockDateFrom, FilterStockDateTo);
            StockModifications = new ObservableCollection<InfoStockModificationDTO>(stockModificationsList);

            var stockSummaryList = await _stockService.GetStockSummaries(SelectedProductName!, FilterStockDateFrom, FilterStockDateTo);
            StockSummaries = new ObservableCollection<InfoStockSummaryDTO>(stockSummaryList);

            ShowInfoDialog(GetLocalizedString("DialogInfoReportsSuccessfullyGeneratedString"));
        }

        public void ValidateProduct()
        {
            ValidateProductDates();
        }

        public void ValidateStock()
        {
            ValidateSelectedProductName();
            ValidateStockDates();
        }

        private void ValidateProductDates()
        {
            ClearErrors(nameof(FilterProductDateFrom));
            ClearErrors(nameof(FilterProductDateTo));

            if (FilterProductDateFrom > DateTime.Now)
            {
                AddError(nameof(FilterProductDateFrom), GetLocalizedString("ValidateCannotBeInTheFutureMessage"));
            }
            if (FilterProductDateTo > DateTime.Now)
            {
                AddError(nameof(FilterProductDateTo), GetLocalizedString("ValidateCannotBeInTheFutureMessage"));
            }
            if (FilterProductDateTo <= FilterProductDateFrom)
            {
                AddError(nameof(FilterProductDateTo), GetLocalizedString("ValidateShiftMustEndAfterItStartsMessage"));
            }
        }

        private void ValidateSelectedProductName()
        {
            ClearErrors(nameof(SelectedProductName));
            if (SelectedProductName == null)
            {
                AddError(nameof(SelectedProductName), GetLocalizedString("ValidateProductNameMustSelectMessage"));
            }
        }

        private void ValidateStockDates()
        {
            ClearErrors(nameof(FilterStockDateFrom));
            ClearErrors(nameof(FilterStockDateTo));

            if (FilterStockDateFrom > DateTime.Now)
            {
                AddError(nameof(FilterStockDateFrom), GetLocalizedString("ValidateCannotBeInTheFutureMessage"));
            }
            if (FilterStockDateTo > DateTime.Now)
            {
                AddError(nameof(FilterStockDateTo), GetLocalizedString("ValidateCannotBeInTheFutureMessage"));
            }
            if (FilterStockDateTo <= FilterStockDateFrom)
            {
                AddError(nameof(FilterStockDateTo), GetLocalizedString("ValidateShiftMustEndAfterItStartsMessage"));
            }
        }

        private string _stockIdColumnHeader = string.Empty;
        public string StockIdColumnHeader
        {
            get => _stockIdColumnHeader;
            set { _stockIdColumnHeader = value; OnPropertyChanged(); }
        }

        private string _productNameColumnHeader = string.Empty;
        public string ProductNameColumnHeader
        {
            get => _productNameColumnHeader;
            set { _productNameColumnHeader = value; OnPropertyChanged(); }
        }

        private string _quantityColumnHeader = string.Empty;
        public string QuantityColumnHeader
        {
            get => _quantityColumnHeader;
            set { _quantityColumnHeader = value; OnPropertyChanged(); }
        }

        private string _expirationDateColumnHeader = string.Empty;
        public string ExpirationDateColumnHeader
        {
            get => _expirationDateColumnHeader;
            set { _expirationDateColumnHeader = value; OnPropertyChanged(); }
        }

        private string _expirationWarningDaysColumnHeader = string.Empty;
        public string ExpirationWarningDaysColumnHeader
        {
            get => _expirationWarningDaysColumnHeader;
            set { _expirationWarningDaysColumnHeader = value; OnPropertyChanged(); }
        }

        private string _idColumnHeader = string.Empty;
        public string IDColumnHeader
        {
            get => _idColumnHeader;
            set { _idColumnHeader = value; OnPropertyChanged(); }
        }

        private string _transactionDateColumnHeader = string.Empty;
        public string TransactionDateColumnHeader
        {
            get => _transactionDateColumnHeader;
            set { _transactionDateColumnHeader = value; OnPropertyChanged(); }
        }

        private string _quantityChangedColumnHeader = string.Empty;
        public string QuantityChangedColumnHeader
        {
            get => _quantityChangedColumnHeader;
            set { _quantityChangedColumnHeader = value; OnPropertyChanged(); }
        }

        private string _transactionTypeColumnHeader = string.Empty;
        public string TransactionTypeColumnHeader
        {
            get => _transactionTypeColumnHeader;
            set { _transactionTypeColumnHeader = value; OnPropertyChanged(); }
        }

        private string _orderIdColumnHeader = string.Empty;
        public string OrderIdColumnHeader
        {
            get => _orderIdColumnHeader;
            set { _orderIdColumnHeader = value; OnPropertyChanged(); }
        }

        private string _userIdColumnHeader = string.Empty;
        public string UserIdColumnHeader
        {
            get => _userIdColumnHeader;
            set { _userIdColumnHeader = value; OnPropertyChanged(); }
        }

        private string _modificationDateColumnHeader = string.Empty;
        public string ModificationDateColumnHeader
        {
            get => _modificationDateColumnHeader;
            set { _modificationDateColumnHeader = value; OnPropertyChanged(); }
        }

        private string _modificationTypeColumnHeader = string.Empty;
        public string ModificationTypeColumnHeader
        {
            get => _modificationTypeColumnHeader;
            set { _modificationTypeColumnHeader = value; OnPropertyChanged(); }
        }

        private string _oldValueColumnHeader = string.Empty;
        public string OldValueColumnHeader
        {
            get => _oldValueColumnHeader;
            set { _oldValueColumnHeader = value; OnPropertyChanged(); }
        }

        private string _newValueColumnHeader = string.Empty;
        public string NewValueColumnHeader
        {
            get => _newValueColumnHeader;
            set { _newValueColumnHeader = value; OnPropertyChanged(); }
        }

        private string _productIdColumnHeader = string.Empty;
        public string ProductIdColumnHeader
        {
            get => _productIdColumnHeader;
            set { _productIdColumnHeader = value; OnPropertyChanged(); }
        }

        private string _totalQuantityColumnHeader = string.Empty;
        public string TotalQuantityColumnHeader
        {
            get => _totalQuantityColumnHeader;
            set { _totalQuantityColumnHeader = value; OnPropertyChanged(); }
        }

        private string _totalAddedColumnHeader = string.Empty;
        public string TotalAddedColumnHeader
        {
            get => _totalAddedColumnHeader;
            set { _totalAddedColumnHeader = value; OnPropertyChanged(); }
        }

        private string _totalSoldColumnHeader = string.Empty;
        public string TotalSoldColumnHeader
        {
            get => _totalSoldColumnHeader;
            set { _totalSoldColumnHeader = value; OnPropertyChanged(); }
        }

        private string _totalModificationsColumnHeader = string.Empty;
        public string TotalModificationsColumnHeader
        {
            get => _totalModificationsColumnHeader;
            set { _totalModificationsColumnHeader = value; OnPropertyChanged(); }
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

        private void UpdateColumnHeaders()
        {
            if (SelectedLanguage == Language.English)
            {
                StockIdColumnHeader = "Stock ID";
                ProductNameColumnHeader = "Product name";
                QuantityColumnHeader = "Quantity";
                ExpirationDateColumnHeader = "Expiration date";
                ExpirationWarningDaysColumnHeader = "Alert in";
                IDColumnHeader = "ID";
                TransactionDateColumnHeader = "Transaction date";
                QuantityChangedColumnHeader = "Quantity";
                TransactionTypeColumnHeader = "Transaction type";
                OrderIdColumnHeader = "Order ID";
                UserIdColumnHeader = "User ID";
                ModificationDateColumnHeader = "Modification date";
                ModificationTypeColumnHeader = "Modification type";
                OldValueColumnHeader = "Old value";
                NewValueColumnHeader = "New value";
                ProductIdColumnHeader = "Product ID";
                TotalQuantityColumnHeader = "Total quantity";
                TotalAddedColumnHeader = "Total added";
                TotalSoldColumnHeader = "Total sold";
                TotalModificationsColumnHeader = "Total modifications";
            }
            else if (SelectedLanguage == Language.Serbian)
            {
                StockIdColumnHeader = "ID zaliha";
                ProductNameColumnHeader = "Ime proizvoda";
                QuantityColumnHeader = "Količina";
                ExpirationDateColumnHeader = "Datum isteka";
                ExpirationWarningDaysColumnHeader = "Upozorenje";
                IDColumnHeader = "ID";
                TransactionDateColumnHeader = "Datum";
                QuantityChangedColumnHeader = "Količina";
                TransactionTypeColumnHeader = "Tip transakcije";
                OrderIdColumnHeader = "Porudžbina";
                UserIdColumnHeader = "ID korisnika";
                ModificationDateColumnHeader = "Datum izmjene";
                ModificationTypeColumnHeader = "Tip izmjene";
                OldValueColumnHeader = "Staro";
                NewValueColumnHeader = "Novo";
                ProductIdColumnHeader = "ID proizvoda";
                TotalQuantityColumnHeader = "Ukupna količina";
                TotalAddedColumnHeader = "Ukupno dodato";
                TotalSoldColumnHeader = "Ukupno prodato";
                TotalModificationsColumnHeader = "Ukupno izmjena";
            }
        }

    }
}