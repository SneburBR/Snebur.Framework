
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Snebur.Utilidade;

namespace Snebur.Imagem
{
    public class RedimensionarImagem : BaseProcessarImagem
    {

        public RedimensionarImagem(Stream stream, int alturaMaxima, double dpi = DPI_PADRAO) :
                                   base(stream, alturaMaxima, EnumLadoComprimento.Altura, dpi, true)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    throw new Exception("Usar o Workder thread ou STA para essa operação");
                }
            }
        }

        public RedimensionarImagem(Stream stream, Size tamanho, 
                                    EnumOpcaoRedimensionar opacaoRedimensionar = EnumOpcaoRedimensionar.UniformeParaPrencher,
                                    bool aumentarImagem = false, double dpi = DPI_PADRAO) :
                                    base(stream, tamanho, opacaoRedimensionar, aumentarImagem, dpi, true)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    throw new Exception("Usar o Workder thread ou STA para essa operação");
                }
            }
        }

        public BitmapSource RetornarImagem()
        {
            var opacao = this.RetornarCacheOptionAutomatico();
            return this.RetornarImagemInterno(this.Stream, opacao, true);
        }
 
        //public BitmapSource Redimensionar(BitmapSource imagem, double scalarX, double scalarY)
        //{
        //    var largura = imagem.PixelWidth * scalarX;
        //    var altura = imagem.PixelHeight * scalarY;
        //    return this.Redimensionar(imagem, largura, altura);
        //}

        //public BitmapSource Redimensionar(BitmapSource imagem, int alturaMaxima)
        //{
        //    var scalar = alturaMaxima / (double)imagem.PixelHeight;
        //    var largura = (int)Math.Round(imagem.PixelHeight * scalar);
        //    return this.Redimensionar(imagem, largura, alturaMaxima);
        //}

        //public BitmapSource Redimensionar(BitmapSource imagem, int largura, int altura)
        //{
        //    var dpi = (int)Math.Max(imagem.DpiX, imagem.DpiY);
        //    return this.Redimensionar(imagem, largura, altura, true, dpi);
        //}

        //public BitmapSource Redimensionar(BitmapSource imagem, int largura, int altura, bool uniforme, int dpi)
        //{


        //    var bitmapSourceRedimensionado = this.RedimensionarInterno(imagem, largura, altura, uniforme, dpi);
        //    if (imagem.IsBgraFormatoSuprotado())
        //    {
        //        return bitmapSourceRedimensionado;
        //    }
        //    return new FormatConvertedBitmap(bitmapSourceRedimensionado, imagem.Format, null, 0);
        //}

        ///// <summary>
        ///// Ao redimensionar um frame, o formato e perfil da cores são mantidos
        ///// </summary>
        ///// <param name="imagem"></param>
        ///// <param name="largura"></param>
        ///// <param name="altura"></param>
        ///// <param name="uniforme"></param>
        ///// <returns></returns>
        //public BitmapSource RedimensionarFrame(BitmapFrame frame, int largura, int altura, bool uniforme)
        //{
        //    var dpi = (int)Math.Max(frame.DpiX, frame.DpiY);
        //    return RedimensionarFrame(frame, largura, altura, uniforme, dpi);
        //}

        //public BitmapSource RedimensionarFrame(BitmapFrame frame, int largura, int altura, bool uniforme, int dpi)
        //{
        //    var perfilOrigem = frame.RetornarPerfilNativo();
        //    var perfilsRGB = PerfilIccUtil.RetornarPerfilNativo(EnumPerfilIcc.sRGB);

        //    using (var converterSrgb = new ConverterPerfilNativo(perfilsRGB))
        //    {
        //        var imagemSrgb = converterSrgb.RetornarBitmapSourceConvertido(frame);
        //        var imagemRedimensionada = this.RedimensionarInterno(imagemSrgb, largura, altura, uniforme, dpi);


        //        using (var converterOriginal = new ConverterPerfilNativo(perfilOrigem))
        //        {
        //            return converterOriginal.RetornarBitmapSourceConvertido(imagemRedimensionada, perfilsRGB);
        //        }
        //    }
        //}

        //private BitmapSource RedimensionarInterno(BitmapSource imagem, int largura, int altura, bool uniforme, int dpi)
        //{
        //    if (uniforme)
        //    {
        //        var tamanhoUniforme = ImagemUtil.RetornaTamanhoUniforme(imagem.PixelWidth, imagem.PixelHeight, largura, altura);
        //        largura = (int)tamanhoUniforme.Width;
        //        altura = (int)tamanhoUniforme.Height;
        //    }

        //    var scalarX = (double)largura / imagem.PixelWidth;
        //    var scalarY = (double)altura / imagem.PixelHeight;
        //    var imagemReduzida = new TransformedBitmap(imagem, new ScaleTransform(scalarX, scalarY));
        //    if (dpi > 0 && (int)imagemReduzida.DpiX != dpi || (int)imagemReduzida.DpiY != dpi)
        //    {
        //        return BitmapSourceUtil.AjustarDpi(imagemReduzida, dpi);
        //    }
        //    return imagemReduzida;

        //}


    }

}
