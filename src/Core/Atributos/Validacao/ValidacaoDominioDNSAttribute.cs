using System.Reflection;

namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property)]
public class ValidacaoDominioDNSAttribute : ValidacaoDominioAttribute, IAtributoValidacao
{
    #region IAtributoValidacao

    public override bool IsValido(
        PropertyInfo propriedade,
        object? paiPropriedade,
        object? valorPropriedade)
    {
        if (!ValidacaoUtil.IsDefinido(valorPropriedade))
        {
            return true;
        }
        return ValidacaoUtil.IsDominioDns(Convert.ToString(valorPropriedade));
    }

    public override string RetornarMensagemValidacao(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade)
    {
        var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
        return String.Format(MensagemValidacao, rotulo);
    }
    #endregion
}