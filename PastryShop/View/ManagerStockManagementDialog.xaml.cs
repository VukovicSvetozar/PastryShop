using System.Windows;
using System.Windows.Controls;

namespace PastryShop.View
{
    public partial class ManagerStockManagementDialog : Window
    {
        private const double HorizontalThreshold = 1200;

        public ManagerStockManagementDialog()
        {
            InitializeComponent();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < HorizontalThreshold)
            {
                StockItemsGrid.RowDefinitions.Clear();
                StockItemsGrid.ColumnDefinitions.Clear();

                StockItemsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(4, GridUnitType.Star) });
                StockItemsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                Grid.SetRow(StockItemsListBox, 0);
                Grid.SetColumn(StockItemsListBox, 0);
                Grid.SetRow(StockButtonItemsStackPanel, 1);
                Grid.SetColumn(StockButtonItemsStackPanel, 0);

                StockButtonItemsStackPanel.Orientation = Orientation.Horizontal;
            }
            else
            {
                StockItemsGrid.RowDefinitions.Clear();
                StockItemsGrid.ColumnDefinitions.Clear();

                StockItemsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) });
                StockItemsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Grid.SetColumn(StockItemsListBox, 0);
                Grid.SetRow(StockItemsListBox, 0);
                Grid.SetColumn(StockButtonItemsStackPanel, 1);
                Grid.SetRow(StockButtonItemsStackPanel, 0);

                StockButtonItemsStackPanel.Orientation = Orientation.Vertical;
            }
        }

    }
}