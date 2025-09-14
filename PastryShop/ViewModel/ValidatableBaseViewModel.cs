using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using LiveChartsCore.SkiaSharpView.Painting;
using PastryShop.View.Dialog;
using PastryShop.ViewModel.Dialog;

namespace PastryShop.ViewModel
{
    public partial class ValidatableBaseViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        public readonly Dictionary<string, List<string>> _propertyErrors = [];

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public bool HasErrors => _propertyErrors.Count != 0;

        public static readonly Regex ValidDigitsRegex = new Regex(@"^\d+$", RegexOptions.Compiled);
        public static readonly Regex ValidLettersAndDigitsRegex = new Regex("^[a-zA-Z0-9ćčžđšĆČŽĐŠ ]+$", RegexOptions.Compiled);
        public static readonly Regex ValidLettersDigitsAndSpaceRegex = new Regex("^[a-zA-Z0-9ćčžđšĆČŽĐŠ ]+$", RegexOptions.Compiled);
        public static readonly Regex ValidPhoneNumberRegex = new Regex(@"^\+?[\d\s\-\(\)]*$", RegexOptions.Compiled);
        public static readonly Regex ValidDimensionsRegex = new Regex(@"^\d+(\.\d+)?(\s*[xX]\s*\d+(\.\d+)?)?$", RegexOptions.Compiled);

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return _propertyErrors.SelectMany(e => e.Value);
            return _propertyErrors.TryGetValue(propertyName, out List<string>? value) ? value : Enumerable.Empty<string>();
        }

        protected void AddError(string propertyName, string error)
        {
            if (!_propertyErrors.ContainsKey(propertyName))
                _propertyErrors[propertyName] = [];

            if (!_propertyErrors[propertyName].Contains(error))
            {
                _propertyErrors[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        protected void ClearErrors(string propertyName)
        {
            if (_propertyErrors.Remove(propertyName))
                OnErrorsChanged(propertyName);
        }

        public void ClearAllErrors()
        {
            var propertyNames = _propertyErrors.Keys.ToList();
            _propertyErrors.Clear();

            foreach (var prop in propertyNames)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(prop));
            }

            OnPropertyChanged(nameof(HasErrors));
        }

        protected void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(HasErrors));
        }

        public static void ShowErrorDialog(string message)
        {
            var infoDialogViewModel = new BaseInformationDialogViewModel
            {
                IconText = "⚠",
                MessageText = message
            };

            var infoDialog = new BaseInformationDialog
            {
                DataContext = infoDialogViewModel
            };

            infoDialog.ShowDialog();
        }

        public static void ShowInfoDialog(string message)
        {
            var infoDialogViewModel = new BaseInformationDialogViewModel
            {
                IconText = "✅",
                MessageText = message
            };

            var infoDialog = new BaseInformationDialog
            {
                DataContext = infoDialogViewModel
            };

            infoDialog.ShowDialog();
        }

        public static BaseChoiceDialog CreateChoiceDialog(string message)
        {
            var dialogViewModel = new BaseChoiceDialogViewModel
            {
                Message = message
            };

            var dialog = new BaseChoiceDialog
            {
                DataContext = dialogViewModel
            };

            return dialog;
        }

        public static BaseInputDialog CreateInputDialog(string message, string placeHolderText, string errorMessage)
        {
            var dialogViewModel = new BaseInputDialogViewModel
            {
                Message = message,
                PlaceHolderText = placeHolderText,
                ErrorMessage = errorMessage
            };

            var dialog = new BaseInputDialog
            {
                DataContext = dialogViewModel
            };

            return dialog;
        }

        public static string GetLocalizedString(string key)
        {
            var localized = Application.Current.TryFindResource(key) as string;
            return localized ?? $"[{key}]";
        }
        public static SolidColorPaint GetPaint(string resourceKey)
        {
            return (SolidColorPaint)Application.Current.Resources[resourceKey];
        }

    }
}