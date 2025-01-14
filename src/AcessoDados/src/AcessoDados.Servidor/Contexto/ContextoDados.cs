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
using System.Threading.Tasks;
 
#if NET6_0_OR_GREATER
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

namespace Snebur.AcessoDados
{
    public abstract partial class BaseContextoDados : __BaseContextoDados, IServicoDados, IContextoDadosSemNotificar, IDisposable
    {

        private CredencialUsuario _credencialAvalista;

        #region Propriedades
        public BancoDadosSuporta SqlSuporte { get; }

        private readonly string ConectionString;

        private bool IsAtivarSeguranca { get; }

        private bool IsValidarUsuarioSessaoUsuario { get; set; }

        private TiposSeguranca TiposSeguranca { get; }

        private bool IsContextoInicializado { get; } = false;

        public bool IsAnonimo { get; } = true;

        //public Credencial CredencialUsuario { get; }

        public CacheSessaoUsuario CacheSessaoUsuario { get; }
        public bool IsCacheSessaoUsuarioInicializado => this.CacheSessaoUsuario != null;
        public override IUsuario UsuarioLogado => this.CacheSessaoUsuario?.Usuario;
        public ISessaoUsuario SessaoUsuarioLogado => this.CacheSessaoUsuario?.SessaoUsuario;

        public IUsuario UsuarioAvalista { get; private set; }

        internal SeguracaContextoDados SeguracaContextoDados { get; }

        internal EstruturaBancoDados EstruturaBancoDados { get; }

        internal BaseConexao Conexao { get; }

        internal protected BaseContextoDados ContextoSessaoUsuario { get; }

        public bool IsContextoSessaoUsuario => this.ContextoSessaoUsuario == this;

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

        public bool IsSessaoUsuarioAtiva
        {
            get
            {
                if (this.SqlSuporte.IsSessaoUsuario)
                {
                    return this.SessaoUsuarioLogado.Status == EnumStatusSessaoUsuario.Ativo;
                }
                return true;
            }
        }


        public CredencialUsuario CredencialAvalista
        {
            get => this._credencialAvalista;
            set
            {
                this._credencialAvalista = value;
                this.IsValidarUsuarioSessaoUsuario = false;
                this.UsuarioAvalista = this.CacheSessaoUsuario.RetornarUsuarioAvalista(this._credencialAvalista);
                this.IsValidarUsuarioSessaoUsuario = true;

                if (this.ContextoSessaoUsuario != this)
                {
                    this.ContextoSessaoUsuario.CredencialAvalista = value;
                }
            }
        }

        internal HashSet<IInterceptador> InterceptoresAtivos { get; } = new HashSet<IInterceptador>();
        internal bool IsInterceptar { get; set; } = true;

        #endregion


        #region Construtor

        private BaseContextoDados(BaseContextoDados contextoSessaoUsuario,
                                  string configuracaoAcessoDados,
                                  string identificadorProprietario,
                                  EnumFlagBancoNaoSuportado flagsNaoSuporta)
        {
            ErroUtil.ValidarReferenciaNula(configuracaoAcessoDados, nameof(configuracaoAcessoDados));

            this.SqlSuporte = new BancoDadosSuporta(flagsNaoSuporta);
            this.ConectionString = AplicacaoSnebur.Atual.ConnectionStrings[configuracaoAcessoDados]
                    ?? throw new ErroNaoDefinido($"Não foi encontrada o String de conexão '{configuracaoAcessoDados}' no arquivo de configuração da aplicação ConnectionStrings App.Config ou Web.Config ");

            if (String.IsNullOrWhiteSpace(identificadorProprietario))
            {
                throw new ErroNaoDefinido("O identificador da instancia não foi definido");
            }

            this.IdentificadorProprietario = identificadorProprietario;
            this.IsIdentificadorProprietarioGlobal = Util.SaoIgual(this.IdentificadorProprietario.ToString(), ConfiguracaoAcessoDados.IdentificadorProprietarioGlobal);
            this.IsFiltrarIdentificadorProprietario = !this.IsIdentificadorProprietarioGlobal;

            this.ContextoSessaoUsuario = contextoSessaoUsuario ?? this;
            this.IsFiltrarIdentificadorProprietario = identificadorProprietario != null;

            this.Conexao = AjudanteConexaoDB.RetornarConexao(this, this.ConectionString);
            this.EstruturaBancoDados = EstruturaBancoDados.RetornarEstruturaBancoDados(this, this.SqlSuporte);

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
            (AplicacaoSnebur.Atual as IAplicacaoContextoDados)?.NovoConexaoDados(this);

        }

        protected BaseContextoDados(BaseContextoDados contextoSessaoUsuario,
                                    string configuracaoAcessoDados,
                                    CredencialUsuario credencial,
                                    Guid identificadorSessaoUsario,
                                    InformacaoSessao informacaoSessaoUsuario,
                                    string identificadorProprietario,
                                    EnumFlagBancoNaoSuportado sqlNaoSuporta) : this(contextoSessaoUsuario,
                                                                                   configuracaoAcessoDados,
                                                                                   credencial,
                                                                                   identificadorSessaoUsario,
                                                                                   informacaoSessaoUsuario,
                                                                                   identificadorProprietario,
                                                                                   sqlNaoSuporta,
                                                                                   true)
        {

        }

        protected BaseContextoDados(string configuracaoAcessoDados,
                                    CredencialUsuario credencial,
                                    Guid identificadorSessaoUsario,
                                    InformacaoSessao informacaoSessaoUsuario,
                                    string identificadorProprietario,
                                    EnumFlagBancoNaoSuportado sqlNaoSuporta) : this(null,
                                                                                    configuracaoAcessoDados,
                                                                                    credencial,
                                                                                    identificadorSessaoUsario,
                                                                                    informacaoSessaoUsuario,
                                                                                    identificadorProprietario,
                                                                                    sqlNaoSuporta,
                                                                                    true)
        {

        }
        private BaseContextoDados(BaseContextoDados contextoSessaoUsuario,
                                  string configuracaoAcessoDados,
                                  CredencialUsuario credencial,
                                  Guid identificadorSessaoUsario,
                                  InformacaoSessao informacaoSessaoUsuario,
                                  string identificadorProprietario,
                                  EnumFlagBancoNaoSuportado sqlNaoSuporta,
                                  bool isValidarUsuarioGlobal) : this(contextoSessaoUsuario,
                                                                      configuracaoAcessoDados,
                                                                      identificadorProprietario,
                                                                      sqlNaoSuporta)
        {
            ErroUtil.ValidarReferenciaNula(credencial, nameof(credencial));

            //this.IsSalvarScopo = isSalvarScopo;
            //ContextoDados.InicializarScopo(this);
            //this.CredencialUsuario = credencial;


            if (this.SqlSuporte.IsSessaoUsuario)
            {
                this.CacheSessaoUsuario = this.RetornarCacheSessaoUsuario(this, credencial, identificadorSessaoUsario, informacaoSessaoUsuario);

                if (this.CacheSessaoUsuario.Usuario == null)
                {
                    throw new ErroSessaoUsuarioInvalida("O usuário não foi definido no cache da sessão o usuário");
                }

                if (this.CacheSessaoUsuario.SessaoUsuario == null)
                {
                    throw new ErroSessaoUsuarioInvalida("A sessão do usuário não foi definido no cache da sessão o usuário");
                }

                this.IsValidarUsuarioSessaoUsuario = true;
                this.IsAnonimo = CredencialUtil.ValidarCredencial(this.UsuarioLogado, CredencialAnonimo.Anonimo);

                if (isValidarUsuarioGlobal)
                {
                    this.VerificarAutorizacaoUsuarioIdentificadorGlobal();
                }

            }

            this.IsContextoInicializado = !isValidarUsuarioGlobal;
        }

        protected BaseContextoDados(string configuracaoAcessoDados,
                             CredencialUsuario credencial,
                             Guid identificadorSessaoUsuario,
                             InformacaoSessao informacaoSessaoUsuario,
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
                                    InformacaoSessao informacaoSessaoUsuario,
                                    string identificadorProprietario,
                                    EnumFlagBancoNaoSuportado sqlNaoSuporta = EnumFlagBancoNaoSuportado.SemRestricao) :
                                    this(null,
                                         configuracaoAcessoDados,
                                         credencialUsuario,
                                         identificadorSessaoUsuario,
                                         informacaoSessaoUsuario,
                                         identificadorProprietario,
                                         sqlNaoSuporta,
                                         false)
        {

            if (credencialAvalista != null)
            {
                //this.IsValidarUsuarioSessaoUsuario = false;
                this.CredencialAvalista = credencialAvalista;
                //this.UsuarioAvalista = this.CacheSessaoUsuario.RetornarUsuarioAvalista(credencialAvalista);
            }

            this.VerificarAutorizacaoUsuarioIdentificadorGlobal();
            this.IsContextoInicializado = true;
            this.IsValidarUsuarioSessaoUsuario = true;
        }

        protected virtual CacheSessaoUsuario RetornarCacheSessaoUsuario(BaseContextoDados baseContextoDados,
                                                                        CredencialUsuario credencial,
                                                                        Guid identificadorSessaoUsario,
                                                                        InformacaoSessao informacaoSessaoUsuario)
        {
            if (this.SqlSuporte.IsSessaoUsuarioContextoAtual)
            {
                return GerenciadorCacheSessaoUsuario.Instancia.RetornarCacheSessaoUsuario(baseContextoDados,
                                                                                          credencial,
                                                                                          identificadorSessaoUsario,
                                                                                          informacaoSessaoUsuario);
            }

            if (this.SqlSuporte.IsSessaoUsuarioHerdada)
            {
                return this.ContextoSessaoUsuario.CacheSessaoUsuario;
            }
            throw new InvalidOperationException("O banco de dados não suporta sessão de usuário");
        }


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

            using (var mapeamento = new MapeamentoConsultaValorScalar(estruturaConsulta,
                                                                     this.EstruturaBancoDados,
                                                                     this.Conexao,
                                                                     this))
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

                return new ResultadoConsulta
                {
                    Entidades = resultado.Entidades,
                    TotalRegistros = resultado.TotalRegistros,
                };
            }
        }
        #endregion

        #region Salvar

        public Task<ResultadoSalvar> SalvarAsync(params IEntidade[] entidades)
        {
            return Task.Run(() => this.Salvar(entidades));
        }

        public ResultadoSalvar Salvar(params IEntidade[] entidades)
        {
            return this.Salvar(entidades, false);
        }

        public override ResultadoSalvar Salvar(IEnumerable<IEntidade> entidades)
        {
            return this.Salvar(entidades, false);
        }

        public override ResultadoSalvar Salvar(IEntidade entidade)
        {
            return this.Salvar(new List<IEntidade> { entidade }, false);
        }

        public virtual ResultadoSalvar Salvar(IEnumerable<IEntidade> entidades,
                                              bool isIgnorarErro)
        {
            this.ValidarSessaoUsuario();

            if (DebugUtil.IsAttached)
            {
                isIgnorarErro = false;
            }

            var resultado = this.SalvarPermissao(entidades);
            if (resultado.Erro != null && (!isIgnorarErro))
            {
                throw resultado.Erro;
            }
            return resultado;
        }

        private ResultadoSalvar SalvarPermissao(IEnumerable<IEntidade> entidades)
        {
            var permissao = this.SeguracaContextoDados?.PermissaoSalvar(this.UsuarioLogado,
                                                                        this.UsuarioAvalista,
                                                                        entidades) ?? EnumPermissao.Autorizado;
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

        private ResultadoSalvar SalvarInterno(IEnumerable<IEntidade> entidades)
        {
            var entidadesHashSet = entidades.OfType<Entidade>().ToHashSet();
            using (var salvarEntidades = new SalvarEntidades(this, entidadesHashSet,
                                                             EnumOpcaoSalvar.Salvar,
                                                             true))
            {
                return (ResultadoSalvar)salvarEntidades.Salvar();
            }
        }

        #endregion

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

        public Task<int> ExecutarSqlAsync(string sql, List<ParametroInfo> parametroInfos = null)
        {
            return Task.Run(() => this.ExecutarSql(sql, parametroInfos));
        }
        public int ExecutarSql(string sql, List<ParametroInfo> parametroInfos = null)
        {
            this.ValidarSessaoUsuario();
            //if (!Debugger.IsAttached)
            //{
            //    LogUtil.ErroAsync(new ErroSeguranca("Somente  é permitido executar SQL em modo de depuração", EnumTipoLogSeguranca.TentativaExecutarSql));
            //    return -1;
            //}
            return this.Conexao.ExecutarComando(sql, parametroInfos);
        }

        public T ExecutarSqlScalar<T>(string sql, List<ParametroInfo> parametroInfos = null)
        {
            this.ValidarSessaoUsuario();
            //if (!Debugger.IsAttached)
            //{
            //    LogUtil.ErroAsync(new ErroSeguranca("Somente é permitido executar SQL em modo de depuração", EnumTipoLogSeguranca.TentativaExecutarSql));
            //    return -1;
            //}

            return this.Conexao.RetornarValorScalar<T>(sql, parametroInfos);
        }

        public List<TMapeamento> MapearSql<TMapeamento>(string sql)
        {
            return this.MapearSql<TMapeamento>(sql, new List<ParametroInfo>());
        }
        public List<TMapeamento> MapearSql<TMapeamento>(string sql,
                                                List<ParametroInfo> parametros)
        {
            this.ValidarSessaoUsuario();

            var tabela = this.Conexao.RetornarDataTable(sql, parametros);
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
            using (var salvarEntidades = new SalvarEntidades(this, entidades.ToHashSet(false), EnumOpcaoSalvar.Salvar, false))
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


        public virtual void NotificarSessaoUsuarioAtiva(IUsuario usuario,
                                                        ISessaoUsuario sessaoUsuario)
        {
            var nowUtc = DateTime.UtcNow;
            usuario.DataHoraUltimoAcesso = nowUtc;
            sessaoUsuario.DataHoraUltimoAcesso = nowUtc;
            sessaoUsuario.Status = EnumStatusSessaoUsuario.Ativo;
            (this as IContextoDadosSemNotificar).SalvarInternoSemNotificacao(new IEntidade[] { usuario, sessaoUsuario }, false);
        }

        public virtual bool IsContinuarSeSessaoInvalida()
        {
            return false;
        }

        #endregion

        #endregion

        #region Privados

        private void ValidarSessaoUsuario()
        {
            if (this.IsValidarUsuarioSessaoUsuario)
            {
                ValidacaoUtil.ValidarReferenciaNula(this.CacheSessaoUsuario, nameof(this.CacheSessaoUsuario));

                if (this.CacheSessaoUsuario.StatusSessaoUsuario != EnumStatusSessaoUsuario.Ativo)
                {
                    if (DebugUtil.IsAttached)
                    {
                        throw new Erro($"Usuário diferente da são ser foi um usuário global retomar no contexto, RetornarCredenciaisGlobais '{this.CacheSessaoUsuario.Usuario.Nome}'");
                    }

                    if (this.CacheSessaoUsuario.SessaoUsuario != null &&
                        this.CacheSessaoUsuario.Usuario != null)
                    {
                        var usuario = this.CacheSessaoUsuario.Usuario;
                        var sessaoUsuario = this.CacheSessaoUsuario.SessaoUsuario;

                        if (this.IsContinuarSeSessaoInvalida())
                        {
                            this.NotificarSessaoUsuarioAtiva(usuario, sessaoUsuario);
                            return;
                        }
                    }
                    throw new ErroSessaoUsuarioExpirada(this.CacheSessaoUsuario.StatusSessaoUsuario,
                                                        this.CacheSessaoUsuario.SessaoUsuario.IdentificadorSessaoUsuario,
                                                        $"O status da sessão '{this.CacheSessaoUsuario.SessaoUsuario.IdentificadorSessaoUsuario}' não é valido {this.CacheSessaoUsuario.StatusSessaoUsuario.ToString()} ");
                }
            }
        }

        private void VerificarAutorizacaoUsuarioIdentificadorGlobal()
        {
            if (this.IsIdentificadorProprietarioGlobal)
            {
                if (!(this.IsUsuarioLogadoAutorizadoIdentificadorProprietarioGlobal()))
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

        internal protected abstract int IdNamespace { get; }

        #endregion
         
        #region Transação

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

        public Type[] TiposEntidade => this.EstruturaBancoDados.TiposEntidade.Values.ToArray();
        public abstract bool IsUsuarioLogadoAutorizadoIdentificadorProprietarioGlobal();

        #region Somente Leitura

        private Lazy<HashSet<EstruturaCampo>> __estruturasCamposSomenteLeituraSobreEscrever
            = new Lazy<HashSet<EstruturaCampo>>(() => new HashSet<EstruturaCampo>());

        public void PermitirSobreSomenteLeitura<TEntidade>(
                    params Expression<Func<TEntidade, object>>[] expressoes)
        {
            var tipoEntidade = typeof(TEntidade);
            if (!this.EstruturaBancoDados.EstruturasEntidade.
                                        TryGetValue(tipoEntidade.Name,
                                        out var estruturaEntidade))
            {
                throw new Exception($"Estrutura entidade não encontrada para o tipo {tipoEntidade.Name}");

            }
            foreach (var expresssao in expressoes)
            {
                var propriedade = ExpressaoUtil.RetornarPropriedade(expresssao);
                if (!estruturaEntidade.EstruturasCampos.TryGetValue(propriedade.Name,
                                                       out var estruturCampo))
                {
                    throw new Exception($"Estrutura do campo não encontrada para a propriedade {propriedade.Name}" +
                                        $" declarada  {propriedade.DeclaringType.Name} não possui o atributo somente leitura");
                }

                if (!estruturCampo.OpcoesSomenteLeitura.IsSomenteLeitura)
                {
                    if (DebugUtil.IsAttached)
                    {
                        throw new Exception($"A propriedade {propriedade.Name} declarada  {propriedade.DeclaringType.Name} não possui o atributo somente leitura");
                    }
                }
                this.__estruturasCamposSomenteLeituraSobreEscrever.Value.Add(estruturCampo);
            }
        }

        internal bool IsPodeSobreEscrever(EstruturaCampo estruturaCampo)
        {
            if (this.__estruturasCamposSomenteLeituraSobreEscrever.IsValueCreated)
            {
                return this.__estruturasCamposSomenteLeituraSobreEscrever.Value.Contains(estruturaCampo);
            }
            return false;

        }

        #endregion

        public void DefinirIdentificadorProprietario(string identificadorProprietario)
        {
            if (this.IdentificadorProprietario != identificadorProprietario)
            {
                if (!DebugUtil.IsAttached && !this.IsIdentificadorProprietarioGlobal)
                {
                    throw new Erro("O identificador atual não é global");
                }
                this.IdentificadorProprietario = identificadorProprietario;
                this.IsIdentificadorProprietarioGlobal = false;
            }
        }

        public void DesativarInterceptores()
        {
            this.IsInterceptar = false;
        }

        public void AtivarInterceptores()
        {
            this.IsInterceptar = true;
        }

        #region


        #endregion

        #region IDisposable

        protected override void DisposeInterno()
        {
            //this.Usuario = null;
            //this.SessaoUsuario = null;
            //ContextoDados.DispensarScopo();
            if (this.__estruturasCamposSomenteLeituraSobreEscrever.IsValueCreated)
            {
                this.__estruturasCamposSomenteLeituraSobreEscrever.Value.Clear();
                this.__estruturasCamposSomenteLeituraSobreEscrever = null;
            }

            base.DisposeInterno();
            (AplicacaoSnebur.Atual as IAplicacaoContextoDados)?.ConexaoDadosDispensado(this);
            //this.ContextoSessaoUsuarioHerdada?.Dispose();
        }
        #endregion
    }

}