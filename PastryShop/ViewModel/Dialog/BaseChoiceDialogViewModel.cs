using System.Windows.Input;
using PastryShop.Command;

namespace PastryShop.ViewModel.Dialog
{
    public partial class BaseChoiceDialogViewModel : BaseViewModel
    {
        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public bool? DialogResult { get; private set; }

        public ICommand YesCommand { get; }

        public Action CloseAction { get; set; } = () => { };

        public BaseChoiceDialogViewModel()
        {
            YesCommand = new RelayCommand(_ => OnYes());
        }

        private void OnYes()
        {
            DialogResult = true;
            CloseAction?.Invoke();
        }

    }
}