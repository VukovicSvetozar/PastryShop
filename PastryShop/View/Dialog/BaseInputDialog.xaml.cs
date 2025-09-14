using System.Windows;
using PastryShop.ViewModel.Dialog;

namespace PastryShop.View.Dialog
{
    public partial class BaseInputDialog : Window
    {
        public BaseInputDialog()
        {
            InitializeComponent();
            Loaded += BaseInputDialog_Loaded;
        }

        private void BaseInputDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is BaseInputDialogViewModel vm)
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