using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Servidor.Salvar;

internal class ComandoInsert : Comando
{
    internal override bool IsAdiconarParametrosChavePrimaria => false;
    internal bool IsRecuperarUltimoId => this.EstruturaEntidade.IsChavePrimariaAutoIncrimento;

    internal ComandoInsert(EntidadeAlterada entidadeAlterada,
                           EstruturaEntidade estruturaEntidade) : base(entidadeAlterada, estruturaEntidade)
    {


    }

    protected override string RetornarSqlComando()
    {
        if (this.EstruturaEntidade.IsChavePrimariaAutoIncrimento)
        {
            if (ConfiguracaoAcessoDados.TipoBancoDadosEnum != EnumTipoBancoDados.PostgreSQLImob)
            {
                //this.EstruturasCampoParametro.Add(this.EstruturaEntidade.EstruturaCampoNomeTipoEntidade);
            }
        }
        else
        {
            this.EstruturasCampoParametro.Add(this.EstruturaEntidade.EstruturaCampoChavePrimaria);
        }
        this.EstruturasCampoParametro.AddRange(this.EstruturaEntidade.EstruturasCampos.Values);

        var campos = this.EstruturasCampoParametro.Select(x => x.NomeCampoSensivel).ToList();
        var camposParametros = this.EstruturasCampoParametro.Select(x => x.NomeParametroOuValorFuncaoServidor).ToList();

        var sb = new StringBuilderSql();
        sb.Append($" INSERT INTO [{this.EstruturaEntidade.Schema}].[{this.EstruturaEntidade.NomeTabela}] ");
        sb.Append($"( {String.Join(",", campos)} ) ");
        sb.Append(" VALUES ");
        sb.AppendLine($"( {String.Join(",", camposParametros)} ); ");

        if (this.IsRecuperarUltimoId)
        {
            sb.Append("SELECT  SCOPE_IDENTITY();");

        }
        return sb.ToString();
    }
}