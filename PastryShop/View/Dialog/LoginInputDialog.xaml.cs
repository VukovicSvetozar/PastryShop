using System.Windows;
using PastryShop.ViewModel.Dialog;

namespace PastryShop.View.Dialog
{
    public partial class LoginInputDialog : Window
    {
        public LoginInputDialog()
        {
            InitializeComponent();
            Loaded += LoginInputDialog_Loaded;
        }

        private void LoginInputDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginInputDialogViewModel vm)
            {
                vm.CloseAction = () =>
                {
                    this.DialogResult = vm.DialogResult;
                    this.Close();
                };
            }
        }

    }
}