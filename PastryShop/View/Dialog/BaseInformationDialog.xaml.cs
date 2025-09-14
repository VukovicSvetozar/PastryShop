using System.Windows;
using PastryShop.ViewModel.Dialog;

namespace PastryShop.View.Dialog
{
    public partial class BaseInformationDialog : Window
    {
        public BaseInformationDialog()
        {
            InitializeComponent();
            Loaded += BaseInformationDialog_Loaded;
        }

        private void BaseInformationDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is BaseInformationDialogViewModel vm)
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