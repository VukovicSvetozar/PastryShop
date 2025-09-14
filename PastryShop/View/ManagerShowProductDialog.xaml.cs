using System.Windows;
using PastryShop.Data.DTO;
using PastryShop.ViewModel;

namespace PastryShop.View
{
    public partial class ManagerShowProductDialog : Window
    {
        public ManagerShowProductDialog(InfoProductDetailsDTO productDTO)
        {
            InitializeComponent();
            DataContext = new ManagerShowProductDialogViewModel(productDTO);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top += 6;
        }

    }
}