using System.Reflection;

namespace Snebur.Dominio.Atributos;

public class ValidacaoDataFimAttribute : BaseAtributoValidacao, IAtributoValidacao
{
    [MensagemValidacao]
    public static string MensagemValidacao { get; } = "A '{0}' deve ser superior a data de inicio.";

    [MensagemValidacao]
    public static string MensagemValidacaoComposta { get; } = "A '{0}' deve ser superior à '{1}'.";
    public string NomePropriedadeDataInicio { get; set; }

    public ValidacaoDataFimAttribute(string nomePropriedadeDataInicio)
    {
        this.NomePropriedadeDataInicio = nomePropriedadeDataInicio;
    }

    public override bool IsValido(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade)
    {
        if (paiPropriedade != null && valorPropriedade is DateTime dataFim)
        {
            var propriedadeDataInicio = paiPropriedade.GetType().GetProperty(this.NomePropriedadeDataInicio);
            if (propriedadeDataInicio == null)
            {
                throw new Erro($"A propriedade {this.NomePropriedadeDataInicio} não foi encontrado na tipo {paiPropriedade.GetType().Name}");
            }

            var tipoPropriedadeInicio = ReflexaoUtil.RetornarTipoSemNullable(propriedadeDataInicio.PropertyType);
            if (tipoPropriedadeInicio != typeof(DateTime))
            {
                throw new Erro($" O tipo '{tipoPropriedadeInicio.Name}' da propriedade da data inicio não é suportado. Esperado '{nameof(DateTime)} ");
            }

            var dataInicio = ReflexaoUtil.RetornarValorPropriedade(paiPropriedade, propriedadeDataInicio);
            if (dataInicio is DateTime dataPublicaacaoTipada)
            {
                return dataFim.AddDays(1).RetornarDataComHoraZerada() > dataPublicaacaoTipada;
            }

            return false;
        }

        return true;
    }

    public override string RetornarMensagemValidacao(PropertyInfo propriedade, object? paiPropriedade, object? valorPropriedade)
    {
        var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
        return String.Format(MensagemValidacao, rotulo);
    }
}