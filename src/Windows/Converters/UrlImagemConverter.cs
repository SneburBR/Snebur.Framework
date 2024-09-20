using Snebur.Dominio;
using Snebur.ServicoArquivo;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Snebur.Windows
{
    public class UrlImagemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IImagem imagem)
            {
                var tamanhoImagem = this.RetornarTamanhoImagem(parameter);
                var urlServicoImagem = AplicacaoSnebur.Atual.UrlServicoImagem;
                var urlImagem = ServicoImagemClienteUtil.RetornarUrlVisualizarImagem(urlServicoImagem, imagem, tamanhoImagem);
                return urlImagem;
            }
            return null;
        }

        private EnumTamanhoImagem RetornarTamanhoImagem(object parameter)
        {
            if (parameter is string parametroTexto)
            {
                if(Enum.TryParse<EnumTamanhoImagem>(parametroTexto, out var tamanhoImagem))
                {
                    return tamanhoImagem;
                }
            }
            return EnumTamanhoImagem.Pequena;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
