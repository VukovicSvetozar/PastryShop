using System.Windows;

namespace PastryShop.View
{
    public partial class ManagerEditUserDialog : Window
    {
        public ManagerEditUserDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top += 15;
        }

    }
}