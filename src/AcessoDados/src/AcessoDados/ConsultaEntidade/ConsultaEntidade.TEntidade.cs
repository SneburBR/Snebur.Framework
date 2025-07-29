namespace Snebur.AcessoDados;

public partial class ConsultaEntidade<TEntidade> : BaseConsultaEntidade, IConsultaEntidade<TEntidade> where TEntidade : IEntidade
{
    #region Construtores

    public ConsultaEntidade(__BaseContextoDados contextoAcessodados) : base(contextoAcessodados, typeof(TEntidade))
    {
    }

    public ConsultaEntidade(__BaseContextoDados contextoAcessodados, Type tipoEntidadeConsulta) : base(contextoAcessodados, tipoEntidadeConsulta)
    {
    }

    public ConsultaEntidade(__BaseContextoDados contextoAcessodados, Type tipoEntidadeConsulta, EstruturaConsulta estruturaConsulta) : base(contextoAcessodados, tipoEntidadeConsulta, estruturaConsulta)
    {
    }
    #endregion

    //public IConsultaEntidade<TConsulta> AsConsulta<TConsulta>() where TConsulta : Entidade
    //{
    //    var tipo = typeof(IConsultaEntidade<TEntidade>);
    //    if (!(ReflexaoUtil.TipoIgualOuHerda(typeof(TConsulta), typeof(TEntidade)) ||
    //         (ReflexaoUtil.TipoIgualOuHerda(typeof(TConsulta), typeof(TEntidade)))))
    //    {
    //        throw new ErroNaoSuportado("AsConsulta");
    //    }
    //    return (IConsultaEntidade<TConsulta>)this;
    //}

    public ConsultaEntidade<TEspecializacao> OfType<TEspecializacao>() where TEspecializacao : TEntidade
    {
        var tipoEntidadeConsulta = typeof(TEspecializacao);
        this.EstruturaConsulta.NomeTipoEntidade = tipoEntidadeConsulta.Name;
        this.EstruturaConsulta.TipoEntidadeAssemblyQualifiedName = tipoEntidadeConsulta.RetornarAssemblyQualifiedName();
        //this.EstruturaConsulta.TipoEntidadeAssemblyQualifiedName = tipoEntidadeConsulta.AssemblyQualifiedName;
        return new ConsultaEntidade<TEspecializacao>(this.ContextoDados, tipoEntidadeConsulta, this.EstruturaConsulta);
    }

    public ConsultaEntidade<TEntidade> Take(int take)
    {
        this.EstruturaConsulta.Take = take;
        return this;
    }

    public ConsultaEntidade<TEntidade> Skip(int skip)
    {
        this.EstruturaConsulta.Skip = skip;
        return this;
    }

    #region Executar no servico

    public List<TEntidade> ToList()
    {
        var resultadoConsulta = this.RetornarResultadoConsulta();
        var lista = new List<TEntidade>();
        lista.AddRange(resultadoConsulta.Entidades.Cast<TEntidade>());
        return lista;
    }

    public TEntidade Single()
    {
        this.EstruturaConsulta.Take = 2;
        return (TEntidade)this.RetornarResultadoConsulta().Entidades.Single();
    }

    public TEntidade? SingleOrDefault()
    {
        this.EstruturaConsulta.Take = 2;
        var entidades = this.RetornarResultadoConsulta().Entidades;
        return (TEntidade?)entidades.SingleOrDefault();
    }

    //public bool Exists()
    //{
    //    this.AbrirPropriedade(x => x.);
    //    return this.SingleOrDefault() != null;
    //}

    public TEntidade First()
    {
        this.EstruturaConsulta.Take = 1;
        return (TEntidade)this.RetornarResultadoConsulta().Entidades.First();
    }

    public TEntidade? FirstOrDefault()
    {
        this.EstruturaConsulta.Take = 1;
        return (TEntidade?)this.RetornarResultadoConsulta().Entidades.FirstOrDefault();
    }
    #endregion

    #region Métodos Privados

    private ResultadoConsulta RetornarResultadoConsulta()
    {
        return this.ContextoDados.RetornarResultadoConsulta(this.EstruturaConsulta);
    }
    #endregion

    public EstruturaConsulta RetornarEstruturaConsulta()
    {
        return this.EstruturaConsulta;
    }

    public ConsultaEntidade<TEntidade> IncluirDeletados()
    {
        this.EstruturaConsulta.IsIncluirDeletados = true;
        return this;
    }
     

    //public TEntidade RetornarPorId(long id)
    //{
    //    return this.Where(x => x.Id == id).Single();
    //}
}