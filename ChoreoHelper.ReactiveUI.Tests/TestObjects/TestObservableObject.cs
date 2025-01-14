using System.ComponentModel;

namespace ReactiveUI.TestObjects;


public sealed class TestObservableObject : INotifyPropertyChanged
{
    private string? _simpleProperty;
    public string? SimpleProperty
    {
        get => _simpleProperty;
        set
        {
            if (_simpleProperty != value)
            {
                _simpleProperty = value;
                OnPropertyChanged(nameof(SimpleProperty));
            }
        }
    }

    private NestedTestObservableObject? _nestedProperty;
    public NestedTestObservableObject? NestedProperty
    {
        get => _nestedProperty;
        set
        {
            if (_nestedProperty != value)
            {
                _nestedProperty = value;
                OnPropertyChanged(nameof(NestedProperty));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}