using System.Windows;
using PastryShop.Data.DTO;
using PastryShop.ViewModel;

namespace PastryShop.View
{
    public partial class ManagerShowUserDialog : Window
    {
        public ManagerShowUserDialog(InfoUserDetailsDTO userDTO)
        {
            InitializeComponent();
            DataContext = new ManagerShowUserDialogViewModel(userDTO);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top += 55;
        }

    }
}