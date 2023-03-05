using Snebur.Dominio;
using Snebur.Seguranca;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;


namespace Snebur.Utilidade
{
    public static class SessaoUtil
    {
        //public const string IDENTIFICADOR_SESSAO_USUARIO = "IdentificadorSessaoUsuario";
        public const string IDENTIFICADOR_APLICACAO = "IdentificadorAplicacao";
        //public const string IDENTIFICADOR_PROPRIETARIO = "IdentificadorProprietario";
        private const string CHAVE_CRIPTOGRAFIA = "248c6619-8119-45bd-ae2a-662512aff841";

        private static Dimensao _resolucao;
        private static CredencialUsuario _credencialUsuario;

        private static object _bloqueioIdentificadorSessaoUsuario = new object();
        private static object _bloqueioAcessoArquivoAppData = new object();

        #region Informação da Sessão usuário

        //public static InformacaoSessaoUsuario RetornarInformacaoSessaoUsuarioAtual()
        //{
        //    return AplicacaoSnebur.Atual.InformacaoSessaoUsuarioAtual;
        //}

        public static InformacaoSessaoUsuario RetornarInformacaoSessaoUsuarioAplicacao()
        {

            var tipoAplicacao = AplicacaoSnebur.Atual.TipoAplicacao;
            //var ipInformacao = SessaoUtil.RetornarIpInformacao();
            var userAgent = SessaoUtil.RetornarUserAgent();

            var identificadorSessaoUsuario = AplicacaoSnebur.Atual.IdentificadorSessaoUsuario;
            //var identificadorProprietario = AplicacaoSnebur.Atual.IdentificadorProprietario;
            var identificadorAplicacao = AplicacaoSnebur.Atual.IdentificadorAplicacao;
            var sistemaOperacional = SessaoUtil.RetornarSistemaOperacional();
            var resolucao = SessaoUtil.RetornarResolucao();
            var versaoAplicacao = AplicacaoSnebur.Atual.VersaoAplicao.ToString();
            var nomeComptuador = Environment.MachineName;

            if (identificadorAplicacao == null)
            {
                throw new ArgumentNullException(nameof(identificadorAplicacao));
            }

            return new InformacaoSessaoUsuario
            {
                IdentificadorSessaoUsuario = identificadorSessaoUsuario,
                //IdentificadorProprietario = identificadorProprietario,
                IdentificadorAplicacao = identificadorAplicacao,
                TipoAplicacao = tipoAplicacao,
                //IPInformacao = ipInformacao,
                //IP = ipInformacao.IP,
                UserAgent = userAgent,
                Cultura = Thread.CurrentThread.CurrentCulture.Name,
                Idioma = CultureInfo.InstalledUICulture.Name,
                Navegador = new Navegador(),
                Plataforma = EnumPlataforma.PC,
                SistemaOperacional = sistemaOperacional,
                Resolucao = resolucao,
                VersaoAplicacao = versaoAplicacao,
                NomeComputador = nomeComptuador,
            };
        }

        private static string RetornarUserAgent()
        {
            return AplicacaoSnebur.Atual.UserAgent;
        }

        //public static DadosIPInformacao RetornarIpInformacao()
        //{
        //    if (_dadosIpInformacao == null)
        //    {
        //        throw new NotImplementedException();
        //        //_dadosIpInformacao = IpUtil.RetornarIPInformacao();
        //    }
        //    return _dadosIpInformacao;
        //}

        private static SistemaOperacional RetornarSistemaOperacional()
        {
            return new SistemaOperacional(EnumSistemaOperacional.Windows, "Windows",
                                          Environment.OSVersion.Version.ToString(),
                                          SistemaUtil.RetornarCodinomeSistemaOperacional());
        }

        private static Dimensao RetornarResolucao()
        {
            if (_resolucao == null)
            {
                if (System.Reflection.Assembly.GetEntryAssembly() != null)
                {
                    var nomeTipoSystemParameters = "System.Windows.SystemParameters, PresentationFramework";
                    var tipoSystemaParameters = Type.GetType(nomeTipoSystemParameters);
                    if (tipoSystemaParameters != null)
                    {
                        var propreidadeLargura = tipoSystemaParameters.GetProperty("PrimaryScreenWidth");
                        var propreidadeAltura = tipoSystemaParameters.GetProperty("PrimaryScreenHeight");

                        var largura = ConverterUtil.Converter<int>(propreidadeLargura.GetValue(null));
                        var altura = ConverterUtil.Converter<int>(propreidadeAltura.GetValue(null));

                        _resolucao = new Dimensao((int)largura, (int)altura);
                    }
                }
                _resolucao = new Dimensao(0, 0);
            }
            return _resolucao;
        }
        #endregion

        #region Identificador Sessão usuario

        private static ConcurrentDictionary<string, Guid> IdentificadoresSessaoUsuario { get; set; } = new ConcurrentDictionary<string, Guid>();

        public static Guid RetornarIdentificadorSessaoUsuario()
        {
            if (IdentificadoresSessaoUsuario.ContainsKey(AplicacaoSnebur.Atual.CredencialUsuario.IdentificadorUsuario))
            {
                return SessaoUtil.IdentificadoresSessaoUsuario[AplicacaoSnebur.Atual.CredencialUsuario.IdentificadorUsuario];
            }
            lock (_bloqueioIdentificadorSessaoUsuario)
            {
                if (!IdentificadoresSessaoUsuario.ContainsKey(AplicacaoSnebur.Atual.CredencialUsuario.IdentificadorUsuario))
                {
                    var caminhoArquivo = SessaoUtil.RetornarCaminhoArquivoIdentificadorSessaoUsuario();
                    var identificador = Guid.Empty;
                    if (File.Exists(caminhoArquivo))
                    {
                        identificador = SessaoUtil.RetornarConteudoAppData<Guid>(caminhoArquivo);
                    }
                    if (identificador == Guid.Empty)
                    {
                        identificador = Guid.NewGuid();
                        SessaoUtil.SalvarIdentificadorSessaoUsuario(identificador);
                    }
                    SessaoUtil.IdentificadoresSessaoUsuario.TryAdd(AplicacaoSnebur.Atual.CredencialUsuario.IdentificadorUsuario, identificador);
                }
            }
            return SessaoUtil.RetornarIdentificadorSessaoUsuario();
        }
        private static void SalvarIdentificadorSessaoUsuario(Guid identificador)
        {
            var caminhoArquivo = SessaoUtil.RetornarCaminhoArquivoIdentificadorSessaoUsuario();
            SessaoUtil.SalvarConteudoAppData(identificador, caminhoArquivo);
        }

        private static string RetornarCaminhoArquivoIdentificadorSessaoUsuario()
        {
            var repositorio = ConfiguracaoUtil.CaminhoAppDataAplicacaoSemVersao;
            var nomeArquivo = SessaoUtil.RetornarNomeArquivoIdentificadorSessaoUsuario();
            return Path.Combine(repositorio, $"{nomeArquivo}-sid.dat");
        }

        private static string RetornarNomeArquivoIdentificadorSessaoUsuario()
        {
            var credencialUsuario = AplicacaoSnebur.Atual.CredencialUsuario;
            //if (DebugUtil.IsAttached)
            //{
            return TextoUtil.RetornarSomentesLetrasNumeros(credencialUsuario.IdentificadorUsuario).ToLower();
            //}
            //return Md5Util.RetornarHash(credencialUsuario.IdentificadorUsuario);
        }

        private static string RetornarCaminhoArquivoCredencialUsuario()
        {
            var repositorio = ConfiguracaoUtil.CaminhoAppDataAplicacaoSemVersao;
            return Path.Combine(repositorio, "user.dat");
        }

        public static void InicializarNovaSessaoUsuario()
        {
            SessaoUtil.LimparCredencialUsuario();
            SessaoUtil.LimparIdentificadoresSessaoUsuario();
            SessaoUtil.SalvarIdentificadorSessaoUsuario(Guid.NewGuid());
        }

        public static void LimparIdentificadoresSessaoUsuario()
        {
            lock (_bloqueioAcessoArquivoAppData)
            {
                var repositorio = ConfiguracaoUtil.CaminhoAppDataAplicacaoSemVersao;
                var arquivos = Directory.GetFiles(repositorio, "*.dat").Where(x => Path.GetFileNameWithoutExtension(x).EndsWith("-sid")).ToList();
                foreach (var arquivo in arquivos)
                {
                    ArquivoUtil.DeletarArquivo(arquivo, false, true);
                }
                IdentificadoresSessaoUsuario.Clear();
            }
        }
        #endregion

        #region Credencial Usuario

        public static CredencialUsuario RetornarCredencialUsuario()
        {
            if (_credencialUsuario == null)
            {
                lock (_bloqueioAcessoArquivoAppData)
                {
                    if (_credencialUsuario == null)
                    {
                        _credencialUsuario = RetornarCredencialUsuarioInterno();
                    }
                }
            }
            return _credencialUsuario;
        }

        public static void LimparCredencialUsuario()
        {
            lock (_bloqueioAcessoArquivoAppData)
            {
                var caminhoArquivo = RetornarCaminhoArquivoCredencialUsuario();
                ArquivoUtil.DeletarArquivo(caminhoArquivo);
                _credencialUsuario = null;
            }
        }

        private static CredencialUsuario RetornarCredencialUsuarioInterno()
        {
            lock (_bloqueioAcessoArquivoAppData)
            {
                var caminhoArquivo = RetornarCaminhoArquivoCredencialUsuario();
                if (File.Exists(caminhoArquivo))
                {
                    try
                    {
                        var credencia = SessaoUtil.RetornarConteudoAppData<CredencialUsuario>(caminhoArquivo);
                        if (credencia != null)
                        {
                            return credencia;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtil.ErroAsync(ex);
                        ArquivoUtil.DeletarArquivo(caminhoArquivo);
                        SessaoUtil.InicializarNovaSessaoUsuario();
                    }
                }
                return CredencialAnonimo.Anonimo;
            }
        }

        public static void SalvarCredencialUsuario(CredencialUsuario credencial)
        {
            lock (_bloqueioAcessoArquivoAppData)
            {
                SessaoUtil.LimparCredencialUsuario();
                var caminhoArquivo = RetornarCaminhoArquivoCredencialUsuario();
                SalvarConteudoAppData(credencial, caminhoArquivo);
                AplicacaoSnebur.Atual.NotificarCredencialAlterada();
                _credencialUsuario = null;
            }
        }
        #endregion

        #region Conteudo do AppData

        private static bool IsCriptografarConteudoAppData { get; } = true;

        private static void SalvarConteudoAppData(object obj, string caminhoArquivo)
        {
            lock (_bloqueioAcessoArquivoAppData)
            {
                var conteudo = RetornarConteudoSalvar(obj);
                ArquivoUtil.DeletarArquivo(caminhoArquivo, false, true);
                File.WriteAllText(caminhoArquivo, conteudo, Encoding.UTF8);
            }
        }

        private static string RetornarConteudoSalvar(object obj)
        {
            var conteudo = JsonUtil.Serializar(obj, true);
            if (IsCriptografarConteudoAppData)
            {
                return CriptografiaUtil.Criptografar(CHAVE_CRIPTOGRAFIA, conteudo);
            }
            return conteudo;
        }

        private static T RetornarConteudoAppData<T>(string caminhoArquivo)
        {
            lock (_bloqueioAcessoArquivoAppData)
            {
                if (File.Exists(caminhoArquivo))
                {
                    try
                    {
                        var conteudo = RetornarConteudoLeitura(caminhoArquivo);
                        return JsonUtil.Deserializar<T>(conteudo, true);
                    }
                    catch (Exception ex)
                    {
                        LogUtil.ErroAsync(ex);
                        ArquivoUtil.DeletarArquivo(caminhoArquivo);

                    }
                }
                return default;
            }
        }

        private static string RetornarConteudoLeitura(string caminhoArquivo)
        {
            if (IsCriptografarConteudoAppData)
            {
                var conteudo = File.ReadAllText(caminhoArquivo, Encoding.UTF8);
                return CriptografiaUtil.Descriptografar(CHAVE_CRIPTOGRAFIA, conteudo);
            }
            return File.ReadAllText(caminhoArquivo, Encoding.UTF8);
        }
        #endregion
    }
}