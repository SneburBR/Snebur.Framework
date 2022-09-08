using Snebur.AcessoDados.Estrutura;
using Snebur.Utilidade;
using System;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal class CampoComputado
    {
        internal EstruturaCampo EstruturaCampo { get; set; }

        internal object Valor { get; set; }

        internal CampoComputado(EstruturaCampo estruturaCampo, object valor)
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
}