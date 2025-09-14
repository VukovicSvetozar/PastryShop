using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PastryShop.ViewModel;
public partial class BaseViewModel : INotifyPropertyChanged
{
    private bool _isBusy;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, Action? onChanged = null, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        onChanged?.Invoke();
        OnPropertyChanged(propertyName);
        return true;
    }

    public async Task<T> SetBusyAsync<T>(Func<Task<T>> task)
    {
        IsBusy = true;
        try
        {
            return await task().ConfigureAwait(false);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task SetBusyAsync(Func<Task> task)
    {
        IsBusy = true;
        try
        {
            await task().ConfigureAwait(false);
        }
        finally
        {
            IsBusy = false;
        }
    }

}