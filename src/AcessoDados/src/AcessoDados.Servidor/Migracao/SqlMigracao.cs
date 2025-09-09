using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados;

internal abstract class SqlMigracao : IDisposable
{

    internal EstruturaEntidade EstruturaEntidade { get; }
    internal string Schema { get; }
    internal string NomeTabela { get; }
    internal string? GrupoArquivoIndices { get; }
    internal List<EstruturaCampo> EstruturasCampo { get; }
    internal List<string> Campos { get; }
    internal List<PropertyInfo> Propriedades { get; }

    internal SqlMigracao(EstruturaEntidade estruturaEntidade,
                         List<PropertyInfo> propriedades)
    {
        this.EstruturasCampo = new List<EstruturaCampo>();
        this.Campos = new List<string>();

        this.EstruturaEntidade = estruturaEntidade;
        this.Propriedades = propriedades;

        this.NomeTabela = estruturaEntidade.NomeTabela;
        this.Schema = estruturaEntidade.Schema;
        this.GrupoArquivoIndices = estruturaEntidade.GrupoArquivoIndices;

        foreach (var propriedade in propriedades)
        {
            var estruturaCampo = this.RetornarEstruturaCampo(propriedade);
            this.EstruturasCampo.Add(estruturaCampo);
            this.Campos.Add(estruturaCampo.NomeCampo);
        }
    }

    internal string RetornarSql()
    {
        switch (ConfiguracaoAcessoDados.TipoBancoDadosEnum)
        {
            case (EnumTipoBancoDados.PostgreSQL):
            case (EnumTipoBancoDados.PostgreSQLImob):

                return this.RetornarSql_PostgreSQL();

            case (EnumTipoBancoDados.SQL_SERVER):

                return this.RetornarSql_SqlServer();

            default:

                throw new ErroNaoSuportado("O tipo do banco de dados não é suportado");
        }
    }

    protected abstract string RetornarSql_PostgreSQL();

    protected abstract string RetornarSql_SqlServer();

    public override string ToString()
    {
        return String.Format("{0}-{1}({2})", this.GetType().Name, this.EstruturaEntidade.TipoEntidade.Name, String.Join(",", this.Campos));
    }

    #region Métodos privados

    protected EstruturaCampo RetornarEstruturaCampo(PropertyInfo propriedade)
    {
        if (propriedade.PropertyType.IsSubclassOf(typeof(Entidade)))
        {
            propriedade = EntidadeUtil.RetornarPropriedadeChaveEstrangeira(this.EstruturaEntidade.TipoEntidade, propriedade);
        }
        return this.EstruturaEntidade.RetornarEstruturaCampo(propriedade.Name);
    }
    #endregion

    #region IDisposable 

    public void Dispose()
    {
        this.EstruturasCampo.Clear();
        this.Campos.Clear();
    }
    #endregion
}