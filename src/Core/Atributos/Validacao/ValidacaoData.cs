using Snebur.Utilidade;
using System;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoDataAttribute : BaseAtributoValidacao, IAtributoValidacao
    {
        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "O campo {0} é invalido.";
        public DateTime? DataMaxima { get; set; }
        public DateTime? DataMinima { get; set; }
        public EnumTipoData TipoData { get; set; }

        [IgnorarConstrutorTS]
        public ValidacaoDataAttribute() : this(EnumTipoData.Normal, null, null)
        {
        }
        [IgnorarConstrutorTS]
        public ValidacaoDataAttribute(EnumTipoData dataTempoEnum) : this(dataTempoEnum, null, null)
        {
        }

        public ValidacaoDataAttribute(EnumTipoData tipoData,
                                      [ParametroOpcionalTS] DateTime? dataMinima,
                                      [ParametroOpcionalTS] DateTime? dataMaxima)
        {
            this.TipoData = tipoData;
            this.DataMinima = dataMinima ?? DataHoraUtil.RetornarDataMinima(tipoData);
            this.DataMaxima = dataMaxima ?? DataHoraUtil.RetornarDataMaxima(tipoData);
        }
        #region IAtributoValidacao

        public override bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            if (!ValidacaoUtil.IsDefinido(valorPropriedade))
            {
                return !ValidacaoUtil.IsPropriedadeRequerida(propriedade);
            }
            var dataComparar = Convert.ToDateTime(valorPropriedade);
            return dataComparar >= this.DataMinima && dataComparar <= this.DataMaxima;
        }

        public override string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            return String.Format(MensagemValidacao, rotulo);
        }
        #endregion
    }
}