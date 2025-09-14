using System.Windows;
using System.Windows.Input;
using PastryShop.Command;
using PastryShop.Data.DTO;

namespace PastryShop.ViewModel
{
    public partial class ManagerShowUserDialogViewModel(InfoUserDetailsDTO userDTO) : BaseViewModel
    {
        public InfoUserDetailsDTO UserDTO { get; } = userDTO ?? throw new ArgumentNullException(nameof(userDTO));

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