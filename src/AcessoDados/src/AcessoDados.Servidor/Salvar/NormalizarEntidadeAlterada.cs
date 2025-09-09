using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Servidor.Salvar;

internal partial class NormalizarEntidadeAlterada : IDisposable
{
    internal List<EntidadeAlterada> EntidadesAlterada { get; }

    internal BaseContextoDados Contexto { get; }

    internal Dictionary<string, EntidadeAlterada> EntidadesAlteradasNormalizadas { get; } = new();

    private NormalizarEntidadeAlterada(
        BaseContextoDados contexto,
        List<EntidadeAlterada> entidadesAlteradas)
    {
        this.EntidadesAlteradasNormalizadas = new Dictionary<string, EntidadeAlterada>();
        this.Contexto = contexto;
        this.EntidadesAlterada = entidadesAlteradas;
    }

    internal List<EntidadeAlterada> RetornarEntidadesAlteradaNormalizada()
    {
        this.Normalizar();

        foreach (var entidadeAlterada in this.EntidadesAlteradasNormalizadas.Values)
        {
            entidadeAlterada.AtualizarEntidadesDepedentes();
        }
        return this.EntidadesAlteradasNormalizadas.Values.ToList();
    }

    internal void Normalizar()
    {
        foreach (var entidade in this.EntidadesAlterada)
        {
            this.Normalizar(entidade);
        }
    }

    private void Normalizar(EntidadeAlterada entidadeAlterada)
    {
        if (!this.EntidadesAlteradasNormalizadas.ContainsKey(entidadeAlterada.IdentificadorEntidade))
        {
            this.EntidadesAlteradasNormalizadas.Add(entidadeAlterada.IdentificadorEntidade, entidadeAlterada);

            var estruturaEntidade = this.Contexto.EstruturaBancoDados.EstruturasEntidade[entidadeAlterada.Entidade.GetType().Name];

            this.NormalizarRelacoesFilho(entidadeAlterada.Entidade, estruturaEntidade);
            this.NormalizarRelacoesNn(entidadeAlterada.Entidade, estruturaEntidade);
        }
    }

    private void NormalizarRelacoesFilho(Entidade entidade, EstruturaEntidade estruturaEntidade)
    {
        var estruturasRelacoesFilhos = estruturaEntidade.TodasRelacoesFilhos;
        foreach (var estruturaRelacaoFilhos in estruturasRelacoesFilhos)
        {
            var entidadesFilho = estruturaRelacaoFilhos.Propriedade.GetValue(entidade) as IListaEntidades
                ?? throw new Exception("A propriedade de relação filho deve implementar a interface IListaEntidades");

            if (entidadesFilho.EntidadesRemovida.Count > 0)
            {
                //TODO: Remover relacoes filhos desabilitar
                //throw new ErroNaoImplementado();
            }
        }
    }

    private void NormalizarRelacoesNn(Entidade entidade, EstruturaEntidade estruturaEntidade)
    {
        var estruturasRelacoesNn = estruturaEntidade.TodasRelacoesNn;
        foreach (var estruturaRelacaoNn in estruturasRelacoesNn)
        {
            var entidadesFilhosNn = estruturaRelacaoNn.Propriedade.GetValue(entidade) as IListaEntidades
                ?? throw new Exception("A propriedade de relação n-n deve implementar a interface IListaEntidades");

            foreach (Entidade entidadeFilhoNn in entidadesFilhosNn)
            {
                var consultaEntidadeRelacaoNn = this.Contexto.RetornarConsulta<Entidade>(estruturaRelacaoNn.EstruturaEntidadeRelacaoNn.TipoEntidade);

                consultaEntidadeRelacaoNn.AdicionarFiltroPropriedade(estruturaRelacaoNn.EstruturaCampoChaveEstrangeiraPai.Propriedade, EnumOperadorFiltro.Igual, entidade.Id);
                consultaEntidadeRelacaoNn.AdicionarFiltroPropriedade(estruturaRelacaoNn.EstruturaCampoChaveEstrangeiraFilho.Propriedade, EnumOperadorFiltro.Igual, entidadeFilhoNn.Id);

                var entidadeRelacaoNn = consultaEntidadeRelacaoNn.SingleOrDefault();
                if (entidadeRelacaoNn is not null)
                {
                    entidadeRelacaoNn = Activator.CreateInstance(estruturaRelacaoNn.EstruturaEntidadeRelacaoNn.TipoEntidade) as Entidade ??
                        throw new ErroNaoDefinido("Não foi possível criar a instância da entidade relação n-n");

                    //Pai
                    if (!entidade.__IsNewEntity)
                    {
                        estruturaRelacaoNn.EstruturaCampoChaveEstrangeiraPai.Propriedade.SetValue(entidadeRelacaoNn, entidade.Id);
                    }
                    else
                    {
                        Guard.NotNull(estruturaRelacaoNn.EstruturaRelacaoPaiEntidadePai);
                        estruturaRelacaoNn.EstruturaRelacaoPaiEntidadePai.Propriedade.SetValue(entidadeRelacaoNn, entidade);
                    }
                    //Filho
                    if (!entidadeFilhoNn.__IsNewEntity)
                    {
                        estruturaRelacaoNn.EstruturaCampoChaveEstrangeiraFilho.Propriedade.SetValue(entidadeRelacaoNn, entidadeFilhoNn.Id);
                    }
                    else
                    {
                        Guard.NotNull(estruturaRelacaoNn.EstruturaRelacaoPaiEntidadeFilho);
                        estruturaRelacaoNn.EstruturaRelacaoPaiEntidadeFilho.Propriedade.SetValue(entidadeRelacaoNn, entidadeFilhoNn);
                    }

                    var novaEntidadeAlterada = new EntidadeAlterada(this.Contexto, entidadeRelacaoNn, estruturaRelacaoNn.EstruturaEntidadeRelacaoNn, EnumOpcaoSalvar.Salvar);
                    this.EntidadesAlteradasNormalizadas.Add(novaEntidadeAlterada.IdentificadorEntidade, novaEntidadeAlterada);
                }
            }
            if (entidadesFilhosNn.EntidadesRemovida.Count > 0)
            {
                foreach (Entidade entidadeFilhoNnRemovida in entidadesFilhosNn.EntidadesRemovida)
                {
                    var consultaEntidadeRelacaoNn = this.Contexto.RetornarConsulta<Entidade>(estruturaRelacaoNn.EstruturaEntidadeRelacaoNn.TipoEntidade);

                    consultaEntidadeRelacaoNn.AdicionarFiltroPropriedade(estruturaRelacaoNn.EstruturaCampoChaveEstrangeiraPai.Propriedade, EnumOperadorFiltro.Igual, entidade.Id);
                    consultaEntidadeRelacaoNn.AdicionarFiltroPropriedade(estruturaRelacaoNn.EstruturaCampoChaveEstrangeiraFilho.Propriedade, EnumOperadorFiltro.Igual, entidadeFilhoNnRemovida.Id);
                    var entidadeRelacaoNn = consultaEntidadeRelacaoNn.SingleOrDefault();
                    if (entidadeRelacaoNn is not null)
                    {
                        var entidadeDeletadas = new EntidadeAlterada(this.Contexto, entidadeRelacaoNn, estruturaRelacaoNn.EstruturaEntidadeRelacaoNn, EnumOpcaoSalvar.Deletar);
                        this.EntidadesAlteradasNormalizadas.Add(entidadeDeletadas.IdentificadorEntidade, entidadeDeletadas);
                    }
                }
            }
        }
    }
    #region IDisposable

    public void Dispose()
    {
    }
    #endregion
}