using System.ComponentModel;

namespace ReactiveUI.TestObjects;

public sealed class NestedTestObservableObject : INotifyPropertyChanged
{
    private int _valueProperty;
    public int ValueProperty
    {
        get => _valueProperty;
        set
        {
            if (_valueProperty != value)
            {
                _valueProperty = value;
                OnPropertyChanged(nameof(ValueProperty));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}