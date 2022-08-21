using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public interface IMargem
    {
        double? Esquerda { get; set; }

        double? Superior { get; set; }

        double? Direita { get; set; }

        double? Inferior { get; set; }

        [IgnorarPropriedadeTS]
        int? EsquerdaVisualizacao { get; }

        [IgnorarPropriedadeTS]
        int? SuperiorVisualizacao { get; }

        [IgnorarPropriedadeTS]
        int? DireitaVisualizacao { get; }

        [IgnorarPropriedadeTS]
        int? InferiorVisualizacao { get; }
    }
}
