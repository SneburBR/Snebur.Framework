﻿
namespace Snebur.Imagens
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.Versioning;

    public static class BitmapExtesao
    {
        public static void Salvar(this Bitmap bitmap, string caminhoDestinho, ImageFormat formato, int qualidade)
        {
            var encoder = RetornarEncoder(formato);
            EncoderParameters myEncoderParameters = null;
            if (formato == ImageFormat.Jpeg)
            {
                var myEncoder = Encoder.Quality;
                myEncoderParameters = new EncoderParameters(1);
                var myEncoderParameter = new EncoderParameter(myEncoder, Convert.ToInt64(qualidade));
                myEncoderParameters.Param[0] = myEncoderParameter;
            }
            bitmap.Save(caminhoDestinho, encoder, myEncoderParameters);
            //Snebur.Utilidade.ArquivoUtil.DeletarArquivo(caminhoDestinho);

        }

        public static ImageCodecInfo RetornarEncoder(ImageFormat formato)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == formato.Guid)
                {
                    return codec;
                }
            }
            throw new Erro($"Não foi possível encontrar um encoder para o formato {formato.ToString()}");
        }
    }

}
