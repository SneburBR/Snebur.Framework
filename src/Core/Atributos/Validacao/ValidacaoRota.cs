using System.Reflection;

namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property)]
public class ValidacaoRotaAttribute : BaseAtributoValidacao
{
    [MensagemValidacao]
    public static string MensagemValidacao { get; } = "Rota invalida";

#region IAtributoValidacao
    public override bool IsValido(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade)
    {
        if (!ValidacaoUtil.IsDefinido(valorPropriedade))
        {
            return true;
        }

        return ValidacaoUtil.IsRota(Convert.ToString(valorPropriedade));
    }

    public override string RetornarMensagemValidacao(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade)
    {
        var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
        return String.Format(MensagemValidacao, rotulo);
    }
#endregion
}