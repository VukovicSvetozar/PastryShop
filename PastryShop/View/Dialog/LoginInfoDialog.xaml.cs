using System.Windows;
using PastryShop.ViewModel.Dialog;

namespace PastryShop.View.Dialog
{
    public partial class LoginInfoDialog : Window
    {
        public LoginInfoDialog()
        {
            InitializeComponent();
            Loaded += LoginWarningDialog_Loaded;
        }

        private void LoginWarningDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginInfoDialogViewModel vm)
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