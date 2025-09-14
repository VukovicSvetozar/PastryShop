using System.Windows;
using PastryShop.ViewModel.Dialog;

namespace PastryShop.View.Dialog
{
    public partial class BaseChoiceDialog : Window
    {
        public BaseChoiceDialog()
        {
            InitializeComponent();
            Loaded += BaseChoiceDialog_Loaded;
        }

        private void BaseChoiceDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is BaseChoiceDialogViewModel vm)
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