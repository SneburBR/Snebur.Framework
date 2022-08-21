using Snebur.Utilidade;
using System;
using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    public class ValidacaoDataPublicacaoAttribute : BaseAtributoValidacao, IAtributoValidacao
    {
        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "A data {0} não pode ser no passado.";

        public ValidacaoDataPublicacaoAttribute()
        {
        }

        public override bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            if (valorPropriedade is DateTime dataPublicacao && paiPropriedade is Entidade entidadePai && entidadePai.Id == 0)
            {
                return dataPublicacao >= DateTime.Now.RetornarDataComHoraZerada();
            }
            return true;
        }

        public override string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            return String.Format(MensagemValidacao, rotulo);
        }
    }
}