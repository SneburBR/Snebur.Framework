using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Snebur.Utilidade;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoDataAttribute : ValidationAttribute, IAtributoValidacao
    {
        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "O campo {0} é invalido.";

        public DateTime DataMaxima { get; set; }

        public DateTime DataMinima { get; set; }

        public EnumDataTempoValidacao TipoData { get; set; }

        [IgnorarConstrutorTS]
        public ValidacaoDataAttribute() : this(EnumDataTempoValidacao.Nenhum, null, null)
        {
        }
        [IgnorarConstrutorTS]
        public ValidacaoDataAttribute(EnumDataTempoValidacao dataTempoEnum) : this(dataTempoEnum, null, null)
        {
        }

        public ValidacaoDataAttribute(EnumTipoData tipoData, DateTime? dataMinima, DateTime? dataMaxima)
        {
            this.TipoData = tipoData;
            this.DataMinima = dataMinima ?? this.RetornarDataMinima(tipoData);
            this.DataMaxima = dataMaxima ?? this.RetornarDataMaxima(tipoData);
        }

        private DateTime RetornarDataMinima(EnumTipoData tipoData)
        {
            var hoje = DateTime.Now;
            switch (tipoData)
            {
                case EnumTipoData.Normal:
                case EnumTipoData.DataMuitoPassado:

                    return hoje.AddHours(-100);

                case EnumTipoData.DataNascimento:

                    return hoje.AddHours(-120);

                case EnumTipoData.DataFutura:
                case EnumTipoData.DataMuitoFutura:

                    return hoje;

                case EnumTipoData.DataPassado:
                    return hoje.AddHours(-10);

                default:
                    throw new Exception("tipo de data não suportado");
            }
        }

        private DateTime RetornarDataMaxima(EnumTipoData tipoData)
        {
            var hoje = DateTime.Now;
            switch (tipoData)
            {
                case EnumTipoData.Normal:
                case EnumTipoData.DataMuitoFutura:
                    return hoje.AddHours(+100);
                case EnumTipoData.DataNascimento:
                case EnumTipoData.DataPassado:
                case EnumTipoData.DataMuitoPassado:
                    return hoje;

                case EnumTipoData.DataFutura:
                    return hoje.AddHours(+10);
                default:
                    throw new Exception("tipo de data não suportado");
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propriedade = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            var paiPropriedade = validationContext.ObjectInstance;
            var valorPropriedade = value;
            if (this.IsValido(propriedade, paiPropriedade, valorPropriedade))
            {
                return ValidationResult.Success;
            }
            else
            {
                this.ErrorMessage = this.RetornarMensagemValidacao(propriedade, paiPropriedade, valorPropriedade);
                return new ValidationResult(this.ErrorMessage, new List<string>() { validationContext.MemberName });
            }
        }
        #region IAtributoValidacao

        public bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            if (!ValidacaoUtil.IsDefinido(valorPropriedade))
            {
                return !ValidacaoUtil.IsPropriedadeRequerida(propriedade);
            }
            var dataComparar = Convert.ToDateTime(valorPropriedade);
            return dataComparar >= this.DataMinima && dataComparar <= this.DataMaxima;
        }

        public string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            return String.Format(MensagemValidacao, rotulo);
        }
        #endregion
    }


}

namespace Snebur.Dominio
{
    public enum EnumTipoData
    {
        Normal,
        DataNascimento,
        DataFutura,
        DataMuitoFutura,
        DataPassado,
        DataMuitoPassado,
    }
}