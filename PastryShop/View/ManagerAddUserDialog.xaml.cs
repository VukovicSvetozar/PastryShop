using System.Windows;
using System.Windows.Controls;
using PastryShop.Service;
using PastryShop.Utility;
using PastryShop.ViewModel;

namespace PastryShop.View
{
    public partial class ManagerAddUserDialog : Window
    {
        public ManagerAddUserDialog(IUserService userService)
        {
            InitializeComponent();

            try
            {
                DataContext = new ManagerAddUserDialogViewModel(userService);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška pri učitavanju podataka: {ex.Message}", ex);
                Close();
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            var viewModel = (ManagerAddUserDialogViewModel)DataContext;

            viewModel.Password = passwordBox.Password;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top += 55;
        }

    }
}