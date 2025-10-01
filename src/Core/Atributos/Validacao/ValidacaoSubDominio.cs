using System.Reflection;

namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property)]
public class ValidacaoSubDominio : BaseAtributoValidacao, IAtributoValidacao
{
    [MensagemValidacao]
    public static string MensagemValidacao { get; } = "O campo {0} é invalido.";

#region IAtributoValidacao
    public override bool IsValido(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade)
    {
        if (!ValidacaoUtil.IsDefinido(valorPropriedade))
        {
            return true;
        }

        var subDominio = Convert.ToString(valorPropriedade);
        throw new NotImplementedException();
    }

    public override string RetornarMensagemValidacao(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade)
    {
        var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
        return String.Format(MensagemValidacao, rotulo);
    }
#endregion
}