using System.Windows;
using System.Windows.Input;
using PastryShop.Command;
using PastryShop.Data.DAO;
using PastryShop.Data.DTO;
using PastryShop.Service;
using PastryShop.Utility;
using PastryShop.View;
using PastryShop.View.Dialog;
using PastryShop.ViewModel.Dialog;

namespace PastryShop.ViewModel
{
    public partial class CashierViewModel : ValidatableBaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IStockService _stockService;
        private readonly ICartService _cartService;

        private object _currentView = new();
        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        private LoginAuthenticatedUserDTO _currentUser = new();
        public LoginAuthenticatedUserDTO CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

        public ICommand ShowPosViewCommand { get; }
        public ICommand ShowOrderViewCommand { get; }
        public ICommand ShowReportViewCommand { get; }
        public ICommand ShowSettingViewCommand { get; }
        public ICommand ShowProfileViewCommand { get; }
        public ICommand LogoutCommand { get; }

        public CashierViewModel(IUserService userService, IProductService productService, IStockService stockService, ICartService cartService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));

            CurrentUser = UserSession.GetCurrentUser();
            UserSession.CurrentUserChanged += (s, e) =>
            {
                CurrentUser = UserSession.GetCurrentUser();
            };

            ShowPosViewCommand = new AsyncRelayCommand(_ => ShowPosView());
            ShowOrderViewCommand = new AsyncRelayCommand(_ => ShowOrderView());
            ShowReportViewCommand = new AsyncRelayCommand(_ => ShowReportView());
            ShowSettingViewCommand = new AsyncRelayCommand(_ => ShowSettingView());
            ShowProfileViewCommand = new AsyncRelayCommand(_ => ShowProfileView());
            LogoutCommand = new AsyncRelayCommand(_ => Logout());

            _ = ShowPosView();

        }

        private async Task ShowPosView()
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CurrentView = new CashierPosDialog
                    {
                        DataContext = new CashierPosDialogViewModel(CurrentUser.Id, _productService, _stockService, _cartService)
                    };
                });
            });
        }

        private async Task ShowOrderView()
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CurrentView = new CashierOrderDialog
                    {
                        DataContext = new CashierOrderDialogViewModel(CurrentUser.Id, _productService, _stockService)
                    };
                });
            });
        }

        private async Task ShowReportView()
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CurrentView = new CashierReportDialog
                    {
                        DataContext = new CashierReportDialogViewModel(CurrentUser.Id, _productService)
                    };
                });
            });
        }

        private async Task ShowSettingView()
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CurrentView = new EditUserSettingDialog
                    {
                        DataContext = new EditUserSettingDialogViewModel(_userService)
                    };
                });
            });
        }

        private async Task ShowProfileView()
        {
            var editUserProfileDto = await _userService.GetEditUserProfileById(CurrentUser.Id);
            if (editUserProfileDto == null)
            {
                ShowInfoDialog(GetLocalizedString("DialogInfoUserNotFoundMessage"));
                return;
            }
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CurrentView = new EditUserProfileDialog
                    {
                        DataContext = new EditUserProfileDialogViewModel(editUserProfileDto, _userService)
                    };
                });
            });
        }

        private static async Task Logout()
        {
            var dialogViewModel = new BaseChoiceDialogViewModel
            {
                Message = GetLocalizedString("DialogWarningLogoutMessage")
            };

            var dialog = new BaseChoiceDialog
            {
                DataContext = dialogViewModel
            };

            if (dialog.ShowDialog() == true)
            {

                await Task.Run(() => UserSession.Logout());

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    var loginPage = new LoginPage(new UserService(new UserDao(PastryShop.App.Settings.ConnectionStrings.MySqlConnectionString)),
                    new ProductService(new ProductDao(PastryShop.App.Settings.ConnectionStrings.MySqlConnectionString),
                         new StockService(new StockDao(PastryShop.App.Settings.ConnectionStrings.MySqlConnectionString))),
                    new StockService(new StockDao(PastryShop.App.Settings.ConnectionStrings.MySqlConnectionString)),
                    new CartService());

                    Application.Current.MainWindow = loginPage;

                    loginPage.Show();

                    Application.Current.Windows[0].Close();
                });
            }
            else
            {
                Application.Current.MainWindow.Focus();
            }

            await Task.CompletedTask;
        }

    }
}