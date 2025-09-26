using System.Reflection;

namespace Snebur.Dominio.Atributos;

[IgnorarInterfaceTS]
public interface IDomainAtributo
{

}

[IgnorarInterfaceTS]
public interface IAtributoValidacao : IDomainAtributo
{
    bool IsValido(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade);

    string RetornarMensagemValidacao(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade);
}

[IgnorarInterfaceTS]
public interface IAtributoValidacaoEntidade : IDomainAtributo
{
    bool IsValido(object servico, List<Entidade> todasEntidades, Entidade entidade);

    string RetornarMensagemValidacao(Entidade entidade);
}