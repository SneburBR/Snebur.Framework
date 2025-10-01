using Newtonsoft.Json;
using Snebur.AcessoDados.Seguranca;

namespace Snebur.AcessoDados;

public class EstruturaConsulta : BaseAcessoDados, IEstruturaConsultaSeguranca
{
    #region Campos Privados
    private string? _nomeTipoEntidade;
    private string? _tipoEntidadeAssemblyQualifiedName;
    private bool _isIncluirDeletados;
    private bool _isIncluirInativos;
    private bool _isDesativarOrdenacao;
    private int _take;
    private int _skip;
    private int _paginaAtual;
    private string? _caminhoPropriedadeFuncao;
    private EnumTipoFuncao _tipoFuncaoEnum;
    private bool _contarRegistros;
    #endregion

    [JsonIgnore, IgnorarPropriedadeTSReflexao]
    internal Type? TipoEntidadeConsulta { get; set; }
    public string? NomeTipoEntidade { get => this.GetPropertyValue(this._nomeTipoEntidade); set => this.SetProperty(this._nomeTipoEntidade, this._nomeTipoEntidade = value); }
    public string? TipoEntidadeAssemblyQualifiedName { get => this.GetPropertyValue(this._tipoEntidadeAssemblyQualifiedName); set => this.SetProperty(this._tipoEntidadeAssemblyQualifiedName, this._tipoEntidadeAssemblyQualifiedName = value); }
    public bool IsIncluirDeletados { get => this.GetPropertyValue(this._isIncluirDeletados); set => this.SetProperty(this._isIncluirDeletados, this._isIncluirDeletados = value); }
    public bool IsIncluirInativos { get => this.GetPropertyValue(this._isIncluirInativos); set => this.SetProperty(this._isIncluirInativos, this._isIncluirInativos = value); }
    public bool IsDesativarOrdenacao { get => this.GetPropertyValue(this._isDesativarOrdenacao); set => this.SetProperty(this._isDesativarOrdenacao, this._isDesativarOrdenacao = value); }
    public int Take { get => this.GetPropertyValue(this._take); set => this.SetProperty(this._take, this._take = value); }
    public int Skip { get => this.GetPropertyValue(this._skip); set => this.SetProperty(this._skip, this._skip = value); }
    public int PaginaAtual { get => this.GetPropertyValue(this._paginaAtual); set => this.SetProperty(this._paginaAtual, this._paginaAtual = value); }
    public string? CaminhoPropriedadeFuncao { get => this.GetPropertyValue(this._caminhoPropriedadeFuncao); set => this.SetProperty(this._caminhoPropriedadeFuncao, this._caminhoPropriedadeFuncao = value); }
    public EnumTipoFuncao TipoFuncaoEnum { get => this.GetPropertyValue(this._tipoFuncaoEnum); set => this.SetProperty(this._tipoFuncaoEnum, this._tipoFuncaoEnum = value); }

    [CriarInstanciaTS]
    public FiltroGrupoE FiltroGrupoE { get; set; } = new FiltroGrupoE();

    [CriarInstanciaTS]
    public FiltroGrupoOU FiltroGrupoOU { get; set; } = new FiltroGrupoOU();
    public Dictionary<string, Ordenacao> Ordenacoes { get; set; } = new Dictionary<string, Ordenacao>();
    public Dictionary<string, RelacaoAbertaEntidade> RelacoesAbertaFiltro { get; set; } = new Dictionary<string, RelacaoAbertaEntidade>();
    public Dictionary<string, RelacaoAbertaEntidade> RelacoesAberta { get; set; } = new Dictionary<string, RelacaoAbertaEntidade>();
    public Dictionary<string, RelacaoAbertaColecao> ColecoesAberta { get; set; } = new Dictionary<string, RelacaoAbertaColecao>();
    public List<string> PropriedadesAbertas { get; set; } = new List<string>();

    public EstruturaConsulta()
    {
    }

    #region IEstruturaConsultaSeguranca  
    private List<string>? _propriedadesAutorizadas;
    [NaoMapear]
    [IgnorarGlobalizacao]
    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    List<string>? IEstruturaConsultaSeguranca.PropriedadesAutorizadas { get => this._propriedadesAutorizadas; }

    List<string>? IEstruturaConsultaSeguranca.PropriedadesAbertas { get => this.PropriedadesAbertas; }
    public bool ContarRegistros { get => this.GetPropertyValue(this._contarRegistros); set => this.SetProperty(this._contarRegistros, this._contarRegistros = value); }

    void IEstruturaConsultaSeguranca.AtribuirPropriedadeAutorizadas(List<string>? propriedadesAutorizadas)
    {
        this._propriedadesAutorizadas = propriedadesAutorizadas;
    }
    #endregion
}