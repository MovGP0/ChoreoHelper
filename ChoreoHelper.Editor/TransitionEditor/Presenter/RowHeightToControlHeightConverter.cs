using System.Globalization;
using System.Windows.Data;

namespace ChoreoHelper.Editor.TransitionEditor.Presenter;

public sealed class RowHeightToControlHeightConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double rowHeight)
            return rowHeight - 8; // Adjust for margin if needed
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}