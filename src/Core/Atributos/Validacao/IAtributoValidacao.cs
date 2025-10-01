using System.Reflection;

namespace Snebur.Dominio.Atributos;

[IgnorarInterfaceTS]
public interface IAtributoValidacao : IDomainAtributo
{
    bool IsValido(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade);

    string RetornarMensagemValidacao(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade);
}
