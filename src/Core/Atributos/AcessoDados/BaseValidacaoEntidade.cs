namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Class)]
public abstract class BaseValidacaoEntidadeAttribute : BaseAtributoDominio, IAtributoValidacaoEntidade
{

    public abstract bool IsValido(
        object contextoDados, 
        IReadOnlyCollection<Entidade> todasEntidades,
          Entidade entidade);

    public abstract string RetornarMensagemValidacao(Entidade entidade);

}
