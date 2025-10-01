using System.Reflection;

namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property)]
public class ValidacaoPalavraTamanhoAttribute : BaseAtributoValidacao, IAtributoValidacao
{
    //private const int ACRECIMO_DELETADO = 36;
    //private const int ACRECIMO_DELETADO = 0;
    [MensagemValidacao]
    public static string MensagemValidacaoMaximo { get; } = "A palavra '{0}' deve ter no máximo {1} caracteres.";

    [MensagemValidacao]
    public static string MensagemValidacaoMinimo { get; } = "A palavra '{0}' deve ter no mínimo {1} caracteres.";

    [MensagemValidacao]
    public static string MensagemValidacaoIntervalo { get; } = "O campo '{0}' deve ter entre {1} e {2} caracteres.";
    public int TamanhoMinimo { get; set; }
    public int TamanhoMaximo { get; set; }

#region Construtores
    [IgnorarConstrutorTS]
    public ValidacaoPalavraTamanhoAttribute(int tamanhoMaximo)
    {
        this.TamanhoMaximo = tamanhoMaximo;
    }

    public ValidacaoPalavraTamanhoAttribute(int tamanhoMinimo, int tamanhoMaximo) : this(tamanhoMaximo)
    {
        this.TamanhoMinimo = tamanhoMinimo;
    }

#endregion
#region IAtributoValidacao
    public override bool IsValido(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade)
    {
        if (!ValidacaoUtil.IsDefinido(valorPropriedade))
        {
            return true;
        }

        var texto = Convert.ToString(valorPropriedade);
        return ValidacaoUtil.ValidarPalavraTamanho(texto, this.TamanhoMinimo, this.TamanhoMaximo);
    }

    public override string RetornarMensagemValidacao(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade)
    {
        //var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
        var valorPropriedadeString = valorPropriedade?.ToString();
        var palavraInvalida = String.IsNullOrWhiteSpace(valorPropriedadeString) ? null : new Regex(@"\s+").Split(valorPropriedadeString).Where(x => !ValidacaoUtil.ValidarTextoTamanho(x, this.TamanhoMinimo, this.TamanhoMaximo)).First();
        if (this.TamanhoMinimo > 0 && this.TamanhoMaximo > 0)
        {
            return String.Format(MensagemValidacaoIntervalo, palavraInvalida, this.TamanhoMinimo, this.TamanhoMaximo);
        }

        if (this.TamanhoMinimo > 0)
        {
            return String.Format(MensagemValidacaoMinimo, palavraInvalida, this.TamanhoMinimo);
        }

        if (this.TamanhoMaximo > 0)
        {
            return String.Format(MensagemValidacaoMaximo, palavraInvalida, this.TamanhoMaximo);
        }

        throw new ErroOperacaoInvalida("O tamanho minimo e o tamanho máximo não foram definido");
    }
#endregion
}