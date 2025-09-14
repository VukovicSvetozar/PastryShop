using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PastryShop.View
{
    public partial class CashierCartDialog : UserControl
    {
        public CashierCartDialog()
        {
            InitializeComponent();
            TotalPriceBorder.SizeChanged += TotalBorder_SizeChanged;
        }

        private void TotalBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = e.NewSize.Width;
            double leftMargin = width * 0.1;
            leftMargin = Math.Min(leftMargin, 30);
            TotalPriceTitle.Margin = new Thickness(leftMargin, 0, 0, 0);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this);
        }

    }
}