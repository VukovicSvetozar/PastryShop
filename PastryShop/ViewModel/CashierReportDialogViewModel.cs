using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using PastryShop.Data.DTO;
using PastryShop.Service;

namespace PastryShop.ViewModel
{
    public partial class CashierReportDialogViewModel : ValidatableBaseViewModel
    {
        private readonly int _userId;
        private readonly IProductService _productService;

        private ObservableCollection<InfoProductTopSalesStatsDTO> _allProducts = [];
        public ObservableCollection<InfoProductTopSalesStatsDTO> Products
        {
            get => _allProducts;
            set => SetProperty(ref _allProducts, value);
        }

        public bool HasDailyOrderStats => DailyFoodItems + DailyDrinkItems + DailyAccessoryItems > 0;

        private List<InfoWeeklyOrderStatsDTO> _userWeeklyStats = [];

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

        private int _dailyOrders;
        public int DailyOrders
        {
            get => _dailyOrders;
            private set { _dailyOrders = value; OnPropertyChanged(); }
        }

        private int _dailyItems;
        public int DailyItems
        {
            get => _dailyItems;
            private set { _dailyItems = value; OnPropertyChanged(); }
        }

        private decimal _dailyRevenue;
        public decimal DailyRevenue
        {
            get => _dailyRevenue;
            private set { _dailyRevenue = value; OnPropertyChanged(); }
        }

        private int _dailyCompletedOrders;
        public int DailyCompletedOrders
        {
            get => _dailyCompletedOrders;
            private set { _dailyCompletedOrders = value; OnPropertyChanged(); }
        }

        private int _dailyCancelledOrders;
        public int DailyCancelledOrders
        {
            get => _dailyCancelledOrders;
            private set { _dailyCancelledOrders = value; OnPropertyChanged(); }
        }

        private int _dailyOnHoldOrders;
        public int DailyOnHoldOrders
        {
            get => _dailyOnHoldOrders;
            private set { _dailyOnHoldOrders = value; OnPropertyChanged(); }
        }

        private int _dailyFoodItems;
        public int DailyFoodItems
        {
            get => _dailyFoodItems;
            private set { _dailyFoodItems = value; OnPropertyChanged(); }
        }

        private int _dailyDrinkItems;
        public int DailyDrinkItems
        {
            get => _dailyDrinkItems;
            private set { _dailyDrinkItems = value; OnPropertyChanged(); }
        }

        private int _dailyAccessoryItems;
        public int DailyAccessoryItems
        {
            get => _dailyAccessoryItems;
            private set { _dailyAccessoryItems = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ISeries> DailyOrderStatsPie { get; }
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

        public CashierReportDialogViewModel(int userId, IProductService productService)
        {
            _userId = userId;
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));

            DailyOrderStatsPie = [];
            WeeklyOrdersAndItemsSeries = [];
            WeeklyRevenueSeries = [];
            WeeklyLabels = [];

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadReport();
            BuildDailyOrderStatsPieSeries();
            BuildWeeklyOrdersItemsSeries();
            BuildWeeklyRevenueSeries();
        }

        private async Task LoadReport()
        {
            var products = await _productService.GetBestSellersProducts(7);
            _allProducts = new ObservableCollection<InfoProductTopSalesStatsDTO>(products);
            Products = new ObservableCollection<InfoProductTopSalesStatsDTO>(products);

            var userOrderStats = await _productService.GetUserOrderStats(_userId, DateTime.Now);
            TotalOrders = userOrderStats.TotalOrders;
            TotalItems = userOrderStats.TotalItems;
            TotalRevenue = userOrderStats.TotalRevenue;
            DailyOrders = userOrderStats.DailyOrders;
            DailyCompletedOrders = userOrderStats.DailyCompletedOrders;
            DailyCancelledOrders = userOrderStats.DailyCancelledOrders;
            DailyOnHoldOrders = userOrderStats.DailyOnHoldOrders;
            DailyItems = userOrderStats.DailyItems;
            DailyRevenue = userOrderStats.DailyRevenue;
            DailyFoodItems = userOrderStats.DailyFoodItems;
            DailyDrinkItems = userOrderStats.DailyDrinkItems;
            DailyAccessoryItems = userOrderStats.DailyAccessoryItems;

            _userWeeklyStats = await _productService.GetUserWeeklyStats(_userId, 6);

            OnPropertyChanged(nameof(HasDailyOrderStats));
        }

        private void BuildDailyOrderStatsPieSeries()
        {
            DailyOrderStatsPie.Clear();

            if (!HasDailyOrderStats)
            {
                DailyOrderStatsPie.Add(new PieSeries<int>
                {
                    Name = GetLocalizedString("ReportPieTitleNoOrdersString"),
                    Values = [1],
                    Fill = GetPaint("PieNoDataPaint"),
                    Stroke = GetPaint("SliceStrokePaint"),
                    InnerRadius = 20
                });
                return;
            }

            var foodFill = GetPaint("PieFoodFillPaint");
            var drinkFill = GetPaint("PieDrinkFillPaint");
            var accFill = GetPaint("PieAccessoryFillPaint");
            var sliceStroke = GetPaint("SliceStrokePaint");

            DailyOrderStatsPie.Add(new PieSeries<int>
            {
                Name = GetLocalizedString("ReportPieTitleFoodString"),
                Values = [DailyFoodItems],
                Fill = foodFill,
                Stroke = sliceStroke,
                InnerRadius = 20
            });
            DailyOrderStatsPie.Add(new PieSeries<int>
            {
                Name = GetLocalizedString("ReportPieTitleDrinkString"),
                Values = [DailyDrinkItems],
                Fill = drinkFill,
                Stroke = sliceStroke,
                InnerRadius = 20
            });
            DailyOrderStatsPie.Add(new PieSeries<int>
            {
                Name = GetLocalizedString("ReportPieTitleAccessoryString"),
                Values = [DailyAccessoryItems],
                Fill = accFill,
                Stroke = sliceStroke,
                InnerRadius = 20
            });
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

    }
}