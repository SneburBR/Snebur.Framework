using System.Reflection;

namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property)]
public class ValidacaoLongAttribute : BaseAtributoValidacao
{
    [MensagemValidacao]
    public static string MensagemValidacao { get; } = "O campo {0} é invalido";

    public ValidacaoLongAttribute()
    {
    }

    public override bool IsValido(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade)
    {
        throw new NotImplementedException();
    }

    public override string RetornarMensagemValidacao(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade)
    {
        throw new NotImplementedException();
    }
}