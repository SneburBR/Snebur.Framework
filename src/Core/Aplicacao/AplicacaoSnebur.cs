using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Timers;
using Snebur.Comunicacao;
using Snebur.Dominio;
using Snebur.Seguranca;
using Snebur.Servicos;
using Snebur.UI;
using Snebur.Utilidade;
using System.Net;

#if NetCore
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif

namespace Snebur
{
    public abstract partial class AplicacaoSnebur
    {
        #region Constantes

        private const int INTERVALO_NOTIFICAR_APLICACAO_ATIVA = 10;
        private const int TEMPO_VERIFICAR_INICIALIZACAO = 30;
        private const string NOME_CULTURA_PADRAO = "pt-BR";

        #endregion

        #region Propriedades 

        protected Dictionary<string, string> UrlsServico { get; } = new Dictionary<string, string>();
        private string _identificadorAplicacao;
        private string _nomeAplicacao;
        private string _nomeEmpresa;
        private Version _versaoAplicacao;
        private EnumTipoAplicacao? _tipoAplicacao;
        private bool? _capturarPrimeiroErroAtivo;
        private CultureInfo _cultura;

        private IServicoLogErro _servicoLogErro;
        private IServicoLogSeguranca _servicoLogSeguranca;
        private IServicoLogAplicacao _servicoLogAplicacao;
        private IServicoLogDesempenho _servicoLogDesempenho;
        private IServicoUsuario _servicoUsuario;
        private object _bloqueioInicializar = new object();

        private readonly Timer TimerAplicacaoAtiva;

        private TimeSpan? _diferencaDataHoraServidor;
        private NameValueCollection _appSettings;
        private NameValueCollection _connectionStrings;

        public virtual string UrlPingInternetConectada { get; set; }

        public IAlerta Alerta { get; set; }

        internal protected virtual dynamic DispatcherObject { get; } = null;

        public virtual bool IsMainThread { get; } = false;

        protected virtual bool IsNotificarLogAplicacaoInicializada { get; } = true;

        public virtual string UrlWebService => this.RetornarUrlServico("UrlWebService");
        public virtual string UrlServicoArquivo => this.RetornarUrlServico("UrlServicoArquivo");
        public virtual string UrlServicoDados => this.RetornarUrlServico("UrlServicoDados") ??
                                                 this.UrlWebService;
        public virtual string UrlServicoImagem => this.RetornarUrlServico("UrlServicoImagem") ??
                                                  this.UrlServicoArquivo;

        public virtual bool IsSeperarAppDataPorIdentificadorPropretario { get; } = true;

        public virtual string IdentificadorAplicacao
        {
            get
            {
                if (this._identificadorAplicacao == null)
                {
                    this._identificadorAplicacao = this.FuncaoRetornarIdentificadorAplicacao.Invoke();
                }
                return this._identificadorAplicacao;
            }
        }

        public virtual CultureInfo CulturaPadrao
        {
            get
            {
                if (this._cultura == null)
                {
                    this._cultura = this.FuncaoRetornarCulturaPadrao.Invoke();
                }
                return this._cultura;
            }
        }

        public virtual string IdentificadorProprietario
        {
            get
            {
                return this.FuncaoRetornarIdentificadorProprietario.Invoke();
            }
        }

        public virtual CredencialUsuario CredencialUsuario
        {
            get
            {
                return this.FuncaoRetornarCredencialUsuarioUsuario.Invoke();
            }
        }

        public virtual Guid IdentificadorSessaoUsuario
        {
            get
            {
                return this.FuncaoRetornarIdentificadorSessaoUsuario.Invoke();
            }
        }

        public bool IsUsuarioAnonimo => this.CredencialUsuario.Validar(CredencialAnonimo.Anonimo);

        public bool CapturarPrimeiroErroAtivo
        {
            get
            {
                if (!this._capturarPrimeiroErroAtivo.HasValue)
                {
                    this._capturarPrimeiroErroAtivo = false;
                    //this.ServicoErro.CapturarPrimeiroErro();
                }
                return this._capturarPrimeiroErroAtivo.Value;
            }
        }

        public string NomeAplicacao
        {
            get
            {
                if (this._nomeAplicacao == null)
                {
                    this._nomeAplicacao = ReflexaoUtil.RetornarNomeAplicacao();
                }
                return this._nomeAplicacao;
            }
        }

        public string NomeEmpresa
        {
            get
            {
                if (this._nomeEmpresa == null)
                {
                    this._nomeEmpresa = ReflexaoUtil.RetornarNomeEmpresa();
                }
                return this._nomeEmpresa;
            }
        }

        public string CaminhoExecutavel
        {
            get
            {
                return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            }
        }

        public virtual EnumAmbienteServidor AmbienteServidor => this.FuncaoRetornarAmbienteServidor.Invoke();

        public string DiretorioAplicacao
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public Version VersaoAplicao
        {
            get
            {
                if (this._versaoAplicacao == null)
                {
                    this._versaoAplicacao = SistemaUtil.VersaoAplicacao;
                }
                return this._versaoAplicacao;
            }
        }

        private object BloqueioDataHoraServidor = new object();
        public TimeSpan DiferencaDataHoraUtcServidor
        {
            get
            {
                if (!this._diferencaDataHoraServidor.HasValue)
                {
                    lock (this.BloqueioDataHoraServidor)
                    {
                        if (!this._diferencaDataHoraServidor.HasValue)
                        {
                            var dataHoraServidor = this.FuncaoRetornarDataHoraUtcServidor.Invoke();
                            this._diferencaDataHoraServidor = dataHoraServidor - DateTime.UtcNow;
                        }
                    }
                }
                return this._diferencaDataHoraServidor.Value;
            }
        }

        public IServicoLogErro ServicoErro
        {
            get
            {
                if (this._servicoLogErro == null && this.FuncaoRetornarServicoLogErro != null)
                {
                    this._servicoLogErro = this.FuncaoRetornarServicoLogErro.Invoke();
                }
                return this._servicoLogErro;
            }
        }

        public IServicoLogSeguranca ServicoSeguranca
        {
            get
            {
                if (this._servicoLogSeguranca == null && this.FuncaoRetornarServicoLogSeguranca != null)
                {
                    this._servicoLogSeguranca = this.FuncaoRetornarServicoLogSeguranca.Invoke();
                }
                return this._servicoLogSeguranca;
            }
        }

        public IServicoLogAplicacao ServicoLogAplicacao
        {
            get
            {
                if (this._servicoLogAplicacao == null && this.FuncaoRetornarServicoLogAplicacao != null)
                {
                    this._servicoLogAplicacao = this.FuncaoRetornarServicoLogAplicacao.Invoke();
                }
                return this._servicoLogAplicacao;
            }
        }

        public IServicoLogDesempenho ServicoDesempenho
        {
            get
            {
                if (this._servicoLogDesempenho == null && this.FuncaoRetornarServicoLogDesempenho != null)
                {
                    this._servicoLogDesempenho = this.FuncaoRetornarServicoLogDesempenho.Invoke();
                }
                return this._servicoLogDesempenho;
            }
        }

        public IServicoUsuario ServicoUsuario
        {
            get
            {
                if (this._servicoUsuario == null && this.FuncaoRetornarServicoUsuario != null)
                {
                    this._servicoUsuario = this.FuncaoRetornarServicoUsuario.Invoke();
                }
                return this._servicoUsuario;
            }
        }

        public virtual EnumTipoAplicacao TipoAplicacao
        {
            get
            {
                //if(this.HttpContext!= null)
                //{
                //    return EnumTipoAplicacao.DotNet_WebService;
                //}

                if (!this._tipoAplicacao.HasValue)
                {
                    this._tipoAplicacao = SistemaUtil.TipoAplicacao;
                }
                return this._tipoAplicacao.Value;
            }
        }

        public string NomeComputador
        {
            get
            {
                return Environment.MachineName;
            }
        }

        public bool AtivarLogServicoOnline { get; private set; }

        public bool Inicializada { get; private set; }

        internal TimeSpan TempoVerificarInicializacao
        {
            get
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    // return TimeSpan.FromHours(10);
                }
                return TimeSpan.FromSeconds(TEMPO_VERIFICAR_INICIALIZACAO);
            }
        }

        public string CaminhoAppDataAplicacao
        {
            get
            {
                return ConfiguracaoUtil.CaminhoAppDataAplicacao;
            }
        }

        public virtual HttpContext HttpContext
        {
            get
            {
#if NetCore
                return this.FuncaoRetornaHttpContextAtual?.Invoke();
#else

                return HttpContext.Current;
#endif
            }
        }

        public virtual NameValueCollection AppSettings
        {
            get
            {
                return ThreadUtil.RetornarValorComBloqueio(ref this._appSettings, this.RetornarAppSettings);
            }
        }

        public virtual NameValueCollection ConnectionStrings
        {
            get
            {
                return ThreadUtil.RetornarValorComBloqueio(ref this._connectionStrings, this.RetornarConnectionStrings);
            }
        }
        #endregion

        #region Propreidade funcoes
        protected Func<CultureInfo> FuncaoRetornarCulturaPadrao { get; set; } = AplicacaoSnebur.RetornarCultauraPadrao;

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected Func<string> FuncaoRetornarIdentificadorProprietario { get; set; } = AplicacaoSnebur.RetornarIdentificadorProprietario;

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected Func<string> FuncaoRetornarIdentificadorAplicacao { get; set; } = AplicacaoSnebur.RetornarIdentificadorAplicacao;

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected Func<Guid> FuncaoRetornarIdentificadorSessaoUsuario { get; set; } = SessaoUtil.RetornarIdentificadorSessaoUsuario;

        protected Func<CredencialUsuario> FuncaoRetornarCredencialUsuarioUsuario { get; set; } = SessaoUtil.RetornarCredencialUsuario;

        protected Func<DateTime> FuncaoRetornarDataHoraUtcServidor { get; set; } = AplicacaoSnebur.RetornarDataHoraUtcServidor;

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected Func<InformacaoSessaoUsuario> FuncaoInformacaoSessaoUsuarioAtual { get; set; } = AplicacaoSnebur.InformacaoSessaoUsuarioAtual;

        protected Func<IInformacaoSessao> FuncaoRetornarInformacaoSessaoUsuario { get; set; } = SessaoUtil.RetornarInformacaoSessaoUsuarioAtual;

        protected Action AcaoIniciarNovaSessaoUsuario { get; set; } = SessaoUtil.InicializarNovaSessaoUsuario;

        protected Func<IServicoLogErro> FuncaoRetornarServicoLogErro { get; set; } = AplicacaoSnebur.RetornarServicoErro;

        protected Func<IServicoLogSeguranca> FuncaoRetornarServicoLogSeguranca { get; set; } = AplicacaoSnebur.RetornarServicoLogSeguranca;

        protected Func<IServicoLogAplicacao> FuncaoRetornarServicoLogAplicacao { get; set; } = AplicacaoSnebur.RetornarServicoLogAplicacao;

        protected Func<IServicoLogDesempenho> FuncaoRetornarServicoLogDesempenho { get; set; } = AplicacaoSnebur.RetornarServicoLogDesempenho;

        protected Func<IServicoUsuario> FuncaoRetornarServicoUsuario { get; set; } = null;

        public Func<BaseInformacaoAdicionalServicoCompartilhado> FuncaoRetornarInformacaoAdicionalServicoCompartilhado { get; set; } = null;

        public Func<EnumAmbienteServidor> FuncaoRetornarAmbienteServidor { get; protected set; } = AplicacaoSnebur.RetornarAmbienteServidor;

        public bool IgnorarErros
        {
            get
            {
                return Convert.ToBoolean(this.AppSettings["IgnorarErros"]);
            }
        }

        public event EventHandler NovaSessaoUsuarioInicializada;

#if NetCore
        protected Func<HttpContext> FuncaoRetornaHttpContextAtual { get; set; }
#endif

        #endregion

        #region Construtor

        protected AplicacaoSnebur()
        {
            AppDomain.CurrentDomain.UnhandledException += this.Aplicacao_UnhandledException;
            //AppDomain.CurrentDomain.FirstChanceException += this.Aplicacao_FirstChanceException;
            AppDomain.CurrentDomain.ProcessExit += this.Aplicacao_ProcessExit;

            this.TimerAplicacaoAtiva = new Timer((int)TimeSpan.FromMinutes(INTERVALO_NOTIFICAR_APLICACAO_ATIVA).TotalMilliseconds);
            this.TimerAplicacaoAtiva.Elapsed += this.Aplicacao_NotificarAplicacaoAtiva;

            System.Threading.Thread.CurrentThread.CurrentCulture = this.CulturaPadrao;
            System.Threading.Thread.CurrentThread.CurrentUICulture = this.CulturaPadrao;

            //WPF
            //Implementar classe AplicacaoSneburWpf que herdade AplicacaoSnebur
            //FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            this.InicializarComunicacao();

            // ThreadUtil.ExecutarDepoisAsync(this.VerificarAplicacaoInicializada, this.TempoVerificarInicializacao);
        }

        public virtual void InicializarAsync()
        {
            ThreadUtil.ExecutarAsync(this.Inicializar);
        }

        public virtual void Inicializar()
        {
            if (!this.Inicializada)
            {
                lock (this._bloqueioInicializar)
                {
                    if (!this.Inicializada)
                    {
                        this.TimerAplicacaoAtiva.Start();
                        this.InicializarServicoUsuario();

                        if (this.IsNotificarLogAplicacaoInicializada)
                        {
                            LogUtil.LogAsync("Aplicação inicializada");
                            LogUtil.LogAplicacaoAtiva();
                        }
                    }
                }
            }
        }
        #endregion

        #region Comunicação

        private void InicializarComunicacao()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            //Ignorar eerros da certificados Https, isso pode ocorres caso da DataHora do computador cliente do usuario esteja mais 12 horas de diferença com servidor
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object obj,
                                                                                          System.Security.Cryptography.X509Certificates.X509Certificate X509certificate,
                                                                                          System.Security.Cryptography.X509Certificates.X509Chain chain,
                                                                                          System.Net.Security.SslPolicyErrors errors)
            {
                return true;
            };

            this.DefinirConfiguracoesHttps();
        }
        //Conexões https
        private void DefinirConfiguracoesHttps()
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 |
                                                       SecurityProtocolType.Tls12 |
                                                       SecurityProtocolType.Tls11 |
                                                       SecurityProtocolType.Tls;

                ServicePointManager.DefaultConnectionLimit = 256;

#if NetCore == false

                var assemblyNet = Assembly.GetAssembly(typeof(System.Net.Configuration.SettingsSection));
                if (assemblyNet != null)
                {
                    var aSettingsType = assemblyNet.GetType("System.Net.Configuration.SettingsSectionInternal");
                    if (aSettingsType != null)
                    {
                        object anInstance = aSettingsType.InvokeMember("Section",
                          BindingFlags.Static |
                          BindingFlags.GetProperty |
                          BindingFlags.NonPublic, null, null, new object[] { });

                        if (anInstance != null)
                        {
                            FieldInfo aUseUnsafeHeaderParsing = aSettingsType.GetField("useUnsafeHeaderParsing", BindingFlags.NonPublic | BindingFlags.Instance);
                            if (aUseUnsafeHeaderParsing != null)
                            {
                                aUseUnsafeHeaderParsing.SetValue(anInstance, true);
                            }
                        }
                    }
                }
#endif
            }
            catch
            {
            }
        }
        #endregion

        #region ServicoUsuario

        private void InicializarServicoUsuario()
        {
            var servicoUsuario = this.ServicoUsuario;
            if (servicoUsuario != null)
            {
                if (!servicoUsuario.SessaoUsuarioAtiva(this.CredencialUsuario, this.IdentificadorSessaoUsuario))
                {
                    this.IniciarNovaSessaoAnonima();
                }
            }
        }

        public virtual void IniciarNovaSessaoAnonima()
        {
            this.AcaoIniciarNovaSessaoUsuario.Invoke();
            this.OnNovaSessaoUsuarioInicializada();
        }
        protected virtual void OnNovaSessaoUsuarioInicializada()
        {
            this.NovaSessaoUsuarioInicializada?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Log aplicação inicializada

        private void VerificarAplicacaoInicializada()
        {
            if (!this.Inicializada)
            {
                //if (System.Diagnostics.Debugger.IsAttached)
                //{
                //    throw new Exception("Chamar o método Inicializar logo apos instanciar a AplicacaoSnebur");
                //}
                this.Inicializar();
            }
        }

        private void AtivarLogAplicacao()
        {
            LogUtil.AtivarLogServicoOnlineAsync((resultado) =>
            {
                this.AtivarLogServicoOnline = resultado;
            });
        }
        #endregion

        #region Manipuladores de erros

        private void Aplicacao_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            if (this.CapturarPrimeiroErroAtivo)
            {
                throw new NotImplementedException();
            }
        }

        private void Aplicacao_ProcessExit(object sender, EventArgs e)
        {
            if (this.Inicializada)
            {
                var informacaoAdicional = ServicoCompartilhadoUtil.RetornarInformacaoAdicionalServicoCompartilhado();
                LogUtil.Log("Aplicação finalizada", informacaoAdicional, false);
            }
        }

        private void Aplicacao_NotificarAplicacaoAtiva(object sender, ElapsedEventArgs e)
        {
            LogUtil.LogAplicacaoAtivaAsync();
        }

        private void Aplicacao_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
            {
                try
                {
                    LogUtil.ErroGlobal(e.ExceptionObject as Exception);
                }
                catch
                {
                }
            }
        }
        #endregion

        #region Métodos públicos

        public void AtualizarDiferencaDataHoraServidor()
        {
            var dataHoraServidor = this.FuncaoRetornarDataHoraUtcServidor.Invoke();
            this._diferencaDataHoraServidor = dataHoraServidor - DateTime.UtcNow;
        }
        #endregion

        #region Métodos privados - manipuladores

        private static string RetornarIdentificadorAplicacao()
        {
            if (AplicacaoSnebur.Atual.AppSettings[SessaoUtil.IDENTIFICADOR_APLICACAO] != null)
            {
                return AplicacaoSnebur.Atual.AppSettings[SessaoUtil.IDENTIFICADOR_APLICACAO];
            }
            var assemblyEntrada = ReflexaoUtil.AssemblyEntrada;
            if (assemblyEntrada != null)
            {
                var assemblyName = new AssemblyName(assemblyEntrada.FullName);
                var identificadorAplicacao = assemblyName.Name;
#if NET50
                const string NET50 = ".Net50";
                if (identificadorAplicacao.EndsWith(NET50))
                {
                    identificadorAplicacao = identificadorAplicacao.Substring(0, identificadorAplicacao.Length - NET50.Length);
                }
#endif
                return identificadorAplicacao;
            }
            throw new Erro("Não foi possivel retornaor o identificador da aplicação");
        }

        private static string RetornarIdentificadorProprietario()
        {
            if (AplicacaoSnebur.Atual.AppSettings[SessaoUtil.IDENTIFICADOR_PROPRIETARIO] != null)
            {
                return AplicacaoSnebur.Atual.AppSettings[SessaoUtil.IDENTIFICADOR_PROPRIETARIO];
            }
            return ConfiguracaoUtil.IDENTIFICADOR_PROPRIETARIO_GLOBAL;
        }

        private static CultureInfo RetornarCultauraPadrao()
        {
            return new CultureInfo(NOME_CULTURA_PADRAO);
        }

        private static DateTime RetornarDataHoraUtcServidor()
        {
            return DateTime.UtcNow;
        }

        private static InformacaoSessaoUsuario InformacaoSessaoUsuarioAtual()
        {
            return SessaoUtil.RetornarInformacaoSessaoUsuarioAtual();
        }

        private static IServicoLogErro RetornarServicoErro()
        {
            return new ServicoErroLocal();
        }

        private static IServicoLogSeguranca RetornarServicoLogSeguranca()
        {
            return new ServicoLogSegurancaLocal();
        }

        private static IServicoLogAplicacao RetornarServicoLogAplicacao()
        {
            return new ServicoLogAplicacaoLocal();
        }

        private static IServicoLogDesempenho RetornarServicoLogDesempenho()
        {
            return new ServicoLogDesempenhoLocal();
        }

        private static EnumAmbienteServidor RetornarAmbienteServidor()
        {
            return ConfiguracaoUtil.AmbienteServidor;
        }
        #endregion

        protected virtual string RetornarUrlServico(string chaveConfiguracao)
        {
            if (!this.UrlsServico.ContainsKey(chaveConfiguracao))
            {
                lock (((ICollection)this.UrlsServico).SyncRoot)
                {
                    if (!this.UrlsServico.ContainsKey(chaveConfiguracao))
                    {
                        var urlServico = this.AppSettings[chaveConfiguracao];
                        if (!String.IsNullOrWhiteSpace(urlServico))
                        {
                            urlServico = AmbienteServidorUtil.NormalizarUrl(this.AppSettings[chaveConfiguracao]);
                        }
                        this.UrlsServico.Add(chaveConfiguracao, urlServico);
                    }
                }
            }
            return this.UrlsServico[chaveConfiguracao];
        }

        protected virtual NameValueCollection RetornarAppSettings()
        {
#if NetCore
            throw new Exception("Esse método deve ser implementado na aplicação de entrada");
#else
            var appSettins = new NameValueCollection();
            foreach (var chave in ConfigurationManager.AppSettings.AllKeys)
            {
                appSettins.Add(chave, ConfigurationManager.AppSettings[chave]);
            }
            return appSettins;
#endif
        }

        protected virtual NameValueCollection RetornarConnectionStrings()
        {
#if NetCore
            throw new Exception("Esse método deve ser implementado na aplicação de entrada");
#else

            var connectionStrings = new NameValueCollection();
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
            {
                connectionStrings.Add(connectionString.Name, connectionString.ConnectionString);
            }
            return connectionStrings;
#endif
        }
    }
}