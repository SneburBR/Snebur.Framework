using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property)]
public abstract class BaseAtributoValidacao : ValidationAttribute, IAtributoValidacao
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {

        var propriedade = validationContext.GetRequiredProperty();
        var paiPropriedade = validationContext.ObjectInstance;
        var valorPropriedade = value;

        if (this.IsValido(propriedade, paiPropriedade, valorPropriedade))
        {
            return ValidationResult.Success;
        }
        else
        {
            this.ErrorMessage = this.RetornarMensagemValidacao(propriedade, paiPropriedade, valorPropriedade);
            return new ValidationResult(
                this.ErrorMessage,
                [validationContext.MemberName!]);
        }
    }

    public abstract bool IsValido(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade);

    public abstract string RetornarMensagemValidacao(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade);
}
