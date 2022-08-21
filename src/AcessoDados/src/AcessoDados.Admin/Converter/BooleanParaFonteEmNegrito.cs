using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Snebur.Utilidade;

namespace Snebur.AcessoDados.Admin
{
    public class BooleanParaFonteEmNegrito : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value!= null && ConverterUtil.ParaBoolean(value))
            {
                return FontWeights.Bold;
            }
            return FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
