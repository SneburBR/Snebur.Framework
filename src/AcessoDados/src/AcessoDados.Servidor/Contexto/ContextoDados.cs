using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Snebur.AcessoDados.Estrutura;
using Snebur.AcessoDados.Manutencao;
using Snebur.AcessoDados.Mapeamento;
using Snebur.AcessoDados.Seguranca;
using Snebur.AcessoDados.Servidor.Salvar;
using Snebur.Dominio;
using Snebur.Seguranca;
using Snebur.Servicos;
using Snebur.Utilidade;
using Snebur.Linq;

#if NET50
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

namespace Snebur.AcessoDados
{
    public abstract partial class BaseContextoDados : __BaseContextoDados, IServicoDados, IContextoDadosSemNotificar, IDisposable
    {
        public SqlSuporte SqlSuporte { get; }

        #region Propriedades

        private readonly string ConectionString;

        private bool IsAtivarSeguranca { get; }

        private bool IsValidarUsuarioSessaoUsuario { get; }

        private TiposSeguranca TiposSeguranca { get; }

        private bool InicializandoContexto { get; } = true;

        public bool IsAnonimo { get; } = true;

        //public Credencial CredencialUsuario { get; }

        private CacheSessaoUsuario CacheSessaoUsuario { get; }

        public bool IsCacheSessaoUsuarioInicializado => this.CacheSessaoUsuario != null;

        public ISessaoUsuario SessaoUsuarioLogado => this.CacheSessaoUsuario?.SessaoUsuario;

        public IUsuario UsuarioAvalista { get; }

        internal SeguracaContextoDados SeguracaContextoDados { get; }

        internal EstruturaBancoDados EstruturaBancoDados { get; }

        internal BaseConexao Conexao { get; }

        public string IdentificadorProprietario { get; private set; }

        public bool IsIdentificadorProprietarioGlobal { get; private set; }

        internal protected bool IsFiltrarIdentificadorProprietario { get; set; }

        internal protected virtual bool IsUsuarioAdministrador { get; } = false;

#if DEBUG
        public List<string> Comandos = new List<string>();
#endif

        public Type TipoEntidadeArquivo => this.EstruturaBancoDados.TipoEntidadeArquivo;

        public Type TipoEntidadeImagem => this.EstruturaBancoDados.TipoEntidadeImagem;

        internal Type TipoEntidadeHistoricoMenutencao => this.EstruturaBancoDados.TipoHistoricoManutencao;

        internal bool IsManutencaoAtiva => this.TipoEntidadeHistoricoMenutencao != null;

        public bool IsSessaoUsuarioAtiva => this.SessaoUsuarioLogado.Estado == EnumEstadoSessaoUsuario.Ativo;

        #endregion

        #region Construtor

        private BaseContextoDados(string configuracaoAcessoDados,
                                  string identificadorProprietario,
                                  EnumFlagBancoNaoSuportado flagsNaoSuporta)
        {
            ErroUtil.ValidarReferenciaNula(configuracaoAcessoDados, nameof(configuracaoAcessoDados));

            this.SqlSuporte = new SqlSuporte(flagsNaoSuporta);
            this.ConectionString = AplicacaoSnebur.Atual.ConnectionStrings[configuracaoAcessoDados] ?? throw new ErroNaoDefinido($"Não foi encontrada o String de conexao '{configuracaoAcessoDados}' no arquivo de configuração da aplicação ConnectionStrings App.Config ou Web.Config ");

            if (String.IsNullOrWhiteSpace(identificadorProprietario))
            {
                throw new ErroNaoDefinido("O identificador da instancia não foi definido");
            }

            this.IdentificadorProprietario = identificadorProprietario;
            this.IsIdentificadorProprietarioGlobal = Util.SaoIgual(this.IdentificadorProprietario.ToString(), ConfiguracaoAcessoDados.IdentificadorProprietarioGlobal);
            this.IsFiltrarIdentificadorProprietario = !this.IsIdentificadorProprietarioGlobal;

            this.Conexao = AjudanteConexaoDB.RetornarConexao(this, this.ConectionString);
            this.EstruturaBancoDados = EstruturaBancoDados.RetornarEstruturaBancoDados(this.GetType(), this.SqlSuporte);

            if (this.SqlSuporte.IsMigracao)
            {
                GerenciadorMigracao.Inicializar(this);

                if (this.IsManutencaoAtiva)
                {
                    GerenciadorManutencao.Inicializar(this);
                }

            }

            this.IsAtivarSeguranca = false;
            if (this.IsAtivarSeguranca)
            {
                this.TiposSeguranca = this.EstruturaBancoDados.TiposSeguranca;
                this.SeguracaContextoDados = SeguracaContextoDados.RetornarSegurancaoContextoDados(this);
            }
            this.PopularBancoInterno();

        }

        protected BaseContextoDados(string configuracaoAcessoDados,
                             CredencialUsuario credencial,
                             Guid identificadorSessaoUsario,
                             InformacaoSessaoUsuario informacaoSessaoUsuario,
                             string identificadorProprietario,
                             EnumFlagBancoNaoSuportado sqlNaoSuporta) : this(configuracaoAcessoDados,
                                                                    identificadorProprietario,
                                                                    sqlNaoSuporta)
        {
            ErroUtil.ValidarReferenciaNula(credencial, nameof(credencial));

            //this.IsSalvarScopo = isSalvarScopo;
            //ContextoDados.InicializarScopo(this);
            //this.CredencialUsuario = credencial;

            this.IsFiltrarIdentificadorProprietario = identificadorProprietario != null;
            this.CacheSessaoUsuario = CacheSessaoUsuario.RetornarCacheSessaoUsuario(this, credencial, identificadorSessaoUsario, informacaoSessaoUsuario);


            if (this.CacheSessaoUsuario.Usuario == null)
            {
                throw new ErroSessaoUsuarioInvalida("O usuário não foi definido no cache da sessão o usuário");
            }

            if (this.CacheSessaoUsuario.SessaoUsuario == null)
            {
                throw new ErroSessaoUsuarioInvalida("A sessão do usuário não foi definido no cache da sessão o usuário");
            }


            //this.ResultadoSessaoUsuario = this.CacheSessaoUsuario.RetornarResltadoSessaoUsuario(credencial, identificadorSessaoUsario, informacaoSessaoUsuario);
            //this.UsuarioLogado = this.ResultadoSessaoUsuario.Usuario;
            //this.SessaoUsuarioLogado = this.ResultadoSessaoUsuario.SessaoUsuario;


            //this.UsuarioLogado = this.AjudanteSessaoUsuario.RetornarUsuario(credencial);
            //this.SessaoUsuarioLogado = this.AjudanteSessaoUsuario.RetornarSessaoUsuario(this.UsuarioLogado, informacaoSessaoUsuario);

            //this.SalvarInternoSemNotificacao((Entidade)this.SessaoUsuarioLogado, true);
            this.IsValidarUsuarioSessaoUsuario = true;
            this.IsAnonimo = CredencialUtil.ValidarCredencial(this.UsuarioLogado, CredencialAnonimo.Anonimo);

            this.VerificarAutorizacaoUsuarioIdentificadorGlobal();
            this.InicializandoContexto = false;
        }

        protected BaseContextoDados(string configuracaoAcessoDados,
                             CredencialUsuario credencial,
                             Guid identificadorSessaoUsuario,
                             InformacaoSessaoUsuario informacaoSessaoUsuario,
                             string identificadorProprietario) :
                             this(configuracaoAcessoDados,
                                  credencial,
                                  identificadorSessaoUsuario,
                                  informacaoSessaoUsuario,
                                  identificadorProprietario,
                                  EnumFlagBancoNaoSuportado.SemRestricao)
        {

        }

        protected BaseContextoDados(string configuracaoAcessoDados,
                                    CredencialUsuario credencialUsuario,
                                    CredencialUsuario credencialAvalista,
                                    Guid identificadorSessaoUsuario,
                                    InformacaoSessaoUsuario informacaoSessaoUsuario,
                                    string identificadorProprietario,
                                    EnumFlagBancoNaoSuportado sqlNaoSuporta = EnumFlagBancoNaoSuportado.SemRestricao) :
                                    this(configuracaoAcessoDados,
                                         credencialUsuario,
                                         identificadorSessaoUsuario,
                                         informacaoSessaoUsuario,
                                         identificadorProprietario,
                                         sqlNaoSuporta)
        {

            if (credencialAvalista != null)
            {
                this.IsValidarUsuarioSessaoUsuario = false;
                this.UsuarioAvalista = this.CacheSessaoUsuario.RetornarUsuarioAvalista(credencialAvalista);
            }

            this.VerificarAutorizacaoUsuarioIdentificadorGlobal();
            this.InicializandoContexto = false;
            this.IsValidarUsuarioSessaoUsuario = true;
        }

        //protected ContextoDados(string configuracaoAcessoDados, bool isSemGerenciamentoUsurioEMigracao) :
        //                        this(configuracaoAcessoDados, ConfiguracaoAcessoDados.IdentificadorProprietarioGlobal, true)
        //{
        //    this.IsValidarUsuarioSessaoUsuario = false;
        //    this.IsAnonimo = true;

        //}
        #endregion

        #region BaseServico 

        public override bool Ping()
        {
            var data = this.RetornarDataHora();
            return (data.AddMinutes(-10) > new DateTime());

        }
        #endregion

        #region IServicoDados

        public override DateTime RetornarDataHora()
        {
            return this.Conexao.RetornarDataHora(false);
        }

        public override DateTime RetornarDataHoraUTC()
        {
            return this.Conexao.RetornarDataHora(true);
        }

        public override object RetornarValorScalar(EstruturaConsulta estruturaConsulta)
        {
            this.ValidarSessaoUsuario();

            using (var mapeamento = new MapeamentoConsultaValorSaclar(estruturaConsulta, this.EstruturaBancoDados, this.Conexao, this))
            {
                return mapeamento.RetornarValorScalar();
            }
        }


        #region Consulta

        public override ResultadoConsulta RetornarResultadoConsulta(EstruturaConsulta estruturaConsulta)
        {
            this.ValidarSessaoUsuario();

            var resultado = this.RetornarResutladoConsultaSeguranca(estruturaConsulta);
            if (resultado.Erro != null)
            {
                throw resultado.Erro;
            }
            return resultado;
        }

        internal ResultadoConsulta RetornarResutladoConsultaSeguranca(EstruturaConsulta estruturaConsulta)
        {
            var permissao = this.RetornarPermissaoLeitura(estruturaConsulta);
            if (permissao == EnumPermissao.Autorizado)
            {
                return this.RetornarResultadoConsultaInterno(estruturaConsulta);
            }
            return new ResultadoConsulta
            {
                Permissao = permissao,
                Erro = new ErroPermissao(permissao, $"Permissão {EnumUtil.RetornarDescricao(permissao)} ")
            };
        }

        internal EnumPermissao RetornarPermissaoLeitura(EstruturaConsulta estruturaConsulta)
        {
            if (!this.IsAtivarSeguranca)
            {
                return EnumPermissao.Autorizado;
            }
            if (!this.IsValidarUsuarioSessaoUsuario)
            {
                return EnumPermissao.Autorizado;
            }
            return this.SeguracaContextoDados.PermissaoLeitura(this.UsuarioLogado, this.UsuarioAvalista, estruturaConsulta);
        }

        internal ResultadoConsulta RetornarResultadoConsultaInterno(EstruturaConsulta estruturaConsulta)
        {
            using (var mapeamento = new MapeamentoConsulta(estruturaConsulta, this.EstruturaBancoDados, this.Conexao, this))
            {
                var resultado = mapeamento.RetornarResultadoConsulta();
                resultado.Entidades.ForEach(x => x.__PropriedadesAlteradas?.Clear());
                return resultado;
            }
        }
        #endregion

        #region Salvar


        public ResultadoSalvar Salvar(params IEntidade[] entidades)
        {
            var lista = new List<Entidade>();
            foreach (var entidade in entidades)
            {
                lista.Add((Entidade)entidade);
            }
            return this.Salvar(lista);
        }

        public ResultadoSalvar Salvar(params Entidade[] entidades)
        {
            var lista = new List<Entidade>();
            lista.AddRange(entidades);
            return this.Salvar(lista);
        }

        public ResultadoSalvar Salvar(ListaEntidades<Entidade> entidades)
        {
            var lista = new List<Entidade>();
            lista.AddRange(entidades);
            return this.Salvar(lista);
        }

        public ResultadoSalvar Salvar(IEnumerable<Entidade> entidades)
        {
            return this.Salvar(entidades.ToList(), false);
        }

        public override ResultadoSalvar Salvar(List<Entidade> entidades)
        {
            return this.Salvar(entidades, false);
        }

        public override ResultadoSalvar Salvar(Entidade entidade)
        {
            return this.Salvar(new List<Entidade> { entidade }, false);
        }

        public ResultadoSalvar Salvar(List<Entidade> entidades, bool ignorarErro)
        {
            this.ValidarSessaoUsuario();

            if (DebugUtil.IsAttached)
            {
                ignorarErro = false;
            }
            var resultado = this.SalvarPermissao(entidades);
            if (resultado.Erro != null && (!ignorarErro))
            {
                throw resultado.Erro;
            }
            return resultado;
        }

        private ResultadoSalvar SalvarPermissao(List<Entidade> entidades)
        {
            var permissao = this.SeguracaContextoDados?.PermissaoSalvar(this.UsuarioLogado, this.UsuarioAvalista, entidades) ?? EnumPermissao.Autorizado;
            if (permissao == EnumPermissao.Autorizado)
            {
                return this.SalvarInterno(entidades);
            }
            return new ResultadoSalvar
            {
                Permissao = permissao,
                Erro = new ErroPermissao(permissao, $"Permissão {EnumUtil.RetornarDescricao(permissao)} ")
            };
        }

        private ResultadoSalvar SalvarInterno(List<Entidade> entidades)
        {
            using (var salvarEntidades = new SalvarEntidades(this, entidades.ToHashSet(), false, true))
            {
                return (ResultadoSalvar)salvarEntidades.Salvar();
            }
        }
        #endregion

        #region Excluir


        //Excluir

        public ResultadoExcluir Excluir<TEntidade>(TEntidade entidade, Expression<Func<TEntidade, object>> expressaoPropriedade) where TEntidade : Entidade
        {
            var relacoes = Util.RetornarRelacoesAbertas<TEntidade>(expressaoPropriedade);
            return this.Excluir(new List<Entidade> { entidade }, relacoes);
        }

        public ResultadoExcluir Excluir<TEntidade>(IEnumerable<TEntidade> entidades, Expression<Func<TEntidade, object>> expressaoPropriedade) where TEntidade : Entidade
        {
            var relacoes = Util.RetornarRelacoesAbertas<TEntidade>(expressaoPropriedade);
            return this.Excluir(entidades.ToList<Entidade>(), relacoes);
        }

        public ResultadoExcluir Excluir<TEntidade>(TEntidade entidade, params Expression<Func<TEntidade, object>>[] expressoesPropriedade) where TEntidade : Entidade
        {
            var relacoes = Util.RetornarRelacoesAbertas<TEntidade>(expressoesPropriedade);
            return this.Excluir(new List<Entidade> { entidade }, relacoes);
        }

        public ResultadoExcluir Excluir<TEntidade>(IEnumerable<TEntidade> entidades, params Expression<Func<TEntidade, object>>[] expressoesPropriedade) where TEntidade : Entidade
        {
            var relacoes = Util.RetornarRelacoesAbertas<TEntidade>(expressoesPropriedade);
            return this.Excluir(entidades.ToList<Entidade>(), relacoes);
        }

        public override ResultadoExcluir Excluir(List<Entidade> entidades)
        {
            return this.Excluir(entidades, null, false);
        }
        public override ResultadoExcluir Excluir(List<Entidade> entidades, string relacoesEmCascata)
        {
            return this.Excluir(entidades, relacoesEmCascata, false);
        }

        public override ResultadoExcluir Excluir(Entidade entidade)
        {
            return this.Excluir(new List<Entidade> { entidade });
        }

        public override ResultadoExcluir Excluir(Entidade entidade, string relacoesEmCascata)
        {
            return this.Excluir(new List<Entidade> { entidade }, relacoesEmCascata);
        }

        public ResultadoExcluir Excluir(IEnumerable<Entidade> entidades, string relacoesEmCascata, bool ignorarErro)
        {
            this.ValidarSessaoUsuario();

            var entidadesExcluir = this.RetornarEntidadesExcluirEmCascata(entidades, relacoesEmCascata);
            using (var salvar = new SalvarEntidades(this, entidadesExcluir.ToHashSet(), true, false))
            {
                var resultado = salvar.Salvar();
                if (resultado.Erro != null && !ignorarErro)
                {
                    throw resultado.Erro;
                }
                return resultado as ResultadoExcluir;
            }
        }

        #endregion

        //public bool AtualizarOrdenacao(IOrdenacao entidade, IOrdenacao entidadeReferencia, EnumPosicaoOrdenacao posicaoOrdenacao)
        //{
        //    var estruturaEntidade = this.EstruturaBancoDados.EstruturasEntidade[entidade.GetType().Name];
        //    var estruturaCampoOrdenacao = estruturaEntidade.EstruturaCampoOrdenacao;

        //    return false;
        //}

        #endregion

        #region Métodos

        #region Públicos


        public string RetornarSql(IConsultaEntidade consulta)
        {
            return this.RetornarSql(consulta.RetornarEstruturaConsulta());
        }

        public string RetornarSql(EstruturaConsulta estruturaConsulta)
        {
            this.ValidarSessaoUsuario();

            using (var mapeamento = new MapeamentoConsulta(estruturaConsulta, this.EstruturaBancoDados, this.Conexao, this))
            {
                return mapeamento.RetornarSql();
            }
        }

        public List<TMapeamento> MapearSql<TMapeamento>(string sql)
        {
            this.ValidarSessaoUsuario();

            var tabela = this.Conexao.RetornarDataTable(sql, null);
            var resultado = new List<TMapeamento>();
            var colunas = tabela.Columns;
            var tipoMapeamento = typeof(TMapeamento);
            foreach (DataRow linha in tabela.Rows)
            {
                var intancia = Activator.CreateInstance<TMapeamento>();
                foreach (DataColumn coluna in colunas)
                {
                    var propriedade = tipoMapeamento.GetProperty(coluna.ColumnName);
                    if (propriedade != null)
                    {
                        var valor = linha[coluna];
                        var valorConvertido = ConverterUtil.Converter(valor, propriedade.PropertyType);
                        propriedade.SetValue(intancia, valorConvertido);
                    }
                }
                resultado.Add(intancia);
            }
            return resultado;
        }
        #endregion

        #region IContextoDadosSemNotificar 

        ResultadoSalvar IContextoDadosSemNotificar.SalvarInternoSemNotificacao(IEntidade entidade)
        {
            return (this as IContextoDadosSemNotificar).SalvarInternoSemNotificacao(entidade, false);
        }

        ResultadoSalvar IContextoDadosSemNotificar.SalvarInternoSemNotificacao(IEntidade entidade, bool ignorarValidacao)
        {
            return (this as IContextoDadosSemNotificar).SalvarInternoSemNotificacao(new IEntidade[] { entidade }, ignorarValidacao);
        }

        ResultadoSalvar IContextoDadosSemNotificar.SalvarInternoSemNotificacao(IEntidade[] p_entidades, bool ignorarValidacao)
        {
            var entidades = p_entidades.Cast<Entidade>().ToList();
            using (var salvarEntidades = new SalvarEntidades(this, entidades.ToHashSet(false), false, false))
            {
                var resultado = salvarEntidades.Salvar(ignorarValidacao);
                if (resultado.IsSucesso && resultado.Erro == null)
                {
                    foreach (var entidade in entidades)
                    {
                        entidade.__PropriedadesAlteradas?.Clear();
                        if (entidade.__PropriedadesAlteradas == null)
                        {
                            entidade.AtivarControladorPropriedadeAlterada();
                        }
                    }
                }
                if (resultado.Erro != null)
                {
                    throw resultado.Erro;
                }
                return resultado as ResultadoSalvar;
            }
        }

        #endregion

        #endregion

        #region Privados

        private void ValidarSessaoUsuario()
        {
            if (this.IsValidarUsuarioSessaoUsuario)
            {
                ValidacaoUtil.ValidarReferenciaNula(this.CacheSessaoUsuario, nameof(this.CacheSessaoUsuario));

                if (this.CacheSessaoUsuario.EstadoSessaoUsuario != EnumEstadoSessaoUsuario.Ativo)
                {
                    if (DebugUtil.IsAttached)
                    {
                        throw new Erro($"Usuário diferente da são ser foi um usuário global retomar no contexto, RetornarCredenciaisGlobais '{this.CacheSessaoUsuario.Usuario.Nome}'");
                    }

                    throw new ErroSessaoUsuarioExpirada(this.CacheSessaoUsuario.EstadoSessaoUsuario,
                        this.CacheSessaoUsuario.SessaoUsuario.IdentificadorSessaoUsuario,
                        $"O estão da sessão '{this.CacheSessaoUsuario.SessaoUsuario.IdentificadorSessaoUsuario.ToString()}' não é valido {this.CacheSessaoUsuario.EstadoSessaoUsuario.ToString()} ");
                }
            }

            //a validação é feita no Cache da sessão de dados
            //if (this.IsValidarUsuarioSessaoUsuario)
            //{
            //    if (this.IsValidarUsuarioSessaoUsuario)
            //    {
            //        ValidacaoUtil.ValidarReferenciaNula(this.UsuarioLogado, nameof(this.UsuarioLogado));
            //        ValidacaoUtil.ValidarReferenciaNula(this.SessaoUsuarioLogado, nameof(this.SessaoUsuarioLogado));
            //        ValidacaoUtil.ValidarReferenciaNula(this.CredencialUsuario, nameof(this.CredencialUsuario));
            //    }
            //    if (this.SessaoUsuarioLogado.Estado != EnumEstadoSessaoUsuario.Ativo)
            //    {
            //        throw new ErroSessaoUsuarioExpirada("A sessão do usuario expirou");
            //    }
            //    var credencialValida = CredencialUtil.ValidarCredencial(this.SessaoUsuarioLogado.Usuario, this.CredencialUsuario);
            //    if (!credencialValida)
            //    {
            //        throw new ErroSessaoUsuarioExpirada("A credencial do usuario é diferente da sessão do usuario");
            //    }
            //}
        }

        private void VerificarAutorizacaoUsuarioIdentificadorGlobal()
        {
            if (this.IsIdentificadorProprietarioGlobal)
            {
                if (!(this.UsuarioLogadoAutorizadoIdentificadorProprietarioGlobal()))
                {
                    throw new ErroSeguranca($" o usuário {this.UsuarioLogado.GetType().Name} identificador {this.UsuarioLogado.IdentificadorUsuario} não é autorizado acesso com Identificador da instancia global", EnumTipoLogSeguranca.IdentificadorProprietarioGlobalNaoAutorizado);
                }
            }
        }

        #endregion

        #region Abstratos

        /// <summary>
        /// Retornar lista de usuarios, que será automatica salva no sistema, 
        /// </summary>
        /// <returns></returns>
        protected abstract List<IUsuario> RetornarUsuariosSistema();

        internal protected virtual HashSet<CredencialUsuario> RetornarCredenciaisGlobais()
        {
            return new HashSet<CredencialUsuario>();
        }

        internal List<IUsuario> RetornarUsuariosSistemaInterno()
        {
            if (BaseContextoDados.IsPopularBanco)
            {
                return this.RetornarUsuariosSistema();
            }
            return new List<IUsuario>();
        }

        #endregion

        #region Popular Banco

        private static bool? __isPopularBanco = null;
        private static bool __isPopularBancoPendente = true;
        private static object __bloqueioPoularBanco = new object();

        private static bool IsPopularBanco
        {
            get
            {
                if (!BaseContextoDados.__isPopularBanco.HasValue)
                {
                    __isPopularBanco = ConverterUtil.ParaBoolean(AplicacaoSnebur.Atual.AppSettings["IsPopularBanco"]);
                }
                return BaseContextoDados.__isPopularBanco.Value;
            }
        }


        private void PopularBancoInterno()
        {
            if (BaseContextoDados.__isPopularBancoPendente)
            {
                if (!BaseContextoDados.IsPopularBanco)
                {
                    BaseContextoDados.__isPopularBancoPendente = false;
                    return;
                }

                lock (BaseContextoDados.__bloqueioPoularBanco)
                {
                    if (BaseContextoDados.__isPopularBancoPendente)
                    {
                        this.OnPopularBanco();
                        BaseContextoDados.__isPopularBancoPendente = false;
                    }
                }
            }
        }

        protected virtual void OnPopularBanco()
        {

        }
        #endregion

        #region Trasacao

        internal SqlTransaction TransacaoAtual { get; private set; }
        internal SqlConnection ConexaoAtual { get; private set; }

        public bool IsExisteTransacao => this.TransacaoAtual != null;

        public void IniciarNovaTransacao()
        {
            this.IniciarNovaTransacao(ConfiguracaoAcessoDados.IsolamentoLevelSalvarPadrao);
        }

        public void IniciarNovaTransacao(IsolationLevel isolamento)
        {
            if (this.IsExisteTransacao)
            {
                this.Commit();
            }
            //this.ValidarTransacaoAtual();
            this.ConexaoAtual = (SqlConnection)this.Conexao.RetornarNovaConexao();
            this.ConexaoAtual.Open();
            this.TransacaoAtual = this.ConexaoAtual.BeginTransaction(isolamento);
        }

        public void Commit()
        {
            this.ValidarTransacaoAtual();
            this.TransacaoAtual.Commit();
            this.DispensarTransacaoAtual();
        }

        public void Rollback()
        {
            //this.ValidarTransacaoAtual();
            this.TransacaoAtual?.Rollback();
            this.DispensarTransacaoAtual();
        }

        private void ValidarTransacaoAtual()
        {
            if (this.TransacaoAtual == null)
            {
                throw new Erro("Não existe nenhum transação aberta");
            }
        }

        private void DispensarTransacaoAtual()
        {
            this.ValidarTransacaoAtual();
            //this.ConexaoAtual.Close();
            this.TransacaoAtual.Dispose();
            this.ConexaoAtual.Dispose();
            this.TransacaoAtual = null;
            this.ConexaoAtual = null;
        }

        #endregion

        protected abstract bool UsuarioLogadoAutorizadoIdentificadorProprietarioGlobal();

        public override IUsuario RetornarUsuarioLogado()
        {
            return this.CacheSessaoUsuario?.Usuario;
        }

        public void DefinirIdentificadorProprietario(string identificadorProprietario)
        {
            if (this.IdentificadorProprietario != identificadorProprietario)
            {
                if (!this.IsIdentificadorProprietarioGlobal)
                {
                    throw new Erro("O identificador atual não é global");
                }
                this.IdentificadorProprietario = identificadorProprietario;
                this.IsIdentificadorProprietarioGlobal = false;
            }
        }

        #region IDisposable

        protected override void DisposeInterno()
        {
            //this.Usuario = null;
            //this.SessaoUsuario = null;
            //ContextoDados.DispensarScopo();
            base.DisposeInterno();
        }
        #endregion
    }

    public enum EnumFlagBancoNaoSuportado
    {
        SemRestricao = 0,
        OffsetFetch = 1,
        ColunaNomeTipoEntidade = 2,
        Usuario = 4,
        Migracao = 8,
    }
}