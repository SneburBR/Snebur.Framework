namespace Snebur.Dominio.Atributos;

[IgnorarInterfaceTS]
public interface IAtributoValidacaoEntidade : IDomainAtributo
{
    bool IsValido(object servico, 
        IReadOnlyCollection<Entidade> todasEntidades,
        Entidade entidade);

    string RetornarMensagemValidacao(Entidade entidade);
}