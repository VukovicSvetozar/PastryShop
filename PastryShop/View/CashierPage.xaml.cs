using System.Windows;
using System.Windows.Input;
using PastryShop.Service;
using PastryShop.ViewModel;

namespace PastryShop.View
{
    public partial class CashierPage : Window
    {
        public CashierPage(IUserService userService, IProductService productService, IStockService stockService, ICartService cartService)
        {
            InitializeComponent();
            this.SizeChanged += Window_SizeChanged;
            try
            {
                DataContext = new CashierViewModel(userService, productService, stockService, cartService);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri prikazivanju stranice: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double windowWidthThreshold = 1230;
            double currentWidth = e.NewSize.Width;
            double windowHeightThreshold = 650;
            double currentHeight = e.NewSize.Height;

            if (currentWidth < windowWidthThreshold || currentHeight < windowHeightThreshold)
            {
                MenuGrid.Rows = 6;
                MenuGrid.Columns = 1;
            }
            else
            {
                MenuGrid.Rows = 3;
                MenuGrid.Columns = 2;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.F11)
            {
                ToggleFullScreen();
                e.Handled = true;
            }
        }

        private void ToggleFullScreen()
        {
            if (this.WindowStyle == WindowStyle.None && this.WindowState == WindowState.Maximized)
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
                this.Left = SystemParameters.WorkArea.Left + (SystemParameters.WorkArea.Width - this.Width) / 2;
                this.Top = SystemParameters.WorkArea.Top + (SystemParameters.WorkArea.Height - this.Height) / 2;
            }
            else
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
        }

    }
}