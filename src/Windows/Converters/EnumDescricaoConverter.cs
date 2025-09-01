using System.Globalization;
using System.Windows.Data;
using Snebur.Utilidade;

namespace Snebur.Windows;

public class EnumDescricaoConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Enum valorEnum)
        {
            return EnumUtil.RetornarDescricao(valorEnum);
        }
        return String.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return new();
    }
}
