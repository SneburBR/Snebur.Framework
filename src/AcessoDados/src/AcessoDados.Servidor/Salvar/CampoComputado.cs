using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Servidor.Salvar;

internal class CampoComputado
{
    internal EstruturaCampo EstruturaCampo { get; }

    internal object? Valor { get; }

    internal CampoComputado(
        EstruturaCampo estruturaCampo,
        object? valor)
    {
        this.EstruturaCampo = estruturaCampo;

        if (valor == DBNull.Value)
        {
            this.Valor = null;
        }
        else
        {
            this.Valor = ConverterUtil.Converter(valor, this.EstruturaCampo.Propriedade.PropertyType);
        }
    }
}