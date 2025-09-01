using System.Drawing;
using Snebur.Dominio;

namespace Snebur;

public static class TiposComplexExtensao
{
    public static Size RetornarDimensaoDrawing(this Dimensao dimensao)
    {
        return new Size((int)dimensao.Largura, (int)dimensao.Altura);
    }

    public static Size RetornarDimensaoVisualizacaoDrawing(this Dimensao dimensao)
    {
        return new Size(dimensao.LarguraVisualizacao, dimensao.AlturaVisualizacao);
    }
}
