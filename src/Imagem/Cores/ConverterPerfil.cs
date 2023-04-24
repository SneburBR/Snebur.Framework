using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Snebur.Utilidade;

namespace Snebur.Imagens
{
    public abstract partial class ConverterPerfil : IDisposable
    {
        public const int QUALIDADE_PADRAO = 92;

        protected Stream StreamOrigem { get; }
        protected string CaminhoArquivoOrigem { get; }
        protected ColorContext PerfilDestinoNativo { get; }
        protected int Qualidade { get; }
        protected bool AlterarDpi { get; }
        protected int Dpi { get; }
        private bool DispensarStreamOrigem { get; } = false;

        //public bool ManterPerfil { get; set; } = true;
        //public bool ManterMetadata { get; set; } = true;

        public ConverterPerfil(string caminhoOrigem, ColorContext perfilDestinoNativo, int qualidade, int dpi, bool alterarDpi)
        {
            this.AlterarDpi = alterarDpi;
            this.PerfilDestinoNativo = perfilDestinoNativo;
            this.CaminhoArquivoOrigem = caminhoOrigem;
            this.Qualidade = qualidade;
            this.Dpi = dpi;
            this.AlterarDpi = dpi == 0;
        }

        public ConverterPerfil(Stream streamOrigem, ColorContext perfilDestinoNativo, int qualidade, int dpi, bool alterarDpi)
        {
            this.AlterarDpi = alterarDpi;
            this.PerfilDestinoNativo = perfilDestinoNativo;
            this.StreamOrigem = streamOrigem;
            this.Qualidade = qualidade;
            this.Dpi = dpi;
        }

        protected ConverterPerfil(ColorContext perfilDestnoNativo)
        {
            this.PerfilDestinoNativo = perfilDestnoNativo;
        }

        //public static ConverterPerfil CriarInstancia(string caminhoOrigem, ColorContext perfilDestinoNativo, int qualidade, int dpi, bool alterarDpi)
        //{

        //}

        public static ConverterPerfil CriarInstancia(string caminhoOrigem, ColorContext perfilDestinoNativo, int qualidade, int dpi, bool alterarDpi, EnumFormatoCor corFormato)
        {
            return new ConverterPerfilNativo(caminhoOrigem, perfilDestinoNativo, qualidade, dpi, alterarDpi, corFormato);
        }

        //public static ConverterPerfil CriarInstanciaVinta(string caminhoOrigem, ColorContext perfilDestinoNativo, int qualidade, int dpi, bool alterarDpi)
        //{
        //    return new ConverterPerfilVinta(caminhoOrigem, perfilDestinoNativo, qualidade, dpi, alterarDpi);
        //}


        //public static ConverterPerfil ConverterPerfilVinta(string caminhoOrigem, ColorContext perfilDestinoNativo, int qualidade, int dpi, bool alterarDpi)
        //{

        //    if (!ImagemUtil.IsExtensaoJpeg(caminhoOrigem))
        //    {
        //        return new ConverterPerfilNativo(caminhoOrigem, perfilDestinoNativo, qualidade, dpi, alterarDpi);
        //    }

        //    using (var fs = StreamUtil.OpenRead(caminhoOrigem))
        //    {

        //        //var encoder = BitmapDecoder.Create(fs, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        //        var formatoOrigem = CorFormatoUtil.RetornarFormatoCor(caminhoOrigem);
        //        var formatoDestino = perfilDestinoNativo.RetornarCorFormato();

        //        switch (formatoOrigem)
        //        {
        //            case (EnumFormatoCor.Indexed):

        //                return new ConverterPerfilNativo(caminhoOrigem, perfilDestinoNativo, qualidade, dpi, alterarDpi);

        //            case (EnumFormatoCor.Rgb):

        //                if (formatoDestino != EnumFormatoCor.Rgb)
        //                {
        //                    if (SistemaUtil.IsWindowsXp)
        //                    {
        //                        LogUtil.ErroAsync(new Exception($"Não é possivel converter um imagem Rgb para Cmyk ou outro formato, não são suportado Windows xp {caminhoOrigem}"));
        //                        perfilDestinoNativo = PerfilIccUtil.RetornarPerfilNativoPadrao(EnumFormatoCor.Rgb);
        //                        return new ConverterPerfilMagick(caminhoOrigem, perfilDestinoNativo, qualidade, dpi, alterarDpi);
        //                    }
        //                    return new ConverterPerfilNativo(caminhoOrigem, perfilDestinoNativo, qualidade, dpi, alterarDpi);

        //                }
        //                //return new ConverterPerfilNativo(caminhoOrigem, perfilDestinoNativo, qualidade, dpi, alterarDpi);
        //                return new ConverterPerfilVinta(caminhoOrigem, perfilDestinoNativo, qualidade, dpi, alterarDpi);

        //            case (EnumFormatoCor.Cmyk):

        //                if (formatoDestino != EnumFormatoCor.Rgb)
        //                {
        //                    if (SistemaUtil.IsWindowsXp)
        //                    {
        //                        LogUtil.ErroAsync(new Exception($"Não é possivel converter um imagem Rgb para Cmyk ou outro formato, não são suportado Windows xp {caminhoOrigem}"));
        //                        perfilDestinoNativo = PerfilIccUtil.RetornarPerfilNativoPadrao(EnumFormatoCor.Rgb);
        //                    }
        //                }
        //                return new ConverterPerfilNativo(caminhoOrigem, perfilDestinoNativo, qualidade, dpi, alterarDpi);


        //            case (EnumFormatoCor.Grayscale):

        //                //return new ConverterPerfilMagick(caminhoOrigem, perfilDestinoNativo, qualidade, dpi, alterarDpi);
        //                return new ConverterPerfilNativo(caminhoOrigem, perfilDestinoNativo, qualidade, dpi, alterarDpi);

        //            default:

        //                return new ConverterPerfilNativo(caminhoOrigem, perfilDestinoNativo, qualidade, dpi, alterarDpi);
        //        }
        //    }

        //}

        protected Stream RetornarStreamOrigem()
        {
            if (File.Exists(this.CaminhoArquivoOrigem))
            {
                return StreamUtil.OpenRead(this.CaminhoArquivoOrigem);
            }

            if (this.StreamOrigem != null)
            {
                return StreamUtil.RetornarMemoryStream(this.StreamOrigem);
            }

            throw new Exception($" A Stream de origem não foi definida {nameof(this.StreamOrigem)} ");
        }

        public abstract void Salvar(string caminhoDestino);

        public abstract void Salvar(Stream streamDestino);

        public void Dispose()
        {
            if (this.DispensarStreamOrigem)
            {
                this.StreamOrigem?.Dispose();
            }
            //throw new NotImplementedException();
        }
    }
}
