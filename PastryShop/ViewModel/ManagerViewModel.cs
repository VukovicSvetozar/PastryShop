using System.Windows;
using System.Windows.Input;
using PastryShop.Service;
using PastryShop.Command;
using PastryShop.Data.DTO;
using PastryShop.Data.DAO;
using PastryShop.View;
using PastryShop.Utility;
using PastryShop.View.Dialog;
using PastryShop.ViewModel.Dialog;

namespace PastryShop.ViewModel
{
    public partial class ManagerViewModel : ValidatableBaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IStockService _stockService;

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

        public ICommand ShowUserManagementCommand { get; }
        public ICommand ShowProductManagementCommand { get; }
        public ICommand ShowReportViewCommand { get; }
        public ICommand ShowSettingViewCommand { get; }
        public ICommand ShowProfileViewCommand { get; }
        public ICommand LogoutCommand { get; }

        public ManagerViewModel(IUserService userService, IProductService productService, IStockService stockService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));

            CurrentUser = UserSession.GetCurrentUser();
            UserSession.CurrentUserChanged += (s, e) =>
            {
                CurrentUser = UserSession.GetCurrentUser();
            };

            ShowUserManagementCommand = new AsyncRelayCommand(_ => ShowUserView());
            ShowProductManagementCommand = new AsyncRelayCommand(_ => ShowProductView());
            ShowReportViewCommand = new AsyncRelayCommand(_ => ShowReportView());
            ShowSettingViewCommand = new AsyncRelayCommand(_ => ShowSettingView());
            ShowProfileViewCommand = new AsyncRelayCommand(_ => ShowProfileView());
            LogoutCommand = new AsyncRelayCommand(_ => Logout());

            _ = ShowUserView();
        }

        private async Task ShowUserView()
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CurrentView = new ManagerUserManagementView
                    {
                        DataContext = new ManagerUserManagementViewModel(_userService)
                    };
                });
            });
        }

        private async Task ShowProductView()
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CurrentView = new ManagerProductManagementView
                    {
                        DataContext = new ManagerProductManagementViewModel(CurrentUser.Id, _productService, _stockService)
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
                    CurrentView = new ManagerReportDialog
                    {
                        DataContext = new ManagerReportDialogViewModel(_productService, _stockService)
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