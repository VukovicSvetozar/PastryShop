using System.Windows;

namespace PastryShop.View
{
    public partial class ManagerEditProductProfileDialog : Window
    {
        public ManagerEditProductProfileDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top += 55;
        }

    }
}