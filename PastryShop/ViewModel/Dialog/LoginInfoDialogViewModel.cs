using PastryShop.Command;
using System.Windows.Input;

namespace PastryShop.ViewModel.Dialog
{
    partial class LoginInfoDialogViewModel : BaseViewModel
    {
        private string _iconText = string.Empty;
        public string IconText
        {
            get => _iconText;
            set => SetProperty(ref _iconText, value);
        }

        private string _messageText = string.Empty;
        public string MessageText
        {
            get => _messageText;
            set => SetProperty(ref _messageText, value);
        }

        public bool? DialogResult { get; private set; }

        public ICommand CloseCommand { get; }

        public Action CloseAction { get; set; } = () => { };

        public LoginInfoDialogViewModel()
        {
            CloseCommand = new RelayCommand(_ => OnClose());
        }

        private void OnClose()
        {
            DialogResult = true;
            CloseAction?.Invoke();
        }

    }
}