using Snebur.AcessoDados.Estrutura;
using System.Collections.Generic;

namespace Snebur.AcessoDados.Mapeamento;

internal abstract partial class BaseSqlBuilder
{
    private BaseMapeamentoEntidade _mapeamentoConsulta;
    protected EstruturaEntidadeApelido EstruturaEntidadeApelido => this._mapeamentoConsulta.EstruturaEntidadeApelido;
    private BaseContextoDados Contexto => this._mapeamentoConsulta.Contexto;
    private EstruturaEntidade EstruturaEntidade => this._mapeamentoConsulta.EstruturaEntidade;
    private BaseConexao ConexaoDB => this._mapeamentoConsulta.ConexaoDB;
    private EstruturaConsulta EstruturaConsulta => this._mapeamentoConsulta.EstruturaConsulta;
    private EstruturaCampoApelido EstruturaCampoApelidoChavePrimaria => this._mapeamentoConsulta.EstruturaCampoApelidoChavePrimaria;
    private IReadOnlyCollection<ParametroInfo> ParametrosInfo 
        => this._mapeamentoConsulta.ParametrosInfo;
    private DicionarioEstrutura<EstruturaCampoApelido> TodasEstruturaCampoApelidoMapeado
        => this._mapeamentoConsulta.TodasEstruturaCampoApelidoMapeado;

    protected int Skip { get; private set; }
    protected int Take { get; private set; }

    protected BaseSqlBuilder(BaseMapeamentoEntidade mapeamentoConsulta)
    {
        this._mapeamentoConsulta = mapeamentoConsulta;
        this.Skip = this.RetornarSkip();
        this.Take = this.RetornarTake();
    }

    internal virtual string MontarSql(BaseFiltroMapeamento filtroMapeamento,
                                      string sqlCampos,
                                      bool isIncluirOrdenacaoPaginacao,
                                      bool isRelacaoFilhos)
    {
        var sqlConsulta = this.RetornarSqlConsulta(filtroMapeamento,
                                                  isIncluirOrdenacaoPaginacao,
                                                  isRelacaoFilhos);
        return $"SELECT {sqlCampos} FROM {sqlConsulta}";
    }

    private int RetornarSkip()
    {
        if (this.EstruturaConsulta.Skip > 0)
        {
            return this.EstruturaConsulta.Skip;
        }
        else if (this.EstruturaConsulta.PaginaAtual > 0)
        {
            return (this.EstruturaConsulta.PaginaAtual - 1) * this.EstruturaConsulta.Take;
        }
        else
        {
            return 0;
        }
    }

    private int RetornarTake()
    {
        if (this.EstruturaEntidade == null)
        {
            throw new Exception("EstruturaEntidade não foi definida");
        }
        return this.EstruturaEntidade.RetornarMaximoConsulta(this.EstruturaConsulta.Take);
    }
}

internal class SqlBuilder : BaseSqlBuilder
{
    internal SqlBuilder(BaseMapeamentoEntidade mapeamentoConsulta) : base(mapeamentoConsulta) { }
}
