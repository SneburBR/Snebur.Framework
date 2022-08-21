using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public interface IOrdenacao /*: IEntidade*/
    {
        [Rotulo("Ordenação")]
        double? Ordenacao { get; set; }
    }

    public interface IOrdenacaoEntidade : IEntidade, IOrdenacao
    {
         
    }

}
