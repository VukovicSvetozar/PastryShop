using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PastryShop.ViewModel;

namespace PastryShop.View
{
    public partial class ManagerUserManagementView : UserControl
    {

        public ManagerUserManagementView()
        {
            InitializeComponent();

            this.Focusable = true;
            FocusManager.SetIsFocusScope(this, true);

            this.Loaded += ManagerUserManagementView_Loaded;
            this.KeyDown += ManagerUserManagementView_KeyDown;
        }

        private void ManagerUserManagementView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }

        private void ManagerUserManagementView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (DataContext is ManagerUserManagementViewModel vm && vm.AddNewUserCommand.CanExecute(null))
                {
                    vm.AddNewUserCommand.Execute(null);
                }
            }
            else if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                FilterComboBox.IsDropDownOpen = true;
            }
            else if (e.Key == Key.Tab || e.Key == Key.F3)
            {
                e.Handled = true;
                SearchTextBox.Focus();
            }
        }

    }
}