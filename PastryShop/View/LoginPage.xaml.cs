using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Net;
using PastryShop.Service;
using PastryShop.ViewModel;
using PastryShop.Utility;

namespace PastryShop.View
{
    public partial class LoginPage : Window
    {
        public LoginPage(IUserService userService, IProductService productService, IStockService stockService, ICartService cartService)
        {
            InitializeComponent();

            try
            {
                DataContext = new LoginViewModel(userService, productService, stockService, cartService);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška pri pokretanju aplikacije: {ex.Message}", ex);
                Close();
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel && sender is PasswordBox passwordBox)
            {
                viewModel.Password = new NetworkCredential(string.Empty, passwordBox.SecurePassword).Password;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                ToggleFullScreen();
            }
            else if (e.Key == Key.Escape)
            {
                if (DataContext is LoginViewModel vm && vm.CancelCommand.CanExecute(null))
                {
                    vm.CancelCommand.Execute(null);
                }
            }
            else if (e.Key == Key.Tab)
            {
                if (Keyboard.FocusedElement == PasswordBox)
                {
                    e.Handled = true;
                    Keyboard.Focus(UsernameTextBox);
                }
            }
            else if (e.Key == Key.L && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (FindName("LanguageComboBox") is ComboBox comboBox)
                {
                    comboBox.IsDropDownOpen = true;
                }
            }
        }

        private void ToggleFullScreen()
        {
            if (this.WindowStyle == WindowStyle.None && this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

    }
}