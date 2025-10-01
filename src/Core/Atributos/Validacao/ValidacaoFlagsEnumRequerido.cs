using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property)]
public class ValidacaoFlagsEnumRequeridoAttribute : RequiredAttribute, IAtributoValidacao
{
    [MensagemValidacao]
    public static string MensagemValidacao { get; } = "O campo {0} deve ser preenchido.";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var resultado = base.IsValid(value, validationContext);
        if (resultado is not null)
        {
            if (!String.IsNullOrWhiteSpace(validationContext.MemberName))
            {
                var propriedade = validationContext.GetRequiredProperty();
                if (propriedade is not null)
                {
                    var paiPropriedade = validationContext.ObjectInstance;
                    var valorPropriedade = value;
                    if (this.IsValido(propriedade, paiPropriedade, valorPropriedade))
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        this.ErrorMessage = this.RetornarMensagemValidacao(propriedade, paiPropriedade, valorPropriedade);
                        resultado.ErrorMessage = this.ErrorMessage;
                    }
                }
            }
        }

        return resultado;
    }

#region IAtributoValidacao
    public bool IsValido(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade)
    {
        if (valorPropriedade is null)
        {
            return false;
        }

        return ValidacaoUtil.IsFlagsEnumDefinida(propriedade.PropertyType, (Enum)valorPropriedade);
    }

    public string RetornarMensagemValidacao(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade)
    {
        var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
        return String.Format(MensagemValidacao, rotulo);
    }
#endregion
}