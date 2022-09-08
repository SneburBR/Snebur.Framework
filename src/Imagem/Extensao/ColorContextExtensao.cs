using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;
using Snebur.Imagem;
using Snebur.Utilidade;

namespace System.Windows.Media
{
    public static class ColorContextExtensao
    {


        public static int RetornarTamanhoPerfil(this ColorContext colorContext)
        {
            var profileHeader = (object)typeof(ColorContext).GetField("_profileHeader", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(colorContext);
            if (profileHeader != null)
            {
                var tamanho = (object)profileHeader.GetType().GetField("phSize", BindingFlags.Instance | BindingFlags.Public).GetValue(profileHeader);
                return Convert.ToInt32(tamanho);
            }
            return 0;
        }

        public static bool IsPerfilsRGB(this ColorContext colorContext)
        {
            var isPerfilsRGB = colorContext.RetornarTamanhoPerfil() == PerfilIccUtil.TAMANHO_PERFIL_SRGB &&
                                             colorContext.RetornarCorFormato() == EnumFormatoCor.Rgb;

            if (isPerfilsRGB)
            {
                var checksum = colorContext.RetornarChecksum();
                if (checksum != PerfilIccUtil.CHECKSUM_SRGB)
                {
                    new Erro($"O checksum do possivel perfil sRGB é diferente porem mesmo tamanho  '{checksum}' ");
                }
            }
            return isPerfilsRGB;
        }

        public static bool IsPerfilAdobeRGB_1998(this ColorContext colorContext)
        {

            var tamanhoPerfil = colorContext.RetornarTamanhoPerfil();
            var isAdobeRGB = colorContext.RetornarCorFormato() == EnumFormatoCor.Rgb &&
                                         (tamanhoPerfil == PerfilIccUtil.TAMANHO_PERFIL_ADOBE_RGB_560 ||
                                          tamanhoPerfil == PerfilIccUtil.TAMANHO_PERFIL_ADOBE_RGB_940);

            if (isAdobeRGB)
            {
                var checksum = colorContext.RetornarChecksum();
                if (!PerfilIccUtil.ChecksumAdobeRGB.Contains(checksum))
                {
                    //checksum do primeiro 500 bytes =
                    var checksum500 = colorContext.RetornarChecksum();
                    if (checksum500 != PerfilIccUtil.CHECKSUM_ADOBE_RGB_500)
                    {
                        throw new Erro("O checksum do perfil Adobe RGB  não é conhecido");
                    }
                }
            }
            return isAdobeRGB;
        }

        //    //public static bool IsRGB(this ColorContext colorContext)
        //    //{
        //    //    throw new NotFiniteNumberException();
        //    //}

        public static EnumFormatoCor RetornarCorFormato(this ColorContext colorContext)
        {
            var colorSpace = (object)typeof(ColorContext).GetProperty("ColorSpaceFamily", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(colorContext);
            var tipo = colorSpace.GetType();
            //var valoresEnum = EnumUtil.RetornarValoresEnum(tipo);
            var descricao = EnumUtil.RetornarDescricao((Enum)colorSpace);
            switch (descricao)
            {
                case "Unknown":
                case "Srgb":
                case "ScRgb":
                case "Rgb":

                    return EnumFormatoCor.Rgb;

                case "Cmyk":

                    return EnumFormatoCor.Cmyk;

                case "Gray":

                    return EnumFormatoCor.Grayscale;

                case "Multichannel":

                    return EnumFormatoCor.Rgb;

                default:

                    return EnumFormatoCor.Rgb;

            }
            throw new NotImplementedException();
        }

        public static ColorContext RetornarPerfilNativo(this ReadOnlyCollection<ColorContext> colecao)
        {
            var contextos = colecao == null ? new List<ColorContext>() : colecao.ToList();
            // contextos = contextos.Where(x => x.RetornarCorFormato() == corFormato).ToList();
            if (contextos.Count > 1)
            {
                contextos = contextos.GroupBy(x => x.RetornarTamanhoPerfil()).Select(x => x.Last()).ToList();
                if (contextos.Count > 1)
                {
                    contextos = contextos.Where(x => x.RetornarTamanhoPerfil() != PerfilIccUtil.TAMANHO_PERFIL_SRGB).ToList();
                }
            }

            if (contextos.Count > 1)
            {
                contextos = contextos.GroupBy(x => x.RetornarChecksum()).Select(x => x.Last()).ToList();
            }

            var perfil = contextos.LastOrDefault();
            if (contextos.Count > 1)
            {
                if (perfil.IsPerfilsRGB())
                {
                    return PerfilIccUtil.RetornarPerfilNativo(EnumPerfilIcc.sRGB);
                }

                if (perfil.IsPerfilAdobeRGB_1998())
                {
                    return PerfilIccUtil.RetornarPerfilNativo(EnumPerfilIcc.AdobeRGB);
                }
            }
            return perfil;
        }


        public static ColorContext RetornarPerfilOrigem(this BitmapFrame frame)
        {
            if (frame.Format == PixelFormats.Cmyk32)
            {
                if (SistemaUtil.IsWindowsXp)
                {
                    return PerfilIccUtil.RetornarPerfilNativo(EnumPerfilIcc.sRGB);
                }
            }

            var perfilNativo = frame.ColorContexts?.RetornarPerfilNativo();
            if (perfilNativo != null)
            {
                var corFormatoImagem = CorFormatoUtil.RetornarFormatoCor(frame.Format);
                var corFormatoPerfil = perfilNativo.RetornarCorFormato();
                if (corFormatoImagem == corFormatoPerfil || (corFormatoImagem == EnumFormatoCor.Indexed && corFormatoPerfil == EnumFormatoCor.Rgb))
                {
                    return perfilNativo;
                }
            }
            return PerfilIccUtil.RetornarPerfilNativoPadrao(frame.Format);
        }

        public static ReadOnlyCollection<ColorContext> RetornarColorContexts(this ColorContext colorContext)
        {
            if (SistemaUtil.IsWindowsXp)
            {
                var perfilsRGB = PerfilIccUtil.RetornarPerfilNativo(EnumPerfilIcc.sRGB);
                return new ReadOnlyCollection<ColorContext>(new ColorContext[] { perfilsRGB });
            }
            return new ReadOnlyCollection<ColorContext>(new ColorContext[] { colorContext });
        }

        public static StandardColorSpace RetornarColorSpaceFamily(this ColorContext colorContext)
        {
            var propriedadeColorSpaceFamily = typeof(ColorContext).GetProperty("ColorSpaceFamily", BindingFlags.Instance | BindingFlags.NonPublic);
            var valor = propriedadeColorSpaceFamily?.GetValue(colorContext);
            if (valor != null)
            {

                if (Enum.TryParse<StandardColorSpace>(valor.ToString(), out StandardColorSpace resultado))
                {
                    return resultado;
                }
            }
            return StandardColorSpace.Rgb;
        }

        public static string RetornarChecksum(this ColorContext colorContext)
        {
            return ChecksumUtil.RetornarChecksum(colorContext.RetornarBytes().ToArray());
        }

        public static string RetornarChecksum500(this ColorContext colorContext)
        {
            return ChecksumUtil.RetornarChecksum(colorContext.RetornarBytes().Take(500).ToArray());
        }

        public static byte[] RetornarBytes(this ColorContext colorContext)
        {
            try
            {
                using (var stream = colorContext.OpenProfileStream())
                {
                    using (var ms = StreamUtil.RetornarMemoryStream(stream))
                    {
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception)
            {
                if (colorContext.ProfileUri != null)
                {
                    LogUtil.ErroAsync(new Exception($"O perfil {colorContext.ProfileUri.AbsoluteUri} não foi encotrado"));
                }
                return new byte[0];
            }
        }




        public static bool Igual(this ColorContext colorContext, ColorContext colorContextComparar)
        {
            if (colorContext.RetornarTamanhoPerfil() == colorContextComparar.RetornarTamanhoPerfil()
                && colorContext.RetornarCorFormato() == colorContextComparar.RetornarCorFormato())
            {
                if (DebugUtil.IsAttached)
                {
                    if (colorContext.RetornarChecksum() != colorContextComparar.RetornarChecksum())
                    {
                        throw new Exception("O checksum dos perfil são diferentes");
                    }
                }
                return true;
            }
            return false;
        }

        //public static ReadOnlyCollection<ColorContext> RetornarPerfil_Cmyk(this ColorContext contexto)
        //{
        //    return new ReadOnlyCollection<ColorContext>(new List<ColorContext>() { PerfilIccUtil.RetornarPerfil_sRGB() });
        //}

    }

    public enum StandardColorSpace
    {
        Unknown = 0,
        Srgb = 1,
        ScRgb = 2,
        Rgb = 3,
        Cmyk = 4,
        Gray = 6,
        Multichannel = 7
    }

}
