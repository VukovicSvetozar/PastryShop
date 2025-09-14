using System.Windows;

namespace PastryShop.View
{
    public partial class ManagerEditProductDataDialog : Window
    {
        public ManagerEditProductDataDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top += 5;
        }

    }
}