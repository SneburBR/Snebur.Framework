namespace Snebur.AcessoDados.Mapeamento;

internal class SqlBuilder2008 : BaseSqlBuilder
{
    internal SqlBuilder2008(BaseMapeamentoEntidade mapeamentoConsulta) : base(mapeamentoConsulta) { }

    internal override string MontarSql(BaseFiltroMapeamento filtroMapeamento, 
                                       string sqlCampos, 
                                       bool isIncluirOrdenacaoPaginacao, 
                                       bool isRelacaoFilhos)
    {
        if (!isIncluirOrdenacaoPaginacao)
        {
            return base.MontarSql(filtroMapeamento, 
                                  sqlCampos, 
                                  isIncluirOrdenacaoPaginacao, 
                                  isRelacaoFilhos);
        }

        if(this.Skip> 0)
        {
            return this.MontarSqlPaginacao(filtroMapeamento,
                                           sqlCampos,
                                           isRelacaoFilhos);
        }

        var sqlConsulta= this.RetornarSqlConsulta(filtroMapeamento, 
                                                  isIncluirOrdenacaoPaginacao, 
                                                  isRelacaoFilhos);

        return $"SELECT TOP {this.Take} {sqlCampos} FROM {sqlConsulta}";
    }

    private string MontarSqlPaginacao(BaseFiltroMapeamento filtroMapeamento, 
                                      string sqlCampos,
                                      bool isRelacaoFilhos)
    {
        var sqlOrdenacao = this.RetornarSqlOrdenacao(filtroMapeamento);
        var sqlConsulta = this.RetornarSqlConsulta(filtroMapeamento, false, isRelacaoFilhos);
        var sb = new StringBuilderSql();
        sb.AppendLine("WITH ConsultaPaginada AS ");
        sb.AppendLine("(");
        sb.AppendLine($"    SELECT ROW_NUMBER() OVER ({sqlOrdenacao}) AS RowNumber, {sqlCampos}");
        sb.AppendLine($"           FROM {sqlConsulta}");
        sb.AppendLine(")");
        sb.AppendLine($"SELECT * FROM ConsultaPaginada WHERE RowNumber BETWEEN {this.Skip + 1} AND {this.Skip + this.Take}");
        return sb.ToString();
    }
}
