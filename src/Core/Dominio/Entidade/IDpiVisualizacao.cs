using Snebur.Dominio.Atributos;
using System;

namespace Snebur.Dominio
{
    public interface IDpiVisualizacao
    {
        [IgnorarPropriedadeTS]
        Func<double> FuncaoDpiVisualizacao { get; set; }
    }
}
