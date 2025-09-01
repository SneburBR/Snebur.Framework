using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Snebur.Windows;

public class MaiorZeroParaVisibilidadeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && Double.TryParse(value.ToString(), out var valor) && valor > 0)
        {
            return Visibility.Visible;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return new();
    }
}
