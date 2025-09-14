using System.Windows;
using PastryShop.Service;
using PastryShop.Utility;
using PastryShop.ViewModel;

namespace PastryShop.View
{
    public partial class ManagerAddProductDialog : Window
    {
        public ManagerAddProductDialog(IProductService productService)
        {
            InitializeComponent();

            try
            {
                DataContext = new ManagerAddProductDialogViewModel(productService);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška pri dodavanju podataka: {ex.Message}", ex);
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top += 56;
        }

    }
}