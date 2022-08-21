using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Windows.Media.Imaging
{
    public static class BitmapDecoderExtensao
    {
        private static FieldInfo _filedCacheOption;
        private static FieldInfo FiledCacheOption
        {
            get
            {
                if (_filedCacheOption == null)
                {
                    _filedCacheOption = typeof(BitmapDecoder).GetField("_cacheOption", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (_filedCacheOption == null)
                    {
                        throw new Exception($"Não foi encotnrado o Field '_cacheOption' no tipo {typeof(BitmapDecoder).Name} ");
                    }
                }
                return _filedCacheOption;
            }

        }
        public static BitmapCacheOption RetornarCacheOption(this BitmapDecoder bitmapDecoder )
        {
            return (BitmapCacheOption)BitmapDecoderExtensao.FiledCacheOption.GetValue (bitmapDecoder);
        }
    }
}
