using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using Snebur.Utilidade;

namespace Snebur.Imagens
{
    public partial class PerfilIccUtil
    {

        public static List<FileInfo> RetornarArquivosPerfilIccUsuario()
        {
            return RetornarArquivosPerfilIcc(PerfilIccUtil.CaminhoRepositorioPerfilIccUsuario);
        }

        public static string CaminhoRepositorioPerfilIcc
        {
            get
            {
                const string NOME_DIRETORIO_PERFIS = "Perfil";
                var diretorio = Path.Combine(ConfiguracaoUtil.CaminhoAppDataAplicacaoSemVersao, NOME_DIRETORIO_PERFIS);
                DiretorioUtil.CriarDiretorio(diretorio);
                return diretorio;
            }
        }

        public static string CaminhoRepositorioPerfilIccUsuario
        {
            get
            {
                const string NOME_DIRETORIO_PERFIL_CLIENTE = "Cliente";
                var diretorio = Path.Combine(PerfilIccUtil.CaminhoRepositorioPerfilIcc, NOME_DIRETORIO_PERFIL_CLIENTE);
                DiretorioUtil.CriarDiretorio(diretorio);
                return diretorio;
            }
        }

        public static string CaminhoRepositorioPerfilIccTemp
        {
            get
            {
                const string NOME_DIRETORIO_PERFIL_TEMP = "Temp";
                var diretorio = Path.Combine(PerfilIccUtil.CaminhoRepositorioPerfilIcc, NOME_DIRETORIO_PERFIL_TEMP);
                DiretorioUtil.CriarDiretorio(diretorio);
                return diretorio;
            }
        }

        public static List<FileInfo> RetornarArquivosPerfilIcc(string caminhoDiretorio)
        {
            var arquivos = new List<FileInfo>();
            var di = new DirectoryInfo(caminhoDiretorio);
            foreach (var extensao in PerfilIccUtil.ExtencoesPerfilIcc)
            {
                var filtrar = String.Format("*{0}", extensao);
                arquivos.AddRange(di.GetFiles(filtrar));
            }
            return arquivos;
        }

        public static bool ArquivoIccValido(string caminhoArquivo)
        {
            return PerfilIccUtil.ArquivoIccValido(new FileInfo(caminhoArquivo));
        }

        public static bool ArquivoIccValido(FileInfo arquivo)
        {
            if (!arquivo.Exists)
            {
                return false;
            }

            try
            {
                var colorContext = new ColorContext(new Uri(arquivo.FullName, UriKind.Absolute));
                var cor = colorContext.RetornarCorFormato();
                switch (cor)
                {
                    case EnumFormatoCor.Rgb:
                    case EnumFormatoCor.Cmyk:
                    case EnumFormatoCor.Indexed:
                    case EnumFormatoCor.Grayscale:
                    case EnumFormatoCor.Desconhecido:

                        return true;

                    default:
                        return false;

                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ExistePerfilIccUsuario(string nameArquivo)
        {
            var caminho = RetornarCaminhoPerfilIccUsuario(nameArquivo);
            return File.Exists(caminho);
        }

        public static string RetornarCaminhoPerfilIccUsuario(string nomeArquivo)
        {
            return Path.Combine(CaminhoRepositorioPerfilIccUsuario, nomeArquivo);
        }

        public static bool SalvarPerfilIccUsuario(FileInfo arquivo)
        {
            try
            {
                var caminhoDestino = Path.Combine(CaminhoRepositorioPerfilIccUsuario, arquivo.Name);
                arquivo.CopyTo(caminhoDestino);
                return true;
            }
            catch
            {
                return false;
            }

        }

        //public static ColorProfile RetornarPerfilMagick(string nomePerfil)
        //{
        //    if (!String.IsNullOrEmpty(nomePerfil))
        //    {
        //        var tipoPerfil = PerfilIccUtil.RetornarPerfilIccConhecido(nomePerfil);
        //        switch (tipoPerfil)
        //        {
        //            case (EnumPerfilIcc.AdobeRGB):

        //                return ColorProfile.AdobeRGB1998;

        //            case (EnumPerfilIcc.sRGB):

        //                return ColorProfile.SRGB;

        //            case (EnumPerfilIcc.AppleRGB):

        //                return ColorProfile.AppleRGB;

        //            case (EnumPerfilIcc.ColorMatchRGB):

        //                return ColorProfile.ColorMatchRGB;

        //            case (EnumPerfilIcc.CmykUSWebCoatedSWOP):

        //                return ColorProfile.USWebCoatedSWOP;

        //            case (EnumPerfilIcc.CmykCoatedFOGRA39):

        //                return ColorProfile.CoatedFOGRA39;

        //            case EnumPerfilIcc.Outro:

        //                return new ColorProfile(File.ReadAllBytes(RetornarCaminhoPerfilIccUsuario(nomePerfil)));

        //            default:

        //                throw new Erro(String.Format("O tipo do perfil não é suportado {0}", EnumUtil.RetornarDescricao(tipoPerfil)));
        //        }
        //    }
        //    return null;
        //}

        public static ColorContext RetornarPerfilNativo(string nomePerfil)
        {
            var tipoPerfil = PerfilIccUtil.RetornarPerfilIccConhecido(nomePerfil);
            if (tipoPerfil != EnumPerfilIcc.Outro)
            {
                return RetornarPerfilNativo(tipoPerfil);
            }
            else
            {
                var caminhoPerfil = RetornarCaminhoPerfilIccUsuario(nomePerfil);
                return new ColorContext(new Uri(caminhoPerfil, UriKind.Absolute));
            }
        }

        public static ColorContext RetornarPerfilNativo(EnumPerfilIcc perfil)
        {
            var caminhoPerfil = PerfilIccUtil.RetornarCaminhoPerfil(perfil);
            return new ColorContext(new Uri(caminhoPerfil, UriKind.Absolute));
        }

        public static ColorContext RetornarPerfilNativo_sGray()
        {
            return new ColorContext(new Uri(RetornarCaminhoPerfil_sGray(), UriKind.Absolute));
        }

        //public static ColorContext RetornarPerfilNativo(ColorProfile colorProfile)
        //{
        //    var nomeArquivo = String.Format("{0}.icc", Guid.NewGuid().ToString());
        //    var caminhoTemp = Path.Combine(CaminhoRepositorioPerfilIccTemp, nomeArquivo);
        //    File.WriteAllBytes(caminhoTemp, colorProfile.ToByteArray());
        //    return new ColorContext(new Uri(caminhoTemp, UriKind.Absolute));
        //}
 
        private static object _bloqueio = new object();
        public static string RetornarCaminhoPerfil(EnumPerfilIcc perfil)
        {
            if (perfil == EnumPerfilIcc.Outro)
            {
                throw new ErroNaoSuportado(String.Format("O perfil não é suportado {0}", EnumUtil.RetornarDescricao(perfil)));
            }

            var nomeArquivo = String.Format("{0}.icc", EnumUtil.RetornarDescricao(perfil));
            var caminhoPerfil = Path.Combine(PerfilIccUtil.CaminhoRepositorioPerfilIcc, nomeArquivo);
            if (!File.Exists(caminhoPerfil))
            {
                lock (_bloqueio)
                {
                    if (!File.Exists(caminhoPerfil))
                    {
                        using (var ms = PerfilIccUtil.RetornarStreamPerfil(perfil))
                        {
                            File.WriteAllBytes(caminhoPerfil, ms.ToArray());
                        }
                    }
               }

            }
            return caminhoPerfil;
        }

        public static MemoryStream RetornarStreamPerfil(EnumPerfilIcc perfil)
        {
            switch (perfil)
            {
                case (EnumPerfilIcc.sRGB):

                    return PerfilIccUtil.RetornarStreamPerfil("sRGB.icm");

                case (EnumPerfilIcc.AdobeRGB):

                    return PerfilIccUtil.RetornarStreamPerfil("AdobeRGB1998.icc");

                case (EnumPerfilIcc.AppleRGB):

                    return PerfilIccUtil.RetornarStreamPerfil("AppleRGB.icc");

                case (EnumPerfilIcc.ColorMatchRGB):

                    return PerfilIccUtil.RetornarStreamPerfil("ColorMatchRGB.icc");

                case (EnumPerfilIcc.CmykCoatedFOGRA39):

                    return PerfilIccUtil.RetornarStreamPerfil("CoatedFOGRA39.icc");

                case (EnumPerfilIcc.CmykUSWebCoatedSWOP):

                    return PerfilIccUtil.RetornarStreamPerfil("USWebCoatedSWOP.icc");

                default:

                    throw new ErroNaoSuportado(String.Format("O perfil não é suportado{0}", EnumUtil.RetornarDescricao(perfil)));
            }
        }

        public static string RetornarCaminhoPerfil_sGray()
        {
            var caminhoPerfil = Path.Combine(PerfilIccUtil.CaminhoRepositorioPerfilIcc, NOME_ARQUIVO_PERFIL_SGRAY);
            if (!File.Exists(caminhoPerfil))
            {
                using (var ms = PerfilIccUtil.RetornarStreamPerfil("sgray.icc"))
                {
                    File.WriteAllBytes(caminhoPerfil, ms.ToArray());
                }
            }
            return caminhoPerfil;
        }

        private static MemoryStream RetornarStreamPerfil(string nomeArquivoEmbutido)
        {
            var caminhoRecurso = String.Format("Snebur.Imagens.Recursos.PerfilIcc.{0}", nomeArquivoEmbutido);
            using (var stream = typeof(PerfilIccUtil).Assembly.GetManifestResourceStream(caminhoRecurso))
            {
                if (stream == null)
                {
                    throw new Exception($"O recurso {caminhoRecurso} não foi encontrado no assembly ");
                }

                return StreamUtil.RetornarMemoryStream(stream);
            }
        }

        public static ColorContext RetornarPerfilNativoPadrao(PixelFormat pixelFormat)
        {
            var corFormato = CorFormatoUtil.RetornarFormatoCor(pixelFormat);
            return RetornarPerfilNativoPadrao(corFormato);
        }

        public static ColorContext RetornarPerfilNativoPadrao(EnumFormatoCor corFormato)
        {

            switch (corFormato)
            {
                case (EnumFormatoCor.Cmyk):

                    return PerfilIccUtil.RetornarPerfilNativo(EnumPerfilIcc.CmykUSWebCoatedSWOP);

                case (EnumFormatoCor.Grayscale):

                    return PerfilIccUtil.RetornarPerfilNativo_sGray();

                case (EnumFormatoCor.Rgb):

                    return PerfilIccUtil.RetornarPerfilNativo(EnumPerfilIcc.sRGB);

                case (EnumFormatoCor.Indexed):

                    return PerfilIccUtil.RetornarPerfilNativo(EnumPerfilIcc.sRGB);

                default:

                    return PerfilIccUtil.RetornarPerfilNativo(EnumPerfilIcc.sRGB);

            }
        }

        //public static ColorProfile RetornarPerfilPadrao(ColorSpace colorSpace)
        //{

        //    switch (colorSpace)
        //    {
        //        case (ColorSpace.CMY):
        //        case (ColorSpace.CMYK):

        //            return ColorProfile.USWebCoatedSWOP;

        //        //return new ColorProfile(PerfilIccUtil.RetornarCaminhoPerfil_USWebCoatedSWOP());

        //        case (ColorSpace.Gray):

        //            var perfil = new ColorProfile(PerfilIccUtil.RetornarCaminhoPerfil_sGray());
        //            return perfil;

        //        case (ColorSpace.RGB):
        //        case (ColorSpace.sRGB):

        //            return ColorProfile.SRGB;
        //        //return new ColorProfile(PerfilIccUtil.RetornarCaminhoPerfil_sRGB());

        //        default:

        //            return ColorProfile.SRGB;
        //            //return new ColorProfile(PerfilIccUtil.RetornarCaminhoPerfil_sRGB());
        //    }
        //}

        //public static Tuple<ColorProfile, ColorSpace> RetornarColorProfileCorEspaco(string caminhoArquivo)
        //{
        //    using (var magic = new MagickImage(new FileInfo(caminhoArquivo)))
        //    {
        //        var perfil = magic.GetColorProfile();
        //        var espaco = magic.ColorSpace;
        //        return new Tuple<ColorProfile, ColorSpace>(perfil, espaco);
        //    }
        //}

        public static EnumFormatoCor RetornarCorFormato(EnumPerfilIcc perfil)
        {
            switch (perfil)
            {
                case (EnumPerfilIcc.AdobeRGB):
                case (EnumPerfilIcc.AppleRGB):
                case (EnumPerfilIcc.ColorMatchRGB):
                case (EnumPerfilIcc.sRGB):

                    return EnumFormatoCor.Rgb;

                case (EnumPerfilIcc.CmykCoatedFOGRA39):
                case (EnumPerfilIcc.CmykUSWebCoatedSWOP):

                    return EnumFormatoCor.Cmyk;

                default:

                    return EnumFormatoCor.Desconhecido;
            }

        }

        private static EnumPerfilIcc RetornarPerfilIccConhecido(string nomePerfil)
        {
            if (Enum.TryParse<EnumPerfilIcc>(nomePerfil, out var perfil))
            {
                return perfil;
            }
            return EnumPerfilIcc.Outro;
        }
    }
}
