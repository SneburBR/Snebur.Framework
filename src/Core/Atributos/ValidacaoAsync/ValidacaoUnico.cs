using Snebur.Utilidade;
using System;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidacaoUnicoAttribute : BaseAtributoValidacaoAsync, IAtributoValidacao, IAtributoMigracao
    {
        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "O {0} '{1}' já existe.";
        public bool IsPermitirDuplicarNulo { get; set; }
        public bool IsPermitirDuplicarZero { get; set; }
        public Type TipoEntidade { get; }
        public string NomePropriedadeFiltro { get; }
        public object ValorPropriedadeFiltro { get; }
        public EnumOperadorComparacao OperadorFiltro { get; }

        public bool IsAceitaNulo { get; set; }

        [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
        public bool IsIgnorarMigracao { get; set; }

        public ValidacaoUnicoAttribute(bool isAceitaNulo = true)
        {
            this.IsAceitaNulo = isAceitaNulo;

        }

        #region IAtributoValidacao

        public bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            if (!ValidacaoUtil.IsDefinido(valorPropriedade))
            {
                return !ValidacaoUtil.IsPropriedadeRequerida(propriedade);
            }
            // throw new ErroNaoImplementado("Validacao unico não implementado");
            return true;
        }

        public string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            return String.Format(MensagemValidacao, rotulo, valorPropriedade?.ToString() ?? "null");
        }
        #endregion
    }
}