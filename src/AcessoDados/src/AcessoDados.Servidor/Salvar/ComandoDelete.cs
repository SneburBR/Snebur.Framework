using Snebur.AcessoDados.Estrutura;
using System;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal class ComandoDelete : Comando
    {

        internal bool IsComandoUpdade { get; set; }

        internal string SqlCommandoIDeletado { get; set; }

        internal ComandoDelete(EntidadeAlterada entidadeAlterada,
            EstruturaEntidade estruturaEntidade) : base(entidadeAlterada, estruturaEntidade)
        {
            this.SqlCommando = this.RetornarSqlCommando();

            if (this.EstruturaEntidade.IsSomenteLeitura)
            {
                throw new ErroSeguranca("Não é autorizado deletar uma entidade somente leitura", Servicos.EnumTipoLogSeguranca.AlterarandoEntidadeSomenteLeitura);
            }
        }

        private string RetornarSqlCommando()
        {
            return this.RetornarSqlCommnadoDelete();
        }

        private string RetornarSqlCommnadoDelete()
        {
            var sb = new StringBuilderSql();
            sb.AppendFormat(" DELETE FROM [{0}].[{1}] WHERE {2} = {3}", this.EstruturaEntidade.Schema, this.EstruturaEntidade.NomeTabela, this.EstruturaEntidade.EstruturaCampoChavePrimaria.NomeCampoSensivel, this.EntidadeAlterada.Entidade.Id);
            return sb.ToString();
        }

        internal string RetornarSqlCommandoUpdate()
        {
            throw new ErroNaoImplementado();
        }
    }
}
