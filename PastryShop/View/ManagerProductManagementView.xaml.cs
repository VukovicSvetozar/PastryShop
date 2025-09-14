using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PastryShop.ViewModel;

namespace PastryShop.View
{
    public partial class ManagerProductManagementView : UserControl
    {
        public ManagerProductManagementView()
        {
            InitializeComponent();

            this.Focusable = true;
            FocusManager.SetIsFocusScope(this, true);

            this.Loaded += ManagerProductManagementView_Loaded;
            this.KeyDown += ManagerProductManagementView_KeyDown;
        }

        private void ManagerProductManagementView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }

        private void ManagerProductManagementView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (DataContext is ManagerProductManagementViewModel vm && vm.AddNewProductCommand.CanExecute(null))
                {
                    vm.AddNewProductCommand.Execute(null);
                }
            }
            else if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                FilterComboBox.IsDropDownOpen = true;
            }
            else if (e.Key == Key.L && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                btnAdvancedFilter.IsChecked = !btnAdvancedFilter.IsChecked;
            }
            else if (e.Key == Key.Tab || e.Key == Key.F3)
            {
                e.Handled = true;
                SearchTextBox.Focus();
            }
        }

    }
}