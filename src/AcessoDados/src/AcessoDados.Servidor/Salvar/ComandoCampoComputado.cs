using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal class ComandoCampoComputado : Comando
    {
        internal override bool IsAdiconarParametrosChavePrimaria { get; }= false;
        internal EstruturaCampo EstruturaCampo { get; }

        internal ComandoCampoComputado(EntidadeAlterada entidadeAlterada,
                                       EstruturaEntidade estruturaEntidade,
                                       EstruturaCampo estruturaCampo) : base(entidadeAlterada, estruturaEntidade)
        {
            this.EstruturaCampo = estruturaCampo;
            this.SqlCommando = this.RetornarSqlComando();
        }

        private string RetornarSqlComando()
        {
            var estruturaEntidade = this.EstruturaEntidade;
            var sb = new StringBuilderSql();
            sb.Append($" SELECT [{this.EstruturaCampo.NomeCampo}] FROM [{estruturaEntidade.Schema}].[{estruturaEntidade.NomeTabela}] WHERE  [{estruturaEntidade.Schema}].[{estruturaEntidade.NomeTabela}] .[{estruturaEntidade.EstruturaCampoChavePrimaria.NomeCampo}] = {estruturaEntidade.EstruturaCampoChavePrimaria.NomeParametro} ");
            this.EstruturasCampoParametro.Add(this.EstruturaEntidade.EstruturaCampoChavePrimaria);
            return sb.ToString();
        }
    }
}