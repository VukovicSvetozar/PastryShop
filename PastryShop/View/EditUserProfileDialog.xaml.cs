using System.Windows;
using System.Windows.Controls;
using PastryShop.ViewModel;

namespace PastryShop.View
{
    public partial class EditUserProfileDialog : UserControl
    {
        public EditUserProfileDialog()
        {
            InitializeComponent();
            this.DataContextChanged += EditUserProfileDialog_DataContextChanged;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is EditUserProfileDialogViewModel viewModel && sender is PasswordBox passwordBox)
            {
                viewModel.Password = passwordBox.Password;
            }
        }

        private void EditUserProfileDialog_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is EditUserProfileDialogViewModel vm)
            {
                vm.ResetCommandExecuted += (s, args) =>
                {
                    PasswordBox.Clear();
                };
            }
        }

    }
}