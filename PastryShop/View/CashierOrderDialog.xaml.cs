using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PastryShop.View
{
    public partial class CashierOrderDialog : UserControl
    {
        public CashierOrderDialog()
        {
            InitializeComponent();
            TotalPriceBorder.SizeChanged += TotalBorder_SizeChanged;
            this.KeyDown += CashierOrderDialog_KeyDown;
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

        private void CashierOrderDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.T && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                FilterComboBox.IsDropDownOpen = true;
            }
        }

    }
}