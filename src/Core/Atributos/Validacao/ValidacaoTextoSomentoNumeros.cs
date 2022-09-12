using Snebur.Utilidade;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoTextoSomentoNumerosAttribute : BaseAtributoValidacao, IAtributoValidacao
    {
        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "A campo {0} é invalido (somente números).";


        #region "Construtor"

        public ValidacaoTextoSomentoNumerosAttribute() : base()
        {
        }
        #endregion

        #region IAtributoValidacao

        public override bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            if (!ValidacaoUtil.IsDefinido(valorPropriedade))
            {
                return !ValidacaoUtil.IsPropriedadeRequerida(propriedade);
            }
            var textp = Convert.ToString(valorPropriedade);
            return ValidacaoUtil.IsSomenteNumeros(textp);
        }

        [Display]
        public override string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            return String.Format(MensagemValidacao, rotulo);
        }
        #endregion

    }
}