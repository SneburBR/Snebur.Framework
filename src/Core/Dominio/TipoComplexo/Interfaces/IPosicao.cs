using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public interface IPosicao
    {
        double X { get; set; }

        double Y { get; set; }

        [IgnorarPropriedadeTS]
        int XVisualizacao { get; }

        [IgnorarPropriedadeTS]
        int YVisualizacao { get; }
    }
}