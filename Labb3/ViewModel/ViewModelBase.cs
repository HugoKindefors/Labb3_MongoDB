using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Labb3.ViewModel;

public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value!;
        RaisePropertyChanged(propertyName);
        return true;
    }
}
