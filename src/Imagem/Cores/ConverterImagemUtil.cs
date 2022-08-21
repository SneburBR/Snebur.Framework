using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Snebur.Dominio;
using Snebur.Utilidade;

namespace Snebur.Imagem
{
    public class ConverterCorFormatoImagemUtil
    {
        public static MemoryStream ConverterFormatado(string caminhoArquivoFormatoOrigem, Stream streamImagem, EnumFormatoImagem formato, int qualidade = ConstantesImagemApresentacao.QUALIDADE_PADRAO)
        {
            var encoder = EncoderUtil.RetornarEncoder(formato, qualidade);
            streamImagem.Position = 0;
            encoder.Frames.Add(BitmapFrame.Create(streamImagem));
            var ms = new MemoryStream();
            encoder.Save(ms);
            return ms;
        }

       
    }
}
