using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PastryShop.View
{
    public partial class CashierPosDialog : UserControl
    {
        public CashierPosDialog()
        {
            InitializeComponent();
            this.Focusable = true;
            FocusManager.SetIsFocusScope(this, true);

            this.Loaded += CashierPosDialog_Loaded;
            this.KeyDown += CashierPosDialog_KeyDown;
        }

        private void CashierPosDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }

        private void OnAddToCartButtonClick(object sender, RoutedEventArgs e)
        {
            RestoreFocusOnCart();
        }

        private void CashierPosDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.T && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                FilterComboBox.IsDropDownOpen = true;
            }
            else if (e.Key == Key.F3)
            {
                e.Handled = true;
                SearchTextBox.Focus();
            }
        }

        private void RestoreFocusOnCart()
        {
            Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    if (CurrentViewHost.Content is IInputElement cartDialog)
                        Keyboard.Focus(cartDialog);
                }),
                DispatcherPriority.Input
            );
        }

    }
}