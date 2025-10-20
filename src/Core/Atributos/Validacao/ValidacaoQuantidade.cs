using System.Reflection;

namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property)]
public class ValidacaoQuantidadeAttribute : BaseAtributoValidacao, IAtributoValidacao
{
    [MensagemValidacao]
    public static string MensagemValidacao { get; } = "O campo '{0}' deve ser um número não negativo.";

    public override bool IsValido(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade)
    {
        if (!ValidacaoUtil.IsDefinido(valorPropriedade))
        {
            return true;
        }
         
        if (valorPropriedade is int @int)
        {
            return @int >= 0;
        }

        if (valorPropriedade is long @long)
        {
            return @long >= 0;
        }

        if (valorPropriedade is decimal @decimal)
        {
            return @decimal >= 0;
        }

        if (valorPropriedade is double @double)
        {
            return @double >= 0;
        }

        if (valorPropriedade is byte @byte)
        {
            return @byte >= byte.MinValue;
        }
        if (valorPropriedade is float @float)
        {
            return @float >= byte.MinValue;
        }

        throw new ArgumentException($"O tipo '{valorPropriedade.GetType()}' não é suportado para validação de quantidade.");
    }

    public override string RetornarMensagemValidacao(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade)
    {
        var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
        return String.Format(MensagemValidacao, rotulo);
    }
}