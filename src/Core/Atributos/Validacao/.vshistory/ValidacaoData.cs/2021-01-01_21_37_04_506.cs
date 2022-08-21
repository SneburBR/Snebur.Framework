﻿using System;
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

        public EnumDataTempoValidacao DataTempoEnum { get; set; }

        [IgnorarConstrutorTS]
        public ValidacaoDataAttribute() : this(EnumDataTempoValidacao.Nenhum, null, null)
        {
        }
        [IgnorarConstrutorTS]
        public ValidacaoDataAttribute(EnumDataTempoValidacao dataTempoEnum) : this(dataTempoEnum, null, null)
        {
        }

        public ValidacaoDataAttribute(EnumDataTempoValidacao dataTempoEnum, DateTime? dataMinima, DateTime? dataMaxima)
        {
            this.DataTempoEnum = dataTempoEnum;
            this.DataMinima = dataMinima ?? DateTime.Now.AddYears(-100);
            this.DataMaxima = dataMaxima ?? DateTime.Now.AddYears(+100);

            switch (dataTempoEnum)
            {
                case (EnumDataTempoValidacao.Futuro):

                    this.DataMinima = DateTime.Now;
                    break;

                case (EnumDataTempoValidacao.Passado):

                    this.DataMaxima = DateTime.Now;
                    break;
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

    public enum EnumDataTempoValidacao
    {
        Futuro = 1,
        Passado = 2,
        Nenhum = 3
    }
}