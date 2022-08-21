using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Snebur.Windows
{
    public class VerdadeiroParaMostrarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool valor)
            {
                var retorno = valor ? Visibility.Visible : Visibility.Hidden;
                return retorno;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}