using System.Reflection;

namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property)]
public class ValidacaoQuantidadeAttribute : BaseAtributoValidacao, IAtributoValidacao
{
    [MensagemValidacao]
    public static string MensagemValidacao { get; set; } = "O campo '{0}' deve ser um número não negativo.";

    public override bool IsValido(PropertyInfo propriedade,
        object? paiPropriedade,
        object? valorPropriedade)
    {
        if (!ValidacaoUtil.IsDefinido(valorPropriedade))
        {
            return true;
        }

        if (valorPropriedade is int quantidade)
        {
            return quantidade >= 0;
        }
        if (valorPropriedade is long quantidadeLong)
        {
            return quantidadeLong >= 0;
        }

        if (valorPropriedade is decimal quantidadeDecimal)
        {
            return quantidadeDecimal >= 0;
        }

        if (valorPropriedade is double quantidadeDouble)
        {
            return quantidadeDouble >= 0;
        }

        throw new ArgumentException($"O tipo '{valorPropriedade.GetType()}' não é suportado para validação de quantidade.");
    }

    public override string RetornarMensagemValidacao(PropertyInfo propriedade,
        object? paiPropriedade,
        object? valorPropriedade)
    {
        var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
        return String.Format(MensagemValidacao, rotulo);
    }
}
