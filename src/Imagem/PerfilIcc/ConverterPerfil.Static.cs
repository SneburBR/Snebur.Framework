using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Snebur.Utilidade;

namespace Snebur.Imagem
{
    public partial class ConverterPerfil
    {
        public static EnumProvedorConvercao RetornarProvedorConversao(string caminhoArquivo, bool jpegUsuarMagick)
        {
            using (var fsOrigem = StreamUtil.OpenRead(caminhoArquivo))
            {
                //var decoder = BitmapDecoder.Create(fsOrigem, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                //var frame = decoder.Frames.First();
                //var formato = frame.Format;
                //var perfilOrigem = frame.RetornarPerfilNativo();

                var corFormato = CorFormatoUtil.RetornarFormatoCor(caminhoArquivo);
                switch (corFormato)
                {
                    case (EnumFormatoCor.Cmyk):

                        if (ImagemUtil.IsExtensaoJpeg(caminhoArquivo) && (jpegUsuarMagick))
                        {
                            return EnumProvedorConvercao.Magick;
                        }
                        return EnumProvedorConvercao.Nativo;

                    case (EnumFormatoCor.Grayscale):

                        return EnumProvedorConvercao.Nativo;

                    default:

                        if (ImagemUtil.IsExtensaoJpeg(caminhoArquivo) && (jpegUsuarMagick))
                        {
                            return EnumProvedorConvercao.Magick;
                        }
                        return EnumProvedorConvercao.Nativo;
                }
            }

        }
    }
}
