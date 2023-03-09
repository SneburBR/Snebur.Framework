using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Snebur.Utilidade;

namespace Snebur.Imagem
{
    public class ProcessarImagemVisualizacao : BaseProcessarImagem
    {
        private bool UsuarBitmapImage { get; }

        public ProcessarImagemVisualizacao(Stream stream, int alturaMaxima, bool usuarBitmapImage = false) : this(stream, alturaMaxima, EnumLadoComprimento.Altura)
        {
            this.UsuarBitmapImage = usuarBitmapImage;
        }

        public ProcessarImagemVisualizacao(Stream stream, int comprimentoMaximo, EnumLadoComprimento ladoComprimento, bool usuarBitmapImage = false) :
                                           base(stream, comprimentoMaximo, ladoComprimento, DPI_PADRAO_VISUALIZACAO, false)
        {
            this.UsuarBitmapImage = usuarBitmapImage;
        }

        public ProcessarImagemVisualizacao(Stream stream, Size tamanhho, EnumOpcaoRedimensionar opcaoRedimensionar, bool aumentarImagem, bool converterPerfilOrigem = false) :
                                          base(stream, tamanhho, opcaoRedimensionar, aumentarImagem, DPI_PADRAO_VISUALIZACAO, converterPerfilOrigem)
        {
            this.UsuarBitmapImage = false;  
        }

        public BitmapSource RetornarImagemVisualizacao()
        {
            //var opacao = this.RetornarCacheOptionAutomatico();
            this.CarregarMetaData = false;
            return this.RetornarImagemInterno(this.Stream, BitmapCacheOption.None, true);
        }

        protected override BitmapSource RetornarImagemErro(Exception ex)
        {
            if (!this.UsuarBitmapImage)
            {
                throw ex;
            }

            try
            {
                if (this.Stream.CanSeek)
                {
                    this.Stream.Seek(0, SeekOrigin.Begin);
                }

                var bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = this.Stream;

                if (this.LadoComprimento == EnumLadoComprimento.Altura)
                {
                    bi.DecodePixelHeight = this.ComprimentoMaximo;
                }

                if (this.LadoComprimento == EnumLadoComprimento.Largura)
                {
                    bi.DecodePixelWidth = this.ComprimentoMaximo;
                }

                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();
                bi.Freeze();
                return bi;

            }
            catch
            {
                throw ;
            }
        }


    }



}
