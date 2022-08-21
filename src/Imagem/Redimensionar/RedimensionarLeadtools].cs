//using Leadtools.Codecs;
//using Leadtools.ImageProcessing.Core;
//using Leadtools.Windows.Media;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Windows.Media.Imaging;
//using Snebur.Utilidade;

//namespace Snebur.Imagem
//{
//    internal class RedimensionarLeadtools : IDisposable
//    {
//        internal BitmapSource Resultado { get; private set; }
//        internal BitmapSource Origem { get; private set; }
//        internal Exception Erro { get; private set; }
//        internal int Largura { get; }
//        internal int Altura { get; }

//        internal RedimensionarLeadtools(BitmapSource origem, int largura, int altura)
//        {
//            this.Origem = origem;
//            this.Largura = largura;
//            this.Altura = altura;
//        }
        
//        internal BitmapSource RetornarImagemRedimensionada()
//        {
//            using (AutoResetEvent are = new AutoResetEvent(false))
//            {
//                ThreadUtil.ExecutarAsync(() =>
//                {
//                    try
//                    {
//                        using (var codesc = new RasterCodecs())
//                        {
//                            using (var rasterImagem = RasterImageConverter.ConvertFromSource(this.Origem, ConvertFromSourceOptions.None))
//                            {
//                                var command = new ResizeInterpolateCommand();
//                                command.Width = this.Largura;
//                                command.Height = this.Largura;
//                                command.ResizeType = ResizeInterpolateCommandType.Lanczos;
//                                command.Run(rasterImagem);
//                                this.Resultado = RasterImageConverter.ConvertToSource(rasterImagem, ConvertToSourceOptions.None) as BitmapSource;
//                            }
//                        }
//                    }
//                    catch(Exception ex)
//                    {
//                        this.Erro = ex;
//                    }
//                });
//                are.WaitOne();

//                GC.Collect();
//                GC.WaitForPendingFinalizers();
//                Thread.Sleep(100);

//                if(this.Erro!= null)
//                {
//                    throw this.Erro;
//                }

//                return this.Resultado;
//            }
//        }
        
//        public void Dispose()
//        {
//            this.Origem = null;
//        }
//    }
//}

