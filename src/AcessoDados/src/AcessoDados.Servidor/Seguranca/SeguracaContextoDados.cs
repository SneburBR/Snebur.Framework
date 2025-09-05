using Snebur.AcessoDados.Estrutura;
using Snebur.Linq;

namespace Snebur.AcessoDados.Seguranca
{
    internal partial class SeguracaContextoDados
    {
        internal Dictionary<Guid, EstruturaIdentificacao> EstruturasIdentificacao { get; } = new Dictionary<Guid, EstruturaIdentificacao>();

        internal IContextoDadosSeguranca ContextoDados { get; }

        internal EstruturaBancoDados EstruturaBancoDados { get; }

        private SeguracaContextoDados(IContextoDadosSeguranca contexto)
        {
            this.ContextoDados = contexto;

            var contextoDados = contexto as BaseContextoDados ??
                throw new Exception($" O contexto deve ser do tipo {typeof(BaseContextoDados).FullName}");

            this.EstruturaBancoDados = contextoDados.EstruturaBancoDados;

            var relacoesAberta = ReflexaoUtil.RetornarNomesPropriedade<IPermissaoEntidade>(
                x => x.Adicionar,
                x => x.Atualizar,
                x => x.Leitura,
                x => x.Deletar,
                x => x.Identificacao);

            var colecoesAberta = ReflexaoUtil.RetornarNomesPropriedade<IPermissaoEntidade>(x => x.PermissoesCampo,
                                                                                           x => x.RestricoesEntidade);

            var expressoesReleacoesAbertaPermissaoCampo = ExpressaoUtil.RetornarExpressoes<IPermissaoEntidade>(
                                                                    x => x.PermissoesCampo!.Incluir()!.Leitura,
                                                                    x => x.PermissoesCampo!.Incluir()!.Atualizar);

            var consultaPermissoesEntidade = contexto.RetornarConsulta<IPermissaoEntidade>(contexto.TiposSeguranca.TipoPermissaoEntidade)
                .AbrirRelacoes(relacoesAberta.ToArray())
                .AbrirColecoes(colecoesAberta.ToArray())
                .AbrirRelacoes(expressoesReleacoesAbertaPermissaoCampo);

            var resultadoConsulta = contextoDados.RetornarResultadoConsultaInterno(consultaPermissoesEntidade.EstruturaConsulta);

            var permissoesEntidade = resultadoConsulta.Entidades.Cast<IPermissaoEntidade>().ToList();

            foreach (var grupo in permissoesEntidade.GroupBy(x => x.Identificacao))
            {
                var identificacao = grupo.Key;
                Guard.NotNull(identificacao);
                this.EstruturasIdentificacao.Add(identificacao.Identificador, new EstruturaIdentificacao(identificacao, grupo.ToList()));
            }
        }

        internal EnumPermissao PermissaoLeitura(IUsuario usuario, IUsuario usuarioAvalista, EstruturaConsulta estruturaConsulta)
        {
            var nomesTipoEntidade = this.RetornarNomesTipoEntidade(estruturaConsulta);
            var autorizacoes = this.RetornarAutorizacoes(usuario,
                EnumOperacao.Leitura,
                nomesTipoEntidade,
                entidades: null,
                estruturaConsulta);

            var permissao = this.RetornarPermissao(usuario, usuarioAvalista, autorizacoes);
            if ((permissao == EnumPermissao.Autorizado) ||
                (permissao == EnumPermissao.AvalistaRequerido))
            {
                this.AplicarRestricoesFiltro(autorizacoes, estruturaConsulta);
            }
            if (permissao == EnumPermissao.AvalistaRequerido)
            {
                if (usuarioAvalista != null)
                {
                    var permissaoAvalista = this.PermissaoLeitura(usuarioAvalista, null, estruturaConsulta);
                    if (permissaoAvalista == EnumPermissao.Autorizado)
                    {
                        var autorizacoesAvalistaRequerido = autorizacoes.Where(x => x.Permissao == EnumPermissao.AvalistaRequerido).ToList();
                        LogSegurancaUtil.LogAvalistaRequerido(usuario, usuarioAvalista, autorizacoesAvalistaRequerido);
                        return EnumPermissao.Autorizado;
                    }
                    return EnumPermissao.AvalistaRequerido;
                }
            }
            return permissao;
        }

        internal EnumPermissao PermissaoSalvar(
            IUsuario usuario,
            IUsuario? usuarioAvalista,
            IEnumerable<IEntidade> entidades)
        {
            var dicionarioEntidadesAdicionar = this.RetornarTodosEntidades(entidades, EnumOperacao.Adicionar);
            var dicionarioEntidadesSalvar = this.RetornarTodosEntidades(entidades, EnumOperacao.Atualizar);

            var nomesTipoEntidadesAdicionar = dicionarioEntidadesAdicionar.Keys.ToHashSet();
            var nomesTipoEntidadesSalvar = dicionarioEntidadesSalvar.Keys.ToHashSet();

            var autorizacoesAdicionar = this.RetornarAutorizacoes(usuario,
                EnumOperacao.Adicionar,
                nomesTipoEntidadesAdicionar,
                dicionarioEntidadesAdicionar,
                estruturaConsutla: null);

            var autorizacoesAtualizar = this.RetornarAutorizacoes(usuario,
                EnumOperacao.Atualizar,
                nomesTipoEntidadesSalvar,
                dicionarioEntidadesSalvar,
                estruturaConsutla: null);

            var autorizacoes = new List<AutorizacaoEntidade>();
            autorizacoes.AddRange(autorizacoesAdicionar);
            autorizacoes.AddRange(autorizacoesAtualizar);

            var permissao = this.RetornarPermissao(usuario, usuarioAvalista, autorizacoes);
            if ((permissao == EnumPermissao.Autorizado) ||
               (permissao == EnumPermissao.AvalistaRequerido))
            {
                this.AplicarRestricoesFiltro(autorizacoes, entidades);
            }

            if (permissao == EnumPermissao.AvalistaRequerido)
            {
                if (usuarioAvalista != null)
                {
                    var permissaoAvalista = this.PermissaoSalvar(usuarioAvalista, null, entidades);
                    if (permissaoAvalista == EnumPermissao.Autorizado)
                    {
                        var autorizacoesAvalistaRequerido = autorizacoes.Where(x => x.Permissao == EnumPermissao.AvalistaRequerido).ToList();
                        LogSegurancaUtil.LogAvalistaRequerido(usuario, usuarioAvalista, autorizacoesAvalistaRequerido);
                        return EnumPermissao.Autorizado;
                    }
                    return EnumPermissao.AvalistaRequerido;
                }
            }
            if (permissao == EnumPermissao.Negado)
            {
                if (this.AutorizacaoEspecialPadrao(usuario, entidades))
                {
                    return EnumPermissao.Autorizado;
                }
            }
            return permissao;
        }

        internal EnumPermissao PermissaoAlterarUsuario(IUsuario usuarioLogado, IUsuario usuario)
        {
            if (usuario != null && !usuario.__IsNewEntity)
            {
                if (usuarioLogado.Identificador == usuario.Identificador)
                {
                    return EnumPermissao.Autorizado;
                }
            }
            return EnumPermissao.Negado;
        }

        internal EnumPermissao PermissaoDeletar(
            IUsuario usuario,
            IUsuario? usuarioAvalista,
            List<Entidade> entidades)
        {
            var dicionarioEntidades = this.RetornarTodosEntidades(entidades, EnumOperacao.Deletar);
            var nomesTipoEntidade = dicionarioEntidades.Keys.ToHashSet();
            var autorizacoes = this.RetornarAutorizacoes(usuario, EnumOperacao.Deletar, nomesTipoEntidade, dicionarioEntidades, null);

            var permissao = this.RetornarPermissao(usuario, usuarioAvalista, autorizacoes);
            if ((permissao == EnumPermissao.Autorizado) ||
                (permissao == EnumPermissao.Autorizado))
            {
                this.AplicarRestricoesFiltro(autorizacoes, entidades);
            }
            return this.RetornarPermissao(usuario, usuarioAvalista, autorizacoes);
        }

        private List<AutorizacaoEntidade> RetornarAutorizacoes(
            IIdentificacao identificacao,
            EnumOperacao operacao,
            HashSet<string> nomesTipoEntidade,
            Dictionary<string, List<Entidade>>? entidades,
            EstruturaConsulta? estruturaConsutla)
        {
            var estruturasIdentificacao = this.RetornarEstruturasIdentificacao(identificacao);
            var autorizacoes = new List<AutorizacaoEntidade>();
            foreach (var nomeTipoEntidade in nomesTipoEntidade)
            {
                var estruturaEntidade = this.EstruturaBancoDados.EstruturasEntidade[nomeTipoEntidade];

                if (this.AutorizacaoEspecialPadrao(operacao, estruturaEntidade))
                {
                    continue;
                }
                var autorizacao = this.RetornarNovaAutorizacao(
                    nomeTipoEntidade,
                    operacao,
                    entidades,
                    estruturaConsutla);

                foreach (var estruturaIdentificacao in estruturasIdentificacao)
                {
                    if (estruturaIdentificacao.PermissoesEntidade.TryGetValue(nomeTipoEntidade,
                        out EstruturaPermissaoEntidade? estruturaRegraEntidade))
                    {
                        var regraOperacao = estruturaRegraEntidade.RetornarRegraOperacao(operacao);
                        var permisao = regraOperacao.RetornarPermisao();

                        if (permisao == EnumPermissao.Autorizado ||
                            permisao == EnumPermissao.AvalistaRequerido)
                        {
                            var avalistaRequerido = (permisao == EnumPermissao.AvalistaRequerido);
                            autorizacao.Autorizar(estruturaRegraEntidade, regraOperacao, avalistaRequerido);
                        }
                    }
                }
                autorizacoes.Add(autorizacao);
            }
            return autorizacoes;
        }

        private AutorizacaoEntidade RetornarNovaAutorizacao(
            string nomeTipoEntidade,
            EnumOperacao operacao,
            Dictionary<string, List<Entidade>>? dicionario,
            EstruturaConsulta? estruturaConsutla)
        {
            switch (operacao)
            {
                case EnumOperacao.Leitura:

                    return new AutorizacaoEntidadeLeitura(nomeTipoEntidade, operacao, estruturaConsutla);

                case EnumOperacao.Adicionar:
                case EnumOperacao.Atualizar:
                case EnumOperacao.Deletar:

                    if (dicionario?.ContainsKey(nomeTipoEntidade) == true)
                    {
                        var entidades = dicionario[nomeTipoEntidade];
                        return new AutorizacaoEntidadeSalvar(nomeTipoEntidade, operacao, entidades);
                    }

                    throw new Exception(
                        $"Para a operação {operacao.ToString()} é necessário informar as entidades. nomeTipoEntidade: {nomeTipoEntidade}");


                default:

                    throw new ErroNaoSuportado($"a operação {operacao.ToString()} não é suportada");
            }
        }

        private bool AutorizacaoEspecialPadrao(EnumOperacao operacao, EstruturaEntidade estruturaEntidade)
        {
            if ((operacao == EnumOperacao.Leitura) && estruturaEntidade.Schema == TabelaSegurancaAttribute.SCHEMA_SEGURANCA)
            {
                return true;
            }
            if (this.ContextoDados.IsAnonimo)
            {
                if (operacao == EnumOperacao.Leitura || operacao == EnumOperacao.Adicionar)
                {
                    if (ReflexaoUtil.IsTipoIgualOuHerda(estruturaEntidade.TipoEntidade, this.ContextoDados.TiposSeguranca.TipoUsuario))
                    {
                        return this.ContextoDados.IsAnonimo;
                    }
                    if (ReflexaoUtil.IsTipoIgualOuHerda(estruturaEntidade.TipoEntidade, this.ContextoDados.TiposSeguranca.TipoSessaoUsuario))
                    {
                        return this.ContextoDados.IsAnonimo;
                    }
                    if (ReflexaoUtil.IsTipoIgualOuHerda(estruturaEntidade.TipoEntidade, this.ContextoDados.TiposSeguranca.TipoIpInformacao))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool AutorizacaoEspecialPadrao(IUsuario usuario, IEnumerable<IEntidade> entidades)
        {
            if (entidades.Count() == 1 &&
                entidades.Single() is IUsuario usuarioTipado &&
                usuarioTipado.GetType().IsSubclassOf(this.ContextoDados.TiposSeguranca.TipoUsuario))
            {
                return this.PermissaoAlterarUsuario(usuario, usuarioTipado) == EnumPermissao.Autorizado;
            }
            return false;
        }

        private EnumPermissao RetornarPermissao(
            IUsuario usuario,
            IUsuario? usuarioAvalista, List<AutorizacaoEntidade> autorizacoes)
        {
            var permissao = this.RetornarResultadoPermissao(autorizacoes);
            if (permissao == EnumPermissao.Autorizado)
            {
                var autorizacoesLogAlteracao = autorizacoes.Where(x => x.IsSalvarLogAlteracao).ToList();
                LogSegurancaUtil.LogAlteracaoEntidade(usuario, usuarioAvalista, autorizacoesLogAlteracao);

                var autorizacoesLogSeguranca = autorizacoes.Where(x => x.IsSalvarLogSeguranca).ToList();
                LogSegurancaUtil.LogSeguranca(usuario, usuarioAvalista, autorizacoesLogSeguranca);
                return EnumPermissao.Autorizado;
            }

            if (permissao == EnumPermissao.Negado)
            {
                var autorizacoesNegada = autorizacoes.Where(x => x.Permissao == EnumPermissao.Negado).ToList();
                LogSegurancaUtil.LogPermissaoNegada(usuario, usuarioAvalista, autorizacoesNegada);
                return EnumPermissao.Negado;
            }
            throw new ErroNaoSuportado($"A permissão não é suportada {permissao.ToString()} ");
        }

        private EnumPermissao RetornarResultadoPermissao(List<AutorizacaoEntidade> autorizacoes)
        {
            foreach (var autorizacao in autorizacoes)
            {
                if (autorizacao.Permissao == EnumPermissao.Negado)
                {
                    return EnumPermissao.Negado;
                }
                if (autorizacao.Permissao == EnumPermissao.AvalistaRequerido)
                {
                    return EnumPermissao.AvalistaRequerido;
                }
            }
            return EnumPermissao.Autorizado;
        }

        private Dictionary<string, List<Entidade>> RetornarTodosEntidades(IEnumerable<IEntidade> entidades, EnumOperacao operacao)
        {
            var dicionario = new Dictionary<string, List<Entidade>>();
            this.VarrerEntidades(dicionario, entidades, new HashSet<Entidade>(), operacao);
            return dicionario;
        }

        private void VarrerEntidades(Dictionary<string, List<Entidade>> dicionario, IEnumerable<IEntidade> entidades, HashSet<Entidade> entidadesAnalisadas, EnumOperacao operacao)
        {
            foreach (var entidade in entidades)
            {
                this.VarrerEntidade(dicionario, entidade, entidadesAnalisadas, operacao);
            }
        }

        private void VarrerEntidade(Dictionary<string, List<Entidade>> dicionario, IEntidade entidade, HashSet<Entidade> entidadesAnalisadas, EnumOperacao operacao)
        {
            if (entidadesAnalisadas.Contains(entidade))
            {
                return;
            }
            if (this.AdicionarEntidadeDicionario(entidade, operacao))
            {
                if (!dicionario.ContainsKey(entidade.__NomeTipoEntidade))
                {
                    dicionario.Add(entidade.__NomeTipoEntidade, new List<Entidade>());
                }
                var lista = dicionario[entidade.__NomeTipoEntidade];
                lista.Add(entidade);
            }
            var estruturaEntidade = this.EstruturaBancoDados.EstruturasEntidade[entidade.__NomeTipoEntidade];
            foreach (var estruturaRelacao in estruturaEntidade.EstruturasRelacoes.Values)
            {
                var propriedade = estruturaRelacao.Propriedade;
                var valorPropriedade = propriedade.GetValue(entidade);
                if (valorPropriedade != null)
                {
                    if (estruturaRelacao.IsColecao)
                    {
                        var colecao = (IEnumerable<IEntidade>)valorPropriedade;
                        this.VarrerEntidades(dicionario, colecao, entidadesAnalisadas, operacao);
                    }
                    else
                    {
                        var entidadeRelacao = (IEntidade)valorPropriedade;
                        this.VarrerEntidade(dicionario, entidadeRelacao, entidadesAnalisadas, operacao);
                    }
                }
            }
        }

        private bool AdicionarEntidadeDicionario(IEntidade entidade, EnumOperacao operacao)
        {
            if (entidade.__IsExisteAlteracao)
            {
                if (entidade.Id == 0 && operacao == EnumOperacao.Adicionar)
                {
                    return true;
                }
                else if (!entidade.__IsNewEntity)
                {
                    if (operacao == EnumOperacao.Atualizar ||
                        operacao == EnumOperacao.Deletar ||
                        operacao == EnumOperacao.Leitura)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private HashSet<string> RetornarNomesTipoEntidade(EstruturaConsulta estruturaConsulta)
        {
            if (string.IsNullOrWhiteSpace(estruturaConsulta.NomeTipoEntidade))
            {
                throw new ErroOperacaoInvalida($"A propriedade NomeTipoEntidadeda EstruturaConsulta não foi definida.");
            }

            var nomesTipoEntidade = new HashSet<string>
            {
                estruturaConsulta.NomeTipoEntidade
            };

            foreach (var relacaoAberta in estruturaConsulta.RelacoesAberta.Values)
            {
                if (!string.IsNullOrWhiteSpace(relacaoAberta.NomeTipoEntidade))
                {
                    nomesTipoEntidade.Add(relacaoAberta.NomeTipoEntidade);
                }
            }
            foreach (var colecaoAberta in estruturaConsulta.ColecoesAberta.Values)
            {
                if (!string.IsNullOrWhiteSpace(colecaoAberta.NomeTipoEntidade))
                {
                    if (colecaoAberta.EstruturaConsulta is null)
                    {
                        throw new ErroOperacaoInvalida($"A propriedade EstruturaConsulta da ColeçãoAberta '{colecaoAberta.CaminhoPropriedade}' não foi definida.");
                    }
                    nomesTipoEntidade.Add(colecaoAberta.NomeTipoEntidade);
                    nomesTipoEntidade.AddRange(this.RetornarNomesTipoEntidade(colecaoAberta.EstruturaConsulta));
                }
            }
            return nomesTipoEntidade;
        }

        private List<EstruturaIdentificacao> RetornarEstruturasIdentificacao(IIdentificacao identificacao)
        {
            return this.RetornarEstruturasIdentificacao(identificacao, new Dictionary<Guid, EstruturaIdentificacao>());
        }

        private List<EstruturaIdentificacao> RetornarEstruturasIdentificacao(IIdentificacao identificacao, Dictionary<Guid, EstruturaIdentificacao> estruturas)
        {
            if (!estruturas.ContainsKey(identificacao.Identificador))
            {
                if (this.EstruturasIdentificacao.ContainsKey(identificacao.Identificador))
                {
                    estruturas.Add(identificacao.Identificador, this.EstruturasIdentificacao[identificacao.Identificador]);
                }
            }
            foreach (var grupo in identificacao.MembrosDe)
            {
                var estrurasGrupo = this.RetornarEstruturasIdentificacao(grupo, estruturas);
                foreach (var estruturGrupo in estrurasGrupo)
                {
                    if (!estruturas.ContainsKey(estruturGrupo.Identificacao.Identificador))
                    {
                        estruturas.Add(estruturGrupo.Identificacao.Identificador, estruturGrupo);
                    }
                }
            }
            return estruturas.Values.ToList();
        }
    }
}