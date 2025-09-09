using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento;

internal abstract partial class BaseEstruturaEntidadeApelido : BaseEstruturaApelido, IDisposable
{

    internal EstruturaCampoApelido? EstruturaCampoApelidoChavePrimaria { get; set; }

    internal EstruturaEntidade EstruturaEntidade { get; }

    internal List<EstruturaEntidadeApelidoBase> EstruturasEntidadeMapeadaBase { get; }

    internal List<EstruturaCampoApelido> EstruturasCampoApelido { get; }

    internal List<EstruturaEntidadeApelido> EstruturasEntidadeRelacaoMapeadaInterna { get; }

    internal BaseEstruturaEntidadeApelido(
        BaseMapeamentoConsulta mapeamentoConsulta,
        string apelidoEntidadeMapeada,
        EstruturaEntidade estruturaEntidade) :
                                          base(mapeamentoConsulta,
                                               apelidoEntidadeMapeada, String.Format("[{0}].[{1}]", estruturaEntidade.Schema, estruturaEntidade.NomeTabela))

    {
        this.EstruturaEntidade = estruturaEntidade;
        this.EstruturasEntidadeMapeadaBase = new List<EstruturaEntidadeApelidoBase>();
        this.EstruturasEntidadeRelacaoMapeadaInterna = new List<EstruturaEntidadeApelido>();
        this.EstruturasCampoApelido = new List<EstruturaCampoApelido>();
    }

    internal string CaminhoCampoNomeTipoEntidade()
    {
        var estrutura = this.RetornarEstruturaEntidadeApelidoCampoNomeTipoEntidade();
        return string.Format(" {0}.[__NomeTipoEntidade] As [__NomeTipoEntidade]  ", estrutura.Apelido);
    }

    private BaseEstruturaEntidadeApelido RetornarEstruturaEntidadeApelidoCampoNomeTipoEntidade()
    {
        if (this.EstruturasEntidadeMapeadaBase.Count() > 0)
        {
            return this.EstruturasEntidadeMapeadaBase.Where(x => x.EstruturaEntidade.EstruturaEntidadeBase == null).Single();
        }
        return this;
    }
    #region IDisposable

    public void Dispose()
    {
        this.EstruturasEntidadeMapeadaBase?.Clear();
        this.EstruturasEntidadeRelacaoMapeadaInterna?.Clear();
        this.EstruturasCampoApelido?.Clear();
    }
    #endregion
}