using PastryShop.Command;
using System.Windows.Input;

namespace PastryShop.ViewModel.Dialog
{
    public partial class LoginInputDialogViewModel : BaseViewModel
    {

        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private string _placeHolderText = string.Empty;
        public string PlaceHolderText
        {
            get => _placeHolderText;
            set => SetProperty(ref _placeHolderText, value);
        }

        private string _input = string.Empty;
        public string Input
        {
            get => _input;
            set
            {
                if (SetProperty(ref _input, value))
                {
                    if (!string.IsNullOrWhiteSpace(value))
                        IsInputMissing = false;
                }
            }
        }

        private bool _isInputMissing;
        public bool IsInputMissing
        {
            get => _isInputMissing;
            set => SetProperty(ref _isInputMissing, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool? DialogResult { get; private set; }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public Action CloseAction { get; set; } = () => { };

        public LoginInputDialogViewModel()
        {
            OkCommand = new RelayCommand(_ => OnOk());
            CancelCommand = new RelayCommand(_ => OnCancel());
        }

        private void OnOk()
        {
            if (string.IsNullOrWhiteSpace(Input))
            {
                IsInputMissing = true;
                return;
            }
            DialogResult = true;
            CloseAction?.Invoke();
        }

        private void OnCancel()
        {
            DialogResult = false;
            CloseAction?.Invoke();
        }

    }
}