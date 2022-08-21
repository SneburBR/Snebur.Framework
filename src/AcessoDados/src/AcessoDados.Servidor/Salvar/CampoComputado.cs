using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using System.Reflection;
using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal class CampoComputado
    {
        internal EstruturaCampo EstruturaCampo { get; set; }

        internal object Valor { get; set; }

        internal CampoComputado(EstruturaCampo estruturaCampo, object valor)
        {
            this.EstruturaCampo = estruturaCampo;

            if (valor== DBNull.Value)
            {
                this.Valor = null;
            }else
            {
                this.Valor = ConverterUtil.Converter(valor, this.EstruturaCampo.Propriedade.PropertyType);
            }
        }
    }
}