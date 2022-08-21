using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Snebur.Utilidade;

namespace Snebur.Imagem
{

    public class RenderizacaoUtil
    {
        public const double POLEGADA = 2.54;
        public const int DPI_RENDERIZACAO = 96;


        public static void SalvarImagemImpressao(string caminhoOrigem,
                                                 string caminhoDestino,
                                                 UIElement elementoRenderizar,
                                                 Size tamanhoRenderizacao,
                                                 Size tamanhoImpressao,
                                                 double dpiImpressao,
                                                 int qualidadeJpeg = ConverterPerfilNativo.QUALIDADE_PADRAO)
        {

            //var tamanhoImpressao = new Size((tamanhoCentimetos.Width / POLEGADA) * dpiImpressao, (tamanhoCentimetos.Height / POLEGADA) * dpiImpressao);
            //var tamanhoRenderizacao = new Size(tamanhoImpressao.Width * scalarAumentarTamanhoRenderizar, tamanhoImpressao.Height * scalarAumentarTamanhoRenderizar);

            try
            {
                var scalarRedimensionarImpressaoX = tamanhoImpressao.Width / tamanhoRenderizacao.Width;
                var scalarRedimensionarImpressaoY = tamanhoImpressao.Height / tamanhoRenderizacao.Height;

                elementoRenderizar.Measure(tamanhoRenderizacao);
                elementoRenderizar.Arrange(new Rect(tamanhoRenderizacao));

                ArquivoUtil.DeletarArquivo(caminhoDestino);


                using (var fsDestino = StreamUtil.CreateWrite(caminhoDestino))
                {
                    //ColorContext perfilDestino;

                    //var formatoOrigem = EnumFormatoCor.Desconhecido;

                    using (var fsOrigem = StreamUtil.OpenRead(caminhoOrigem))
                    {
                        var decoder = DecoderUtil.RetornarDecoder(fsOrigem);

                        var frameOriginal = decoder.Frames[0];

                        //formatoOrigem = CorFormatoUtil.RetornarFormatoCor(frameOriginal.Format);
                        //perfilDestino = frameOriginal.RetornarPerfilOrigem();

                        //var qualidade = (perfilDestino.IsPerfilsRGB())  ? ConverterPerfilNativo.QUALIDADE_PADRAO : 100;
                        //qualidade = (SistemaUtil.IsWindowsXp) ? 100 : qualidade;


                        var imagemRenderizada = new RenderTargetBitmap(Convert.ToInt32(tamanhoRenderizacao.Width),
                                                                       Convert.ToInt32(tamanhoRenderizacao.Height), 96, 96, PixelFormats.Pbgra32);
                        RenderOptions.SetBitmapScalingMode(imagemRenderizada, BitmapScalingMode.NearestNeighbor);
                        imagemRenderizada.Render(elementoRenderizar);


                        if (ConfiguracaoTeste.SalvarImagemTemporaria)
                        {
                            var caminho = $@"c:\temp\ImagemRenderizada_SemPerfil_Antes__SetBitmapScalingMode___{Guid.NewGuid().ToString()}.jpg";
                            ImagemUtil.SalvarImagem(imagemRenderizada, caminho);
                        }


                        BitmapSource imagemFinal;
                        if (scalarRedimensionarImpressaoX > 0 &&
                            scalarRedimensionarImpressaoY > 0 &&
                            scalarRedimensionarImpressaoX != 1 &&
                            scalarRedimensionarImpressaoY != 1)
                        {
                            try
                            {
                                var scalarTransform = new ScaleTransform(scalarRedimensionarImpressaoX, scalarRedimensionarImpressaoY);
                                var imagemRedimensionada = new TransformedBitmap(imagemRenderizada, scalarTransform);
                                imagemFinal = BitmapSourceUtil.AjustarDpi(imagemRedimensionada, 300);
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    imagemFinal = BitmapSourceUtil.AjustarDpi(imagemRenderizada, dpiImpressao);
                                }
                                catch (Exception)
                                {
                                    imagemFinal = imagemRenderizada;
                                }
                            }
                        }
                        else
                        {
                            imagemFinal = BitmapSourceUtil.AjustarDpi(imagemRenderizada, 300); ;
                        }

                        if (ConfiguracaoTeste.SalvarImagemTemporaria)
                        {
                            var caminho = $@"c:\temp\ImagemRenderizada_SemPerfil_{Guid.NewGuid().ToString()}.jpg";
                            ImagemUtil.SalvarImagem(imagemRenderizada, caminho);
                        }

                        var perfil_sRGB = PerfilIccUtil.RetornarPerfilNativo(EnumPerfilIcc.sRGB);
                        var metadata = MetadataUtil.RetornarMetadata(frameOriginal, true);

                        var encoder = EncoderUtil.RetornarEncoder(decoder, qualidadeJpeg);
                        var frame = BitmapFrame.Create(imagemFinal, null, metadata, perfil_sRGB.RetornarColorContexts());
                        encoder.Frames.Add(frame);
                        encoder.Save(fsDestino);

                    }

                   
                    elementoRenderizar.Clear();


                }
            }
            catch (OutOfMemoryException ex)
            {
                new ErroMemoriaInsuficiente(caminhoOrigem, ex);
            }

        }

        //public static void Limpar(UIElement elemento)
        //{
        //    elemento.Measure(new Size(1, 1));
        //    elemento.Arrange(new Rect(new Size(1, 1)));
        //    elemento.UpdateLayout();
        //}
    }
}
