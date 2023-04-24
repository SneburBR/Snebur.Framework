using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using Snebur.Utilidade;

namespace Snebur.Imagens
{
    public class DecoderUtil
    {

        public static BitmapCreateOptions OpcoesBitmap
        {
            get
            {
                //if (SistemaUtil.IsWindowsXp)
                //{
                //    return BitmapCreateOptions.PreservePixelFormat;
                //}
                return BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;
            }
        }

        public static BitmapDecoder RetornarDecoder(Stream stream, BitmapCacheOption cacheOption = BitmapCacheOption.OnDemand)
        {
            try
            {
                //if (SistemaUtil.IsWindowsXp)
                //{
                //    return BitmapDecoder.Create(stream, DecoderUtil.OpcoesBitmap, BitmapCacheOption.OnLoad);
                //}
                return BitmapDecoder.Create(stream, OpcoesBitmap, cacheOption);
            }
            catch (OutOfMemoryException)
            {
                return BitmapDecoder.Create(stream, DecoderUtil.OpcoesBitmap, BitmapCacheOption.None);
            }
            catch (Exception ex)
            {
                throw new ErroImagemCorrompida("Não foi posisvel carregar imagem", ex);
            }
            //return BitmapDecoder.Create(stream, BitmapCreateOptions.IgnoreColorProfile , BitmapCacheOption.OnLoad);
        }

    }
}
