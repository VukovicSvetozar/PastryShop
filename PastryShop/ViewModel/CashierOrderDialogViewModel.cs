using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using PastryShop.Command;
using PastryShop.Data.DTO;
using PastryShop.Enum;
using PastryShop.Service;
using PastryShop.ViewModel.Dialog;
using PastryShop.View.Dialog;
using PastryShop.Utility;

namespace PastryShop.ViewModel
{
    public partial class CashierOrderDialogViewModel : ValidatableBaseViewModel
    {
        private readonly int _userId;
        private readonly IProductService _productService;
        private readonly IStockService _stockService;

        private ObservableCollection<InfoOrderDTO> _allOrders = [];
        private ObservableCollection<InfoOrderDTO> _orders = [];
        private ObservableCollection<InfoOrderItemDTO> _orderItems = [];

        public ObservableCollection<InfoOrderDTO> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        public ObservableCollection<InfoOrderItemDTO> OrderItems
        {
            get => _orderItems;
            set => SetProperty(ref _orderItems, value);
        }

        private InfoOrderDTO? _selectedOrder;
        public InfoOrderDTO? SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (SetProperty(ref _selectedOrder, value))
                {
                    LoadOrderItems();
                }
            }
        }

        private string _selectedFilter = "OrderFilterAllOrderKey";
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (SetProperty(ref _selectedFilter, value))
                {
                    FilterOrders();
                }
            }
        }

        private string _idColumnHeader = string.Empty;
        public string IDColumnHeader
        {
            get => _idColumnHeader;
            set { _idColumnHeader = value; OnPropertyChanged(); }
        }

        private string _dateColumnHeader = string.Empty;
        public string DateColumnHeader
        {
            get => _dateColumnHeader;
            set { _dateColumnHeader = value; OnPropertyChanged(); }
        }

        private string _totalPriceColumnHeader = string.Empty;
        public string TotalPriceColumnHeader
        {
            get => _totalPriceColumnHeader;
            set { _totalPriceColumnHeader = value; OnPropertyChanged(); }
        }

        private string _statusColumnHeader = string.Empty;
        public string StatusColumnHeader
        {
            get => _statusColumnHeader;
            set { _statusColumnHeader = value; OnPropertyChanged(); }
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

        public ObservableCollection<string> FilterOptions { get; } = ["OrderFilterAllOrderKey", "OrderFilterCompletedKey", "OrderFilterCancelledKey", "OrderFilterOnHoldKey"];

        public ICommand CancelledOrderCommand { get; }
        public ICommand PayCashCommand { get; }
        public ICommand PayCardCommand { get; }

        public CashierOrderDialogViewModel(int userId, IProductService productService, IStockService stockService)
        {
            _userId = userId;
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));

            SelectedLanguage = UserSession.GetCurrentUser().Language;
            UpdateColumnHeaders();

            CancelledOrderCommand = new AsyncRelayCommand(CancelledOrder);
            PayCashCommand = new AsyncRelayCommand(PayCash);
            PayCardCommand = new AsyncRelayCommand(PayCard);

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadOrders();
        }

        private async Task LoadOrders()
        {
            var orders = await _productService.GetRecentOrdersForUser(_userId);
            _allOrders = new ObservableCollection<InfoOrderDTO>(orders);
            Orders = new ObservableCollection<InfoOrderDTO>(_allOrders);
            OnPropertyChanged(nameof(Orders));
        }

        private async void LoadOrderItems()
        {
            if (SelectedOrder != null)
            {
                var items = await _productService.GetOrderItemsByOrderId(SelectedOrder.Id);
                OrderItems = new ObservableCollection<InfoOrderItemDTO>(items);
            }
            else
            {
                OrderItems.Clear();
            }
        }

        private void FilterOrders()
        {
            var filtered = _allOrders;

            if (_selectedFilter == "OrderFilterCompletedKey")
            {
                filtered = new ObservableCollection<InfoOrderDTO>(_allOrders.Where(p => p.Status == OrderStatus.Completed));
            }
            else if (_selectedFilter == "OrderFilterCancelledKey")
            {
                filtered = new ObservableCollection<InfoOrderDTO>(_allOrders.Where(p => p.Status == OrderStatus.Cancelled));
            }
            else if (_selectedFilter == "OrderFilterOnHoldKey")
            {
                filtered = new ObservableCollection<InfoOrderDTO>(_allOrders.Where(p => p.Status == OrderStatus.OnHold));
            }

            Orders = filtered;
            OnPropertyChanged(nameof(Orders));
        }

        private async Task CancelledOrder(object? parameter)
        {
            try
            {
                if (SelectedOrder == null)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningOrderNotSelectedString"));
                    return;
                }

                if (SelectedOrder.Status == OrderStatus.Cancelled)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningOrderAlreadyCancelledString"));
                    return;
                }

                var dialog = CreateChoiceDialog(GetLocalizedString("DialogChoiceOrderCanceledString"));

                if (dialog.ShowDialog() == true)
                {
                    await Cancelled(SelectedOrder.Id);
                    await LoadOrders();
                    ShowInfoDialog(GetLocalizedString("DialogInfoOrderCanceledSuccessfulyString"));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom kreiranja porudžbine: {ex.Message}", ex);
            }
        }
        private async Task PayCash(object? parameter)
        {
            try
            {
                if (SelectedOrder == null)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningOrderNotSelectedString"));
                    return;
                }

                if (SelectedOrder.Status == OrderStatus.Cancelled)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningOrderCancelledString"));
                    return;
                }

                if (SelectedOrder.Status == OrderStatus.Completed)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningOrderAlreadyPaidString"));
                    return;
                }

                var dialog = CreateChoiceDialog(GetLocalizedString("DialogChoicePosCartPayCashString"));

                if (dialog.ShowDialog() == true)
                {
                    await Payment(SelectedOrder.Id, null);
                    await LoadOrders();
                    ShowInfoDialog(GetLocalizedString("DialogInfoOrderPaidSuccessfulyString"));

                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom kreiranja porudžbine: {ex.Message}", ex);
            }
        }

        private async Task PayCard(object? parameter)
        {
            try
            {
                if (SelectedOrder == null)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningOrderNotSelectedString"));
                    return;
                }

                if (SelectedOrder.Status == OrderStatus.Cancelled)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningOrderCancelledString"));
                    return;
                }

                if (SelectedOrder.Status == OrderStatus.Completed)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningOrderAlreadyPaidString"));
                    return;
                }

                var dialogViewModel = new BaseInputDialogViewModel
                {
                    Message = GetLocalizedString("DialogChoicePosCartPayCardString"),
                    PlaceHolderText = GetLocalizedString("DialogInputPosCartEnterCardNumberString"),
                    ErrorMessage = GetLocalizedString("DialogWarningPosCartFieldRequiredString")
                };

                var dialog = new BaseInputDialog
                {
                    DataContext = dialogViewModel
                };

                if (dialog.ShowDialog() == true)
                {
                    await Payment(SelectedOrder.Id, dialogViewModel.Input);
                    await LoadOrders();
                    ShowInfoDialog(GetLocalizedString("DialogInfoOrderPaidSuccessfulyString"));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom kreiranja porudžbine: {ex.Message}", ex);
            }
        }

        private async Task Cancelled(int orderId)
        {
            await _stockService.RefundStock(orderId);

            if (_selectedOrder!.Status == OrderStatus.Completed)
            {
                EditStatusPaymentDTO editStatusPaymentDTO = new()
                {
                    OrderId = orderId,
                    PaymentStatus = PaymentStatus.Refunded
                };
                await _productService.EditStatusPayment(editStatusPaymentDTO);
            }

            EditOrderDataDTO orderDTO = new()
            {
                Id = _selectedOrder!.Id,
                Status = OrderStatus.Cancelled
            };
            await _productService.EditOrderData(orderDTO);
        }

        private async Task Payment(int orderId, string? cardNumber)
        {
            PaymentMethod paymentMethod = cardNumber != null ? PaymentMethod.Card : PaymentMethod.Cash;

            AddPaymentDTO addPaymentDTO = new()
            {
                UserId = _userId,
                OrderId = orderId,
                PaymentMethod = paymentMethod,
                PaymentStatus = PaymentStatus.Completed,
                AmountPaid = _selectedOrder!.TotalPrice,
                CardNumber = cardNumber,
            };
            await _productService.CreatePayment(addPaymentDTO);

            EditOrderDataDTO orderDTO = new()
            {
                Id = _selectedOrder.Id,
                Status = OrderStatus.Completed
            };
            await _productService.EditOrderData(orderDTO);
        }

        private void UpdateColumnHeaders()
        {
            if (SelectedLanguage == Language.English)
            {
                IDColumnHeader = "ID";
                DateColumnHeader = "Date";
                TotalPriceColumnHeader = "Total Price";
                StatusColumnHeader = "Status";
            }
            else if (SelectedLanguage == Language.Serbian)
            {
                IDColumnHeader = "ID";
                DateColumnHeader = "Datum";
                TotalPriceColumnHeader = "Ukupna Cijena";
                StatusColumnHeader = "Status";
            }
        }

    }
}