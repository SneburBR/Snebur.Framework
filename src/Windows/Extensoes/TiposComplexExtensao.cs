using System.Windows;
using Snebur.Dominio;

namespace Snebur
{
    public static class TiposComplexExtensao
    {
        public static Size RetornarDimensaoWindows(this Dimensao dimensao)
        {
            return new Size(dimensao.Largura, dimensao.Altura);
        }

        public static Size RetornarDimensaoVisualizacaoWindows(this Dimensao dimensao)
        {
            return new Size(dimensao.LarguraVisualizacao, 
                            dimensao.AlturaVisualizacao);
        }

        public static Thickness RetornarMargemVisualizacaoWindows(this Margem margem)
        {
            return new Thickness(margem.EsquerdaVisualizacao.Value,
                                 margem.SuperiorVisualizacao.Value,
                                 margem.DireitaVisualizacao.Value,
                                 margem.InferiorVisualizacao.Value);
        }
    }
}
