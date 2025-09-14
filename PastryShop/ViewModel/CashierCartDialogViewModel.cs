using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using PastryShop.Command;
using PastryShop.Data.DTO;
using PastryShop.Service;
using PastryShop.Enum;
using PastryShop.View.Dialog;
using PastryShop.ViewModel.Dialog;
using PastryShop.Utility;

namespace PastryShop.ViewModel
{

    public partial class CashierCartDialogViewModel : ValidatableBaseViewModel
    {
        private readonly int _userId;
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly IStockService _stockService;

        public ObservableCollection<CashierCartItemViewModel> CartProducts { get; }

        public decimal TotalCartPrice => CartProducts.Sum(item => item.TotalPrice);

        public Func<Task> OnCartCleared { get; set; } = () => Task.CompletedTask;

        public ICommand RemoveCartItemCommand { get; }
        public ICommand PayLaterCommand { get; }
        public ICommand PayCashCommand { get; }
        public ICommand PayCardCommand { get; }

        public CashierCartDialogViewModel(int userId, ICartService cartService, IProductService productService, IStockService stockService)
        {
            _userId = userId;
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));

            CartProducts = new ObservableCollection<CashierCartItemViewModel>(
                _cartService.GetProducts().Select(dto => new CashierCartItemViewModel(dto))
            );

            _cartService.GetProducts().CollectionChanged += ServiceProducts_CollectionChanged;

            foreach (var item in CartProducts)
            {
                SubscribeToItem(item);
            }

            CartProducts.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalCartPrice));

            RemoveCartItemCommand = new AsyncRelayCommand(RemoveCartItem);
            PayLaterCommand = new AsyncRelayCommand(PayLater);
            PayCashCommand = new AsyncRelayCommand(PayCash);
            PayCardCommand = new AsyncRelayCommand(PayCard);
        }

        private void ServiceProducts_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (e.NewItems != null)
                {
                    foreach (InfoProductBasicDTO newDto in e.NewItems)
                    {
                        if (!CartProducts.Any(item => item.Product.Id == newDto.Id))
                        {
                            var newItem = new CashierCartItemViewModel(newDto);
                            SubscribeToItem(newItem);
                            CartProducts.Add(newItem);
                        }
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (InfoProductBasicDTO oldDto in e.OldItems)
                    {
                        var itemToRemove = CartProducts.FirstOrDefault(item => item.Product.Id == oldDto.Id);
                        if (itemToRemove != null)
                        {
                            itemToRemove.PropertyChanged -= Item_PropertyChanged;
                            CartProducts.Remove(itemToRemove);
                        }
                    }
                }
                OnPropertyChanged(nameof(TotalCartPrice));
            });
        }

        private void SubscribeToItem(CashierCartItemViewModel item)
        {
            item.PropertyChanged += Item_PropertyChanged;
        }

        private void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CashierCartItemViewModel.QuantityProduct) ||
                e.PropertyName == nameof(CashierCartItemViewModel.TotalPrice))
            {
                OnPropertyChanged(nameof(TotalCartPrice));
            }
        }

        private async Task RemoveCartItem(object? parameter)
        {
            if (parameter is CashierCartItemViewModel item)
            {
                await Task.Run(() => _cartService.RemoveProduct(item.Product));
            }
        }

        private async Task PayLater(object? parameter)
        {
            try
            {
                if (CartProducts == null || CartProducts.Count == 0)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningPosCartNoItemsInCartString"));
                    return;
                }

                var dialog = CreateChoiceDialog(GetLocalizedString("DialogChoicePosCartPayLaterString"));

                if (dialog.ShowDialog() == true)
                {
                    await Order(OrderStatus.OnHold);
                    ClearCart();
                    ShowInfoDialog(GetLocalizedString("DialogInfoPosCartCreatedOrderSuccessfullyString"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška prilikom kreiranja porudžbine: {ex.Message}");
            }
        }

        private async Task PayCash(object? parameter)
        {
            try
            {
                if (CartProducts == null || CartProducts.Count == 0)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningPosCartNoItemsInCartString"));
                    return;
                }

                var dialog = CreateChoiceDialog(GetLocalizedString("DialogChoicePosCartPayCashString"));

                if (dialog.ShowDialog() == true)
                {
                    int orderId = await Order(OrderStatus.Completed);
                    Payment(orderId, null);
                    ClearCart();
                    ShowInfoDialog(GetLocalizedString("DialogInfoPosCartCreatedOrderSuccessfullyString"));

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
                if (CartProducts == null || CartProducts.Count == 0)
                {
                    ShowErrorDialog(GetLocalizedString("DialogWarningPosCartNoItemsInCartString"));
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
                    int orderId = await Order(OrderStatus.Completed);
                    Payment(orderId, dialogViewModel.Input);
                    ClearCart();
                    ShowInfoDialog(GetLocalizedString("DialogInfoPosCartCreatedOrderSuccessfullyString"));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom kreiranja porudžbine: {ex.Message}", ex);

            }
        }

        private async Task<int> Order(OrderStatus orderStatus)
        {
            AddOrderDTO addOrderDTO = new()
            {
                UserId = _userId,
                TotalPrice = TotalCartPrice,
                Status = orderStatus

            };
            var orderId = await _productService.CreateOrder(addOrderDTO);

            foreach (var item in CartProducts)
            {
                AddOrderItemDTO addOrderItemDTO = new()
                {
                    OrderId = orderId,
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityProduct,
                    UnitPrice = item.Product.Price
                };
                await _productService.CreateOrderItem(addOrderItemDTO);

                EditReduceStockDTO reduceStockDTO = new()
                {
                    UserId = _userId,
                    ProductId = item.Product.Id,
                    OrderId = orderId,
                    QuantityToReduce = item.QuantityProduct,
                };
                await _stockService.ReduceStockData(reduceStockDTO);
            }
            return orderId;
        }

        private async void Payment(int orderId, string? cardNumber)
        {
            PaymentMethod paymentMethod = cardNumber != null ? PaymentMethod.Card : PaymentMethod.Cash;

            AddPaymentDTO addPaymentDTO = new()
            {
                UserId = _userId,
                OrderId = orderId,
                PaymentMethod = paymentMethod,
                PaymentStatus = PaymentStatus.Completed,
                AmountPaid = TotalCartPrice,
                CardNumber = cardNumber,

            };
            await _productService.CreatePayment(addPaymentDTO);
        }

        private void ClearCart()
        {
            CartProducts.Clear();
            _cartService.ClearCart();
            OnCartCleared?.Invoke();
        }

    }
}