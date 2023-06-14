using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal class ComandoCampoComputado : Comando
    {
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
            sb.AppendFormat(" SELECT [{0}] FROM [{1}].[{2}] WHERE  [{1}].[{2}] .[{3}] = {4} ",
                            this.EstruturaCampo.NomeCampo,
                            estruturaEntidade.Schema,
                            estruturaEntidade.NomeTabela,
                            estruturaEntidade.EstruturaCampoChavePrimaria.NomeCampo,
                            estruturaEntidade.EstruturaCampoChavePrimaria.NomeParametro);

            this.EstruturasCampoParametro.Add(this.EstruturaEntidade.EstruturaCampoChavePrimaria);
            return sb.ToString();
        }
    }
}