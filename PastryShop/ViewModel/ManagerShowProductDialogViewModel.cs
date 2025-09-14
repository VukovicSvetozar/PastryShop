using System.Windows;
using System.Windows.Input;
using PastryShop.Command;
using PastryShop.Data.DTO;

namespace PastryShop.ViewModel
{
    public partial class ManagerShowProductDialogViewModel(InfoProductDetailsDTO productDTO) : BaseViewModel
    {
        public InfoProductDetailsDTO ProductDTO { get; } = productDTO ?? throw new ArgumentNullException(nameof(productDTO));

        public ICommand CloseCommand { get; } = new AsyncRelayCommand(async parameter =>
        {
            if (parameter is Window window)
            {
                await Task.Run(() =>
                {
                    window.Dispatcher.Invoke(() => window.Close());
                });
            }
        });

    }
}