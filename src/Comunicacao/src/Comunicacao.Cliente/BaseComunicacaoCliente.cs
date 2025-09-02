using Snebur.AcessoDados;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Snebur.Comunicacao
{
    public abstract partial class BaseComunicacaoCliente : IBaseServico
    {

        private const string PARALAVRA_ASYNC = "Async";

        #region Propriedades

        private string UrlServico { get; }

        #endregion

        #region Credenciais

        protected abstract CredencialServico CredencialServico { get; }

        protected virtual CredencialUsuario CredencialUsuario => AplicacaoSnebur.Atual.CredencialUsuario;

        protected virtual CredencialUsuario CredencialAvalista { get; }

        #endregion

        #region Construtor

        public BaseComunicacaoCliente(string urlServico)
        {
            if (String.IsNullOrEmpty(urlServico))
            {
                throw new ErroComunicacao($"A url do serviço '{this.GetType().Name}' não foi definida.");
            }

            this.UrlServico = AmbienteServidorUtil.NormalizarUrl(urlServico);
        }

        #endregion

        #region Chamadas

        protected virtual Dictionary<string, string> ParametrosCabecalhoAdicionais
        {
            get
            {
                return null;
            }
        }

        protected T ChamarServico<T>(MethodBase metodo, object[] valoresParametro)
        {
            return this.ChamarServico<T>(metodo, metodo, valoresParametro);
        }

        protected T ChamarServico<T>(MethodBase metodo, MethodBase metodoParametros, object[] valoresParametro)
        {

            if (AplicacaoSnebur.Atual.IsMainThread)
            {
                throw new ErroComunicacao("As chamadas síncronas não podem ser executadas na MainThread");
            }

            var tipoRetorno = typeof(T);
            var contrato = this.RetornarContratoChamada(metodo, metodoParametros, false, valoresParametro);

            var chamdaServico = new ChamadaServico(this.RetornarNomeManipulador(), contrato, this.UrlServico, tipoRetorno, this.ParametrosCabecalhoAdicionais);
            var resultado = chamdaServico.ExecutarChamada();

            if (resultado is ResultadoSessaoUsuarioInvalida resultadoSessaoUsuarioInvalida)
            {
                AplicacaoSnebur.Atual.IniciarNovaSessaoAnonima();

                throw new ErroSessaoUsuarioExpirada(resultadoSessaoUsuarioInvalida.StatusSessaoUsuario,
                                                    resultadoSessaoUsuarioInvalida.IdentificadorSessaoUsuario,
                                                    "Sessão do usuário foi finalizada");
            }
            return (T)resultado;
        }

        protected void ChamarServicoAsync<T>(MethodBase metodo, Action<T> callback, params object[] valoresParametro)
        {
            this.ChamarServicoAsync<T>(metodo, metodo, callback, valoresParametro);
        }

        protected void ChamarServicoAsync<T>(MethodBase metodo, MethodBase metodoParametros, Action<T> callback, params object[] valoresParametro)
        {
            var tipoRetorno = typeof(T);
            var chamadaAsync = new ChamadaServicoAsync(this.RetornarNomeManipulador(), this.RetornarContratoChamada(metodo, metodoParametros, true, valoresParametro), this.UrlServico, tipoRetorno, this.ParametrosCabecalhoAdicionais);
            chamadaAsync.ExecutarChamaraAsync(args =>
            {
                if (args.Error != null)
                {
                    throw new Exception("Erro na chamada async do servico", args.Error);
                }
                callback.Invoke((T)args.Resultado);
            });
        }

        protected virtual string RetornarNomeManipulador()
        {
            var nome = this.GetType().Name;
            if (nome.EndsWith("Cliente"))
            {
                nome = nome.Replace("Cliente", String.Empty);
            }
            return nome;
        }

        #endregion

        #region Contrato 

        public string RetornarContratoSerializado(string nomeMetodo, params object[] parametros)
        {
            var metodo = this.GetType().GetMethods().Where(x => x.Name == nomeMetodo).SingleOrDefault();
            return this.RetornarContratoSerializado(metodo, parametros);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string RetornarContratoSerializado(MethodBase metodo, params object[] parametros)
        {
            var contrato = this.RetornarContratoChamada(metodo, metodo, false, parametros);
            return JsonUtil.Serializar(contrato, EnumTipoSerializacao.DotNet, null, true);
        }

        #endregion

        #region Métodos protegidos - virtual

        protected virtual InformacaoSessao RetornarInformacaoSessoUsuarioRequisicaoAtual()
        {
            return AplicacaoSnebur.Atual.InformacaoSessao;
        }

        protected virtual Guid RetornarIdentificadorSessaoUsuario()
        {
            return AplicacaoSnebur.Atual.IdentificadorSessaoUsuario;
        }

        protected virtual string IdentificadorProprietarioRequisicaoAtual()
        {
            return AplicacaoSnebur.Atual.IdentificadorProprietario;
        }

        //protected virtual InformacaoSessaoUsuario RetornarInformacaoSessaoUsuario()
        //{
        //    return SessaoUsuarioUtil.RetornarInformacaoSessaoUsuarioAtual();
        //}

        #endregion

        #region Métodos privados

        protected ContratoChamada RetornarContratoChamada(MethodBase metodoChamada,
                                                          MethodBase metodoParametros, bool isAsync, params object[] valoresParametro)
        {
            var informacaoSessaoUsuario = this.RetornarInformacaoSessoUsuarioRequisicaoAtual();
            informacaoSessaoUsuario.TipoAplicacao = AplicacaoSnebur.Atual.TipoAplicacao;

            var identificadorPropriedade = this.IdentificadorProprietarioRequisicaoAtual();
            var identificadorSessaoUsuario = this.RetornarIdentificadorSessaoUsuario();

            var nomeMetodo = this.RetornarNomeMetodo(metodoChamada, isAsync);
            var operacao = nomeMetodo;

            var cabecalho = new Cabecalho
            {
                IdentificadorProprietario = identificadorPropriedade,
                CredencialServico = this.CredencialServico,
                CredencialUsuario = this.CredencialUsuario,
                CredencialAvalista = this.CredencialAvalista
            };

            var contrato = new ContratoChamada
            {
                Async = isAsync,
                Cabecalho = cabecalho,
                InformacaoSessao = informacaoSessaoUsuario,
                IdentificadorSessaoUsuario = identificadorSessaoUsuario,
                Operacao = operacao,
                DataHora = DateTime.UtcNow,
            };

            var parametrosChamada = this.RetornarParametrosChamada(metodoChamada, metodoParametros, isAsync, valoresParametro);
            contrato.Parametros.AddRange(parametrosChamada);
            return contrato;
        }

        private string RetornarNomeMetodo(MethodBase metodo, bool isAsync)
        {
            var nomeMetodo = metodo.Name;
            if (isAsync)
            {
                if (!nomeMetodo.EndsWith(PARALAVRA_ASYNC))
                {
                    throw new Exception($"O nome do método para um chamada assincrona deve terminar com {PARALAVRA_ASYNC}");
                }
                return nomeMetodo.Substring(0, nomeMetodo.Length - PARALAVRA_ASYNC.Length);
            }
            return nomeMetodo;
        }

        protected virtual List<ParametroChamada> RetornarParametrosChamada(MethodBase metodoChamada,
                                                                           MethodBase metodoParametros,
                                                                           bool isAsync, object[] valoresParametro)
        {

            var parametros = metodoParametros.GetParameters().ToList();
            if (isAsync)
            {
                parametros.Remove(parametros.Last());
            }

            if (valoresParametro.Count() != parametros.Count)
            {
                throw new Exception("O numero parametros é diferente do método");
            }

            var parametrosChamada = new List<ParametroChamada>();
            var totalParametros = parametros.Count;
            for (int i = 0; i <= (totalParametros - 1); i++)
            {
                var valorPametro = valoresParametro[i];
                var parametro = parametros[i];

                if (valorPametro != null &&
                    !ReflexaoUtil.IsTipoIgualOuHerda(valorPametro.GetType(), parametro.ParameterType))
                {
                    throw new Exception($"O tipo do parâmetro {parametro.Name} '{parametro.ParameterType.Name}' é diferente do valor '{valorPametro.GetType().Name}'");
                }
                parametrosChamada.Add(this.RetornarParametroChamada(parametro, valorPametro));
            }
            return parametrosChamada;
        }

        #endregion

        #region IBaseServico

        public bool Ping()
        {
            object[] parametros = { };
            return this.ChamarServico<bool>(MethodBase.GetCurrentMethod(), parametros);
        }

        public DateTime RetornarDataHoraUTC()
        {
            object[] parametros = { };
            return this.ChamarServico<DateTime>(MethodBase.GetCurrentMethod(), parametros);
        }

        #endregion

    }
}
