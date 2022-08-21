using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal class ComandoCampoComputado : Comando
    {

        internal EstruturaCampo EstruturaCampo { get; set; }

        internal ComandoCampoComputado(EntidadeAlterada entidadeAlterada, EstruturaEntidade estruturaEntidade, EstruturaCampo estruturaCampo) : base(entidadeAlterada, estruturaEntidade)
        {
            this.EstruturaCampo = estruturaCampo;
            this.SqlCommando = this.RetornarSqlComando();
        }

        private string RetornarSqlComando()
        {
            var sb = new StringBuilderSql();
            sb.AppendFormat(" SELECT [{0}] FROM [{1}].[{2}] WHERE  [{1}].[{2}] .[{3}] = {4} ",
                            this.EstruturaCampo.NomeCampo, this.EstruturaEntidade.Schema, this.EstruturaEntidade.NomeTabela,
                            this.EstruturaEntidade.EstruturaCampoChavePrimaria.NomeCampo,
                            this.EstruturaEntidade.EstruturaCampoChavePrimaria.NomeParametro);

            this.EstruturasCampoParametro.Add(this.EstruturaEntidade.EstruturaCampoChavePrimaria);
            return sb.ToString();
        }
    }
}