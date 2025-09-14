using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PastryShop.ViewModel;

namespace PastryShop.View
{
    public partial class ManagerReportDialog : UserControl
    {
        public ManagerReportDialog()
        {
            InitializeComponent();

            this.Focusable = true;
            FocusManager.SetIsFocusScope(this, true);

            this.Loaded += ManagerReportDialog_Loaded;
            this.KeyDown += ManagerReportDialog_KeyDown;
        }

        private void ManagerReportDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }

        private void ManagerReportDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (DataContext is ManagerReportDialogViewModel vm && vm.SearchProductReportCommand.CanExecute(null))
                {
                    vm.SearchProductReportCommand.Execute(null);
                }
            }
            else if (e.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (DataContext is ManagerReportDialogViewModel vm && vm.SearchStockReportCommand.CanExecute(null))
                {
                    vm.SearchStockReportCommand.Execute(null);
                }
            }
            else if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                ProductComboBox.Focus();
                ProductComboBox.IsDropDownOpen = true;
            }
        }

    }
}