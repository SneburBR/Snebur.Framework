using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Snebur.Imagens;

public partial class PerfilIccUtil
{

    public const int TAMANHO_PERFIL_SRGB = 3144;

    //total de bytes do perfil -
    //existem dois perfil
    internal const int TAMANHO_PERFIL_ADOBE_RGB_560 = 560;

    internal const int TAMANHO_PERFIL_ADOBE_RGB_940 = 940;

    internal const string CHECKSUM_SRGB = "ad2cad08-8e01-42f2-b558-5c7c4099f646";

    private const string CHECKSUM_ADOBE_RGB_560 = "89d04ccc-967c-41a0-82d6-2e80b9cce1ea";

    private const string CHECKSUM_ADOBE_RGB_960 = "1babe5e4-a680-4c7b-b585-67ace9817ca1";

    internal const string CHECKSUM_ADOBE_RGB_500 = "7231731d-eba6-4d73-b912-f8534f6229e9";

    internal static HashSet<string> ChecksumAdobeRGB { get; } =
                    new HashSet<string>(new string[] {CHECKSUM_ADOBE_RGB_560,
                                                      "f37206ef-b757-446b-8643-d116754ca556", //560,
                                                      "1f9b2526-535a-4c2c-868b-65e299599739", //560,
                                                      CHECKSUM_ADOBE_RGB_960});

    private const string NOME_ARQUIVO_PERFIL_SGRAY = "sgray.icc";

    //private const string NOME_PERFIL_SGRAY = "sGray";

    public static string[] ExtencoesPerfilIcc = { ".icm", ".icc" };

#pragma warning disable IDE0032  
    private static ColorContext? _sRGB;
    private static ReadOnlyCollection<ColorContext>? _sRGBColorContexts;
#pragma warning restore IDE0032 

    public static ColorContext sRGB
    {
        get
        {
            if (_sRGB is null)
            {
                lock (_bloqueio)
                {
                    if (_sRGB == null)
                    {
                        _sRGB = PerfilIccUtil.RetornarPerfilNativo(EnumPerfilIcc.sRGB);
                    }
                }
            }
            return _sRGB;
        }
    }

    public static ReadOnlyCollection<ColorContext> sRGBColorContexts
    {
        get
        {
            if (_sRGBColorContexts == null)
            {
                lock (_bloqueio)
                {
                    if (_sRGBColorContexts == null)
                    {
                        _sRGBColorContexts = new ReadOnlyCollection<ColorContext>(new ColorContext[] { sRGB }); ;
                    }
                }
            }
            return _sRGBColorContexts;
        }
    }
}
