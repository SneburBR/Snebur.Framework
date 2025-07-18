using Snebur.Seguranca;
using Snebur.Servicos;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Http;
#endif

namespace Snebur.Comunicacao
{

    public abstract partial class BaseManipuladorRequisicao
    {
        private const int TEMPO_EXPIRAR_TOKEN = Token.TEMPO_EXPIRAR_TOKEN_PADRAO;

        private Dictionary<string, Type> _tiposManipuladore;
        private Dictionary<string, Type> Manipuladores { get; } = new Dictionary<string, Type>();
        private Dictionary<string, (Type tipo, bool isValidarToken)> ManipuladoresGenericos { get; } = new Dictionary<string, (Type, bool)>();
        private Dictionary<string, bool> ArquivosAutorizados { get; } = new Dictionary<string, bool>();
        private List<string> DiretoriosImagemAutorizado { get; } = new List<string>();

        private Dictionary<string, Type> TiposManipuladores => LazyUtil.RetornarValorLazyComBloqueio(ref _tiposManipuladore, this.RetornarTiposManipuladores);
        protected bool IsAutoRegistrarManipulador { get; set; } = true;
        private readonly object _bloqueio = new object();

        public string CaminhoAplicacao { get; }

#if NET5_0_OR_GREATER

        public BaseManipuladorRequisicao(string caminhoAplicacao)
        {
            this.CaminhoAplicacao = caminhoAplicacao;
            this.AutorizarArquivo("favicon.ico", true);
            this.InicializarManipuladores();
        }

#else
        public BaseManipuladorRequisicao()
        {
            this.AutorizarArquivo("favicon.ico", true);
            this.InicializarManipuladores();
        }
#endif

        public abstract void InicializarManipuladores();

        #region Métodos privados

        private bool IsRequicaoValida(HttpRequest request, bool isValidarUrlMd5)
        {
            if (DebugUtil.IsAttached)
            {
                return true;
            }

            if (!this.IsAplicacaoAutorizada(request))
            {
                this.NotificarLogSeguranca(request, EnumTipoLogSeguranca.AplicacaoNaoAutorizada);
                return false;
            }

            var identificadorProprietario = this.RetornarIdentificadorProprietario(request);
            if (String.IsNullOrWhiteSpace(identificadorProprietario))
            {
                this.NotificarLogSeguranca(request, EnumTipoLogSeguranca.IdentificadorProprietarioInvalido);
                return false;
            }
            var token = HttpUtility.UrlDecode(request.Headers[ParametrosComunicacao.TOKEN]);
            return this.ValidarToken(request, token, isValidarUrlMd5);
        }

        private void NotificarLogSeguranca(HttpRequest request,
                                          EnumTipoLogSeguranca tipoLogSeguranca)
        {
            var host = request.RetornarUrlRequisicao()?.Host?.ToLower() ?? " host não definida";
            var url = request?.RetornarUrlRequisicao()?.AbsoluteUri ?? " url não definida";
            var identificadorPropriedadetarioNoCabecalho = request.Headers.GetValue(ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO) ??
                                                                                 $"{ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO} não definido no cabeçalho ";

            var origem = request.Headers.GetValue(ConstantesCabecalho.ORIGIN) ?? "Origem não definida";
            var ambienteIIS = ConfiguracaoUtil.AmbienteServidor.ToString();
            var nomeAssembly = request.Headers.GetValue(ConstantesCabecalho.NOME_ASSEMBLY_APLICACAO);
            var identificadorAplicacao = request.Headers.GetValue(ParametrosComunicacao.IDENTIFICADOR_APLICACAO);

            var mensagem = $"Host: '{host}'\r\n" +
                           $"Url: '{url}' \r\n" +
                           $"Cabeçalho {ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO}: '{identificadorPropriedadetarioNoCabecalho}'\r\n" +
                           $"Cabeçalho {ConstantesCabecalho.ORIGIN}: '{origem}'\r\n" +
                           $"Cabeçalho {ConstantesCabecalho.NOME_ASSEMBLY_APLICACAO}: '{nomeAssembly}'\r\n" +
                           $"Cabeçalho {ParametrosComunicacao.IDENTIFICADOR_APLICACAO}: '{identificadorAplicacao}'\r\n" +
                           $"Ambiente IIS : {ambienteIIS}\r\n";

            LogUtil.SegurancaAsync(mensagem, tipoLogSeguranca);
        }

        private bool ValidarToken(HttpRequest request, string token, bool isValidarUrlMd5)
        {
            var resultado = Token.ValidarToken(token, TimeSpan.FromSeconds(TEMPO_EXPIRAR_TOKEN));
            if (resultado.Status != EnumStatusToken.Valido)
            {
                var tipoLogSeguranca = resultado.RetornarTipoLogReguranca();
                var mensagem = String.Format("Token : {0} - DataHora requisição : {1}, DataHora Token {2}", token, DateTime.UtcNow, resultado.DataHora);
                LogUtil.SegurancaAsync(mensagem, tipoLogSeguranca);
                return false;
            }
            return true;
        }

        private Type RetornarTipoServico(string nomeServico)
        {
            if (!this.Manipuladores.ContainsKey(nomeServico))
            {
                this.TentarRegistrarManipulador(nomeServico);
                if (!this.Manipuladores.ContainsKey(nomeServico))
                {
                    if (DebugUtil.IsAttached)
                    {
                        throw new ErroManipualdorNaoEncontrado(String.Format("O manipulador {0} não foi encontrado, deve ser inicializado no construtor do manipulador do WebService", nomeServico));
                    }
                    return null;
                }
            }
            return this.Manipuladores[nomeServico];
        }

        public void NotificarServicoNaoEncontado(HttpContext httpContext,
                                                 string nomeServico)
        {
            LogUtil.SegurancaAsync(nomeServico, EnumTipoLogSeguranca.ServicoNaoEncontrado);
            httpContext.Response.StatusCode = 404;
        }

        private void TentarRegistrarManipulador(string nomeServico)
        {
            if (this.IsAutoRegistrarManipulador)
            {
                lock (this._bloqueio)
                {
                    if (!this.Manipuladores.ContainsKey(nomeServico))
                    {
                        var tipo = this.TiposManipuladores.GetValueOrDefault(nomeServico);
                        if (tipo != null)
                        {
                            this.Manipuladores.Add(nomeServico, tipo);
                        }
                    }
                }
            }
        }

        #endregion

        #region Métodos compartilhados

        protected void AutorizarArquivo(string nomeArquivo,
                                        bool isIgnorarValidacaoTokenAplicacao = false)
        {
            this.ArquivosAutorizados.Add(nomeArquivo.ToLower(), isIgnorarValidacaoTokenAplicacao);
        }

        protected void AutorizarDiretorio(string nomeDiretorio)
        {
            nomeDiretorio = UriUtil.AjustarBarraInicialFinal(nomeDiretorio.ToLower());
            this.DiretoriosImagemAutorizado.Add(nomeDiretorio);
        }

        protected void AdicionarServico<T>(string nomeServico) where T : BaseComunicacaoServidor
        {
            var tipo = typeof(T);
            this.AdicionarServico(nomeServico, tipo);
        }

        protected void AdicionarServico<T>() where T : BaseComunicacaoServidor
        {
            var tipo = typeof(T);
            this.AdicionarServico(tipo.Name, tipo);
        }
        protected void AdicionarServico(string nome, Type tipo)
        {
            if (this.Manipuladores.ContainsKey(nome))
            {
                throw new Exception(String.Format("Já foi adicionado um manipulador para {0}", nome));
            }

            if (!this.Manipuladores.ContainsKey(nome))
            {
                if (!tipo.IsSubclassOf(typeof(BaseComunicacaoServidor)))
                {
                    throw new Exception(String.Format("O tipo não é suportado, ele deve herda de {0}", typeof(BaseComunicacaoServidor).Name));
                }
                this.Manipuladores.Add(nome, tipo);
            }
        }

        protected void AdicionarManipuladorGenerico<T>(bool isValidarToken) where T : IHttpHandler
        {
            var tipo = typeof(T);
            this.AdicionarManipuladorGenerico<T>(tipo.Name.ToLower(), isValidarToken);
        }

        protected void AdicionarManipuladorGenerico<T>(string nomeManipulador, bool isValidarToken) where T : IHttpHandler
        {
            var tipo = typeof(T);
            var chave = nomeManipulador.ToLower();
            if (this.ManipuladoresGenericos.ContainsKey(chave))
            {
                throw new Exception($"Já foi adicionado um manipulador para {chave}");
            }
            this.ManipuladoresGenericos.Add(chave, (tipo, isValidarToken));
        }

        #endregion

        #region Métodos virtuais

        public virtual void AntesProcessarRequisicao(HttpContext context)
        {

        }

        public virtual void DepoisProcessarRequisicao(HttpContext context)
        {

        }

        #endregion

        #region Métodos abstratos

        protected abstract string RetornarIdentificadorProprietario(HttpRequest httpRequest);

        protected abstract bool IsAplicacaoAutorizada(HttpRequest request);

        #endregion

        #region Métodos privados
        private Dictionary<string, Type> RetornarTiposManipuladores()
        {
            var retorno = new Dictionary<string, Type>();
            var tiposServicos = new List<Type>();
            var tipoServico = typeof(BaseComunicacaoServidor);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var tipo in assembly.GetLoadableTypes())
                {
                    if (tipo.IsSubclassOf(tipoServico))
                    {
                        tiposServicos.Add(tipo);
                    }
                }
            }
            var tipos = tiposServicos.GroupBy(x => x.Name).Where(x => x.Count() == 1).Select(x => x.Single());
            return tipos.ToDictionary(x => x.Name);
        }
        #endregion

        public void Dispose()
        {
        }

    }

}
