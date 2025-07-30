namespace Snebur.AcessoDados;

public abstract class BaseRelacaoAberta : BaseAcessoDados
{
    #region Campos Privados

    private string? _caminhoPropriedade;
    private string? _nomeTipoEntidade;
    private string? _tipoEntidadeAssemblyQualifiedName;
    private string? _nomeTipoDeclarado;
    private string? _tipoDeclaradoAssemblyQualifiedName;

    #endregion

    public string? CaminhoPropriedade { get => this.GetPropertyValue(this._caminhoPropriedade); set => this.SetProperty(this._caminhoPropriedade, this._caminhoPropriedade = value); }

    public string? NomeTipoEntidade { get => this.GetPropertyValue(this._nomeTipoEntidade); set => this.SetProperty(this._nomeTipoEntidade, this._nomeTipoEntidade = value); }

    public string? NomeTipoDeclarado { get => this.GetPropertyValue(this._nomeTipoDeclarado); set => this.SetProperty(this._nomeTipoDeclarado, this._nomeTipoDeclarado = value); }
    public string? TipoEntidadeAssemblyQualifiedName { get => this.GetPropertyValue(this._tipoEntidadeAssemblyQualifiedName); set => this.SetProperty(this._tipoEntidadeAssemblyQualifiedName, this._tipoEntidadeAssemblyQualifiedName = value); }
    public string? TipoDeclaradoAssemblyQualifiedName { get => this.GetPropertyValue(this._tipoDeclaradoAssemblyQualifiedName); set => this.SetProperty(this._tipoDeclaradoAssemblyQualifiedName, this._tipoDeclaradoAssemblyQualifiedName = value); }

    public HashSet<string> PropriedadesAbertas { get; set; } = new();

    [IgnorarPropriedade]
    [IgnorarPropriedadeTSReflexao]
    public PropertyInfo? Propriedade { get; set; }

    public BaseRelacaoAberta()
    {
    }

    [IgnorarConstrutorTS]
    public BaseRelacaoAberta(PropertyInfo propriedade)
    {
        var tipoEntidade = this.RetornarTipoEntidade(propriedade);
        if (!tipoEntidade.IsSubclassOf(typeof(Entidade)))
        {
            throw new ErroOperacaoInvalida("O tipo da entidade não é suportado");
        }
        this.CaminhoPropriedade = propriedade.Name;
        this.NomeTipoEntidade = tipoEntidade.Name;
        //this.TipoEntidadeAssemblyQualifiedName = tipoEntidade.AssemblyQualifiedName;
        this.TipoEntidadeAssemblyQualifiedName = tipoEntidade.RetornarAssemblyQualifiedName();

        this.NomeTipoDeclarado = propriedade.DeclaringType?.Name;
        this.TipoDeclaradoAssemblyQualifiedName = propriedade.DeclaringType?.RetornarAssemblyQualifiedName();
        //this.TipoDeclaradoAssemblyQualifiedName = propriedade.DeclaringType.AssemblyQualifiedName;
    }

    private Type RetornarTipoEntidade(PropertyInfo propriedade)
    {
        if (propriedade.PropertyType.IsGenericType && ReflexaoUtil.IsTipoRetornaColecao(propriedade.PropertyType))
        {
            return propriedade.PropertyType.GetGenericArguments().Single();
        }
        return propriedade.PropertyType;
    }
}