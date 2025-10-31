using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Servidor.Salvar;

internal class EntidadeAlterada
{
    #region Propriedades

    internal Entidade Entidade { get; }
    internal bool IsImplementaIDeletado { get; }
    internal EnumTipoAlteracao TipoAlteracao { get; }
    internal EstruturaEntidade EstruturaEntidade { get; }
    internal Dictionary<string, RelacaoChaveEstrageniraDependente> EntidadesRelacaoChaveEstrangeiraDepedente { get; } = new();
    internal List<CampoComputado> CamposComputado { get; set; }
    internal List<PropertyInfo> PropriedadesAtualizadas { get; set; }
    internal BaseContextoDados Contexto { get; }
    public List<Comando>? Comandos { get; internal set; }

    internal string IdentificadorEntidade => this.Entidade.__IdentificadorEntidade;
    #endregion

    private long _idRollback;

    internal EntidadeAlterada(
        BaseContextoDados contexto,
        Entidade entidade,
        EstruturaEntidade estruturaEntidade,
        EnumOpcaoSalvar opcaoSalvar)
    {
        this._idRollback = entidade.Id;
        this.CamposComputado = new List<CampoComputado>();
        this.PropriedadesAtualizadas = new List<PropertyInfo>();
        this.Contexto = contexto;
        this.Entidade = entidade;
        this.EstruturaEntidade = estruturaEntidade;
        this.IsImplementaIDeletado = estruturaEntidade.IsImplementaInterfaceIDeletado && !estruturaEntidade.IsDeletarRegistro;
        this.TipoAlteracao = this.AnalisarOpcaoSalvar(opcaoSalvar);

        if (entidade.__IsIdentity != estruturaEntidade.IsIdentity)
        {
            throw new ErroOperacaoInvalida("A entidade possui configuração inválida de chave primária Identity");
        }
    }

    private EnumTipoAlteracao AnalisarOpcaoSalvar(EnumOpcaoSalvar opcaoSalvar)
    {
        var entidade = this.Entidade;
        var estruturaEntidade = this.EstruturaEntidade;
        if (opcaoSalvar == EnumOpcaoSalvar.Deletar ||
            opcaoSalvar == EnumOpcaoSalvar.DeletarRegistro)
        {

            if (entidade.Id == 0)
            {
                throw new ErroOperacaoInvalida("Não é possível deletar uma entidade com id 0");
            }

            if (this.IsImplementaIDeletado &&
                 opcaoSalvar == EnumOpcaoSalvar.Deletar)
            {
                if (estruturaEntidade.IsImplementaInterfaceIAtivo)
                {
                    var entidadeAtivo = (IAtivo)entidade;
                    entidadeAtivo.IsAtivo = false;
                }

                var entidadeDeletada = (IDeletado)entidade;
                var contexto = this.Contexto;

                if (contexto.SessaoUsuarioLogado is null)
                {
                    throw new ErroOperacaoInvalida("O contexto não possui sessão de usuário logado para setar o valor de deletado");
                }

                entidadeDeletada.IsDeletado = true;
                entidadeDeletada.SessaoUsuarioDeletado_Id = contexto.SessaoUsuarioLogado.Id;
                entidadeDeletada.SessaoUsuarioDeletado = contexto.SessaoUsuarioLogado;
                entidadeDeletada.DataHoraDeletado = contexto.SqlSuporte.IsDataHoraUtc ? DateTime.UtcNow : DateTime.Now;
                this.AtualizarValorEntidadeDeletada();

                return EnumTipoAlteracao.Update;
            }
            return EnumTipoAlteracao.Delete;
        }

        if (!estruturaEntidade.IsIdentity)
        {
            if (entidade.Id == 0)
            {
                throw new ErroOperacaoInvalida($"A entidade '{entidade.GetType().Name}' não possui chave primária de auto incremento, O Id é requerido");
            }
            return EnumTipoAlteracao.InsertOrUpdate;
        }

        return (entidade.Id == 0) ? EnumTipoAlteracao.Insert : EnumTipoAlteracao.Update;
    }

    private void AtualizarValorEntidadeDeletada()
    {
        foreach (var propriedade in this.EstruturaEntidade.TipoEntidade.GetProperties())
        {
            var atributo = CustomAttributeExtensions.GetCustomAttribute<ValorDeletadoConcatenarGuidAttribute>(propriedade);
            if (atributo != null)
            {
                if (propriedade.PropertyType != typeof(string))
                {
                    throw new Erro($"O atributo {nameof(ValorDeletadoConcatenarGuidAttribute)} é suportado apenas para propriedades do tipo string ");
                }
                var maximoCaracteres = propriedade.GetCustomAttribute<ValidacaoTextoTamanhoAttribute>()?.MaximumLength ?? Int32.MaxValue;
                maximoCaracteres = Math.Min(maximoCaracteres, 255);
                var minGuid = Guid.NewGuid().ToString().Split('-').First();
                var valorDeletado = String.Concat(minGuid, " - deletado ", propriedade.GetValue(this.Entidade)?.ToString()).RetornarPrimeirosCaracteres(maximoCaracteres);
                propriedade.SetValue(this.Entidade, valorDeletado);
            }

            var atributoDeletado = CustomAttributeExtensions.GetCustomAttribute<ValorDeletadoAttribute>(propriedade);
            if (atributoDeletado != null)
            {
                propriedade.SetValue(this.Entidade, atributoDeletado.Valor);
            }
        }
    }

    internal void AtualizarEntidadesDepedentes()
    {
        this.EntidadesRelacaoChaveEstrangeiraDepedente.AddRange(this.RetornarEntidadesRelacaoChaveEstrangeiraDepedente());
    }

    public List<Comando> RetornarCommandos()
    {
        switch (this.TipoAlteracao)
        {
            case EnumTipoAlteracao.Insert:
            case EnumTipoAlteracao.Update:
            case EnumTipoAlteracao.InsertOrUpdate:

                return this.RetornarComandosSalvar();

            case (EnumTipoAlteracao.Delete):

                return this.RetornarComandosDeletar();

            default:

                throw new ErroNaoSuportado("O tipo da alteração não é suportado");
        }
    }

    #region Salvar

    private List<Comando> RetornarComandosSalvar()
    {
        var comandos = new List<Comando>();
        var estruturasEntidade = this.RetornarEstruturasEntidadeSalvar();
        foreach (var estruturaEntidade in estruturasEntidade)
        {
            switch (this.TipoAlteracao)
            {
                case EnumTipoAlteracao.Insert:

                    comandos.Add(new ComandoInsert(this, estruturaEntidade));
                    break;

                case EnumTipoAlteracao.InsertOrUpdate:

                    var comandoUnsertUpdate = new ComandoInsertOrUpdate(this, estruturaEntidade);

                    comandos.Add(comandoUnsertUpdate);

                    this.AnalisarCamposComputados(estruturaEntidade, comandoUnsertUpdate as IComandoUpdate);

                    break;

                case EnumTipoAlteracao.Update:

                    var comandoUpdate = new ComandoUpdate(this, estruturaEntidade);
                    if (comandoUpdate.ExisteAtualizacao)
                    {
                        comandos.Add(comandoUpdate);

                        this.AnalisarCamposComputados(estruturaEntidade, comandoUpdate);
                    }
                    break;

                case EnumTipoAlteracao.Delete:

                    throw new ErroOperacaoInvalida("Operação invalida");

                default:

                    throw new Erro("O tipo de alteração não é suportado");
            }
            if (this.TipoAlteracao == EnumTipoAlteracao.Insert)
            {
                foreach (var estruturaCampo in estruturaEntidade.EstruturasCamposComputadoBanco)
                {
                    comandos.Add(new ComandoCampoComputado(this, estruturaEntidade, estruturaCampo));
                }
            }
        }
        return comandos;
    }

    private void AnalisarCamposComputados(
        EstruturaEntidade estruturaEntidade,
        IComandoUpdate comandoUpdate)
    {
        foreach (var estruturaCampo in estruturaEntidade.EstruturasCamposComputadoServico)
        {
            if (comandoUpdate.PropriedadesAlterada.ContainsKey(estruturaCampo.Propriedade.Name))
            {
                var valor = estruturaCampo.Propriedade.GetValue(this.Entidade);
                this.CamposComputado.Add(new CampoComputado(estruturaCampo, valor));
            }
        }
    }

    private List<EstruturaEntidade> RetornarEstruturasEntidadeSalvar()
    {
        var estruturas = this.RetornarEstruturasEntidade();
        estruturas.Reverse();
        return estruturas;
    }
    #endregion

    #region Deletar

    private List<Comando> RetornarComandosDeletar()
    {
        var comandos = new List<Comando>();
        var estruturasEntidade = this.RetornarEstruturasEntidade();
        foreach (var estruturaEntidade in estruturasEntidade)
        {
            var comando = new ComandoDelete(this, estruturaEntidade);
            if (!String.IsNullOrWhiteSpace(comando.SqlCommando))
            {
                comandos.Add(comando);
            }
        }
        return comandos;
    }
    #endregion

    private List<EstruturaEntidade> RetornarEstruturasEntidade()
    {
        var estruturasEntidade = new List<EstruturaEntidade>();
        estruturasEntidade.Add(this.EstruturaEntidade);

        var estruturaAtual = this.EstruturaEntidade;
        while (estruturaAtual.EstruturaEntidadeBase != null)
        {
            estruturasEntidade.Add(estruturaAtual.EstruturaEntidadeBase);
            estruturaAtual = estruturaAtual.EstruturaEntidadeBase;
        }
        return estruturasEntidade;
    }

    #region Métodos privados

    private Dictionary<string, RelacaoChaveEstrageniraDependente> RetornarEntidadesRelacaoChaveEstrangeiraDepedente()
    {
        if (this.TipoAlteracao == EnumTipoAlteracao.Delete)
        {
            return new Dictionary<string, RelacaoChaveEstrageniraDependente>();
        }

        var relacoesDepedente = new Dictionary<string, RelacaoChaveEstrageniraDependente>();
        var estruturasRelacaoChaveEstrangeira = this.EstruturaEntidade.TodasRelacoesChaveEstrangeira;

        var estruturas = new List<EstruturaRelacao>();
        foreach (var estruturaRelacaoChaveEstrangeira in estruturasRelacaoChaveEstrangeira)
        {
            var entidadeRelacao = estruturaRelacaoChaveEstrangeira.Propriedade.GetValue(this.Entidade) as Entidade;
            if (entidadeRelacao is not null &&
                !this.IsIgnorarRelacaoDepedenta(entidadeRelacao))
            {
                if (!relacoesDepedente.ContainsKey(entidadeRelacao.__IdentificadorEntidade))
                {
                    relacoesDepedente.Add(entidadeRelacao.__IdentificadorEntidade, new RelacaoChaveEstrageniraDependente(entidadeRelacao, estruturaRelacaoChaveEstrangeira));
                }
            }
        }
        return relacoesDepedente;
    }

    private bool IsIgnorarRelacaoDepedenta(Entidade entidadeRelacao)
    {
        var estruturaEntidadeRelacao = this.Contexto.EstruturaBancoDados.EstruturasEntidade[entidadeRelacao.GetType().Name];
        return estruturaEntidadeRelacao.IsImplementaInterfaceISessaoUsuario;
    }

    internal void AtualizarPropriedadeCampoComputado(CampoComputado campoComputado)
    {
        var propriedade = campoComputado.EstruturaCampo.Propriedade;
        propriedade.SetValue(this.Entidade, campoComputado.Valor);
    }

    public override string ToString()
    {
        return String.Format("{0} - {1}", base.ToString(), this.Entidade.ToString());
    }

    internal void SetId(long id)
    {
        this.Entidade.Id = id;
    }
    internal void Rollback()
    {
        this.Entidade.Id = this._idRollback;
    }

    internal Dictionary<string, PropriedadeAlterada> RetornarPropriedadesAlteradas()
    {
        return this.Entidade.RetornarPropriedadesAlteradas() ??
                    new Dictionary<string, PropriedadeAlterada>();
    }
    #endregion
}