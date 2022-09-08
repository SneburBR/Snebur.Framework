using Snebur.AcessoDados;
using Snebur.Dominio;
using Snebur.Seguranca;
using Snebur.Servicos;
using Snebur.Utilidade;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

namespace Snebur.Comunicacao
{
    public abstract partial class BaseComunicacaoServidor : IHttpHandler, IDisposable, IBaseServico
    {
        private static readonly long TEMPO_LIMITE_PADRAO_LOG_DESEMPEHO = (DebugUtil.IsAttached) ? (long)TimeSpan.FromSeconds(5).TotalMilliseconds : (long)TimeSpan.FromSeconds(2).TotalMilliseconds;

        #region Propriedades

        protected abstract CredencialServico CredencialServico { get; }
        public CredencialUsuario CredencialUsuario { get; private set; }
        public Guid IdentificadorSessaoUsuario { get; private set; }
        public InformacaoSessaoUsuario InformacaoSessaoUsuario { get; private set; }
        public string IdentificadorProprietario { get; protected internal set; }
        public bool IsBloqueiarThreadSessaoUsuario { get; protected internal set; }

        #endregion

        #region Construtor

        public BaseComunicacaoServidor()
        {

        }

        #endregion

        #region  IHttpHandler - Construtor 

        public void ProcessRequest(HttpContext httpContext)
        {
            var response = httpContext.Response;
            try
            {
                this.AntesProcessRequest(httpContext);

                //Sem cache
                response.CacheControl = "no-cache";
                response.AddHeader("Pragma", "no-cache");

                response.Cache.SetNoStore();
                response.Cache.SetCacheability(HttpCacheability.NoCache);
                response.Cache.SetExpires(DateTime.Now.AddYears(-1));

                using (var requisicao = new Requisicao(httpContext, this.CredencialServico, this.GetType().Name))
                {
                    if (requisicao.CredencialServicoValida())
                    {
                        this.InformacaoSessaoUsuario = requisicao.InformacaoSessaoUsuario;
                        this.CredencialUsuario = requisicao.CredencialUsuario;
                        this.IdentificadorSessaoUsuario = requisicao.InformacaoSessaoUsuario.IdentificadorSessaoUsuario;
                        this.Inicializar(requisicao);

                        try
                        {
                            var resultadoSerializado = this.RetornarResultadoChamadaSerializado(requisicao, httpContext);

                            if (!requisicao.ContratoChamada.Async && requisicao.SerializarJavascript)
                            {
                                httpContext.Response.ContentType = "text/json";
                                httpContext.Response.Charset = "utf8";
                                httpContext.Response.ContentEncoding = Encoding.UTF8;
                                httpContext.Response.Write(resultadoSerializado);
                            }
                            else
                            {
                                var conteudo = PacoteUtil.CompactarPacote(resultadoSerializado);
                                httpContext.Response.BinaryWrite(conteudo);
                            }
                            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                        }

                        catch (Exception ex)
                        {
                            throw new ErroRequisicao(ex, requisicao);
                        }
                    }
                    else
                    {
                        var mensagemSeguranca = String.Format("A credencial do serviço não autorizada '{0}' '{1}' - '{2}' ", this.GetType().Name, this.CredencialServico.IdentificadorUsuario, this.CredencialServico.Senha);
                        if (DebugUtil.IsAttached)
                        {
                            throw new Exception(mensagemSeguranca);
                        }

                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        response.SubStatusCode = Convert.ToInt32(EnumTipoLogSeguranca.CredencialServicoNaoAutorizada);

                        LogUtil.SegurancaAsync(mensagemSeguranca, EnumTipoLogSeguranca.CredencialServicoNaoAutorizada);
                    }
                }
            }
            catch (ErroDeserializarContrato)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.SubStatusCode = Convert.ToInt32(EnumTipoLogSeguranca.ContratoInvalido);

                var mensagemSeguranca = String.Format("O contrato da chamada é invalido '{0}' '{1}' - '{2}' ", this.GetType().Name, this.CredencialServico.IdentificadorUsuario, this.CredencialServico.Senha);
                LogUtil.SegurancaAsync(mensagemSeguranca, EnumTipoLogSeguranca.ContratoInvalido);
            }
            catch (ErroMetodoOperacaoNaoFoiEncontrado erro)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.SubStatusCode = Convert.ToInt32(EnumTipoLogSeguranca.MetodoOperacaoNaoEncontrado);

                var mensagemSeguranca = String.Format("O método '{0}' não foi encontrado no serviço '{1}'", erro.NomeMetodo, this.GetType().Name);
                LogUtil.SegurancaAsync(mensagemSeguranca, EnumTipoLogSeguranca.MetodoOperacaoNaoEncontrado);
            }
            catch (ErroRequisicao ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string RetornarResultadoChamadaSerializado(Requisicao requisicao, HttpContext httpContext)
        {
            if (this.IsManterCache && !this.OperacoesIgnorarCaches.Contains(requisicao.Operacao))
            {
                return this.RetornarResultadoChamadaSerializadoCache(requisicao, httpContext);
            }
            return this.RetornarResultadoChamadaSerializadoInterno(requisicao, httpContext);

        }

        private string RetornarResultadoChamadaSerializadoCache(Requisicao requisicao, HttpContext httpContext)
        {
            lock (this.RetornarObjetoBloqueioThreadCache(requisicao))
            {
                var resultadoEmCache = this.RetornarConteudoCache(requisicao);
                if (resultadoEmCache != null)
                {
                    return resultadoEmCache;
                }

                var resultado = this.RetornarResultadoChamadaSerializadoInterno(requisicao, httpContext);
                this.SalvarChace(requisicao, resultado);
                return resultado;
            }
        }

        private string RetornarResultadoChamadaSerializadoInterno(Requisicao requisicao, HttpContext httpContext)
        {
            var metodoOperacao = this.RetornarMetodo(requisicao.Operacao);
            var parametros = this.RetornarParametrosMetodoOperacao(metodoOperacao, requisicao.Parametros);
            parametros = this.NormalizarParametros(parametros);

            var resultadoChamada = this.RetornarResultadoChamada(requisicao, httpContext, parametros);
            resultadoChamada.DataHora = DateTime.UtcNow;
            resultadoChamada.NomeServico = this.GetType().Name;

            return JsonUtil.Serializar(resultadoChamada, requisicao.SerializarJavascript);
        }

        protected virtual object RetornarResultadoOperacao(Requisicao requisicao,
                                                           MethodInfo metodoOperacao,
                                                           object[] parametros)
        {
            if (!this.IsBloqueiarThreadSessaoUsuario)
            {
                return metodoOperacao.Invoke(this, parametros);
            }

            var objetoBloqueio = this.RetornarObjetoBloqueioThread();
            lock (objetoBloqueio)
            {
                return metodoOperacao.Invoke(this, parametros);
            }
        }

        protected virtual void Inicializar(Requisicao requisicao)
        {
        }

        protected virtual long RetonarTempoLimiteOperacao(Requisicao requisicao)
        {
            return BaseComunicacaoServidor.TEMPO_LIMITE_PADRAO_LOG_DESEMPEHO;
        }

        private void NotificarLogLentidaoAsync(Requisicao requisicao, Stopwatch tempo)
        {
            if (!DebugUtil.IsAttached)
            {
                var mensagem = this.RetornarMensagemLogLentidao(requisicao.Operacao, tempo);
                LogUtil.DesempenhoAsync(mensagem, tempo, Servicos.EnumTipoLogDesempenho.LentidaoServicoComunicacao, false,
                (Guid identificador) =>
                {
                    var json = JsonUtil.Serializar(requisicao, true);
                    var nomeArquivo = identificador.ToString() + ".json";
                    var caminho = Path.Combine(ConfiguracaoUtil.CaminhoAppDataAplicacaoLogs, "Desempenho", nomeArquivo);
                    ArquivoUtil.SalvarArquivoTexto(caminho, json, Encoding.UTF8);
                });
            }
        }

        protected virtual string RetornarMensagemLogLentidao(string operacao, Stopwatch tempo)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Lentidão");
            sb.AppendLine($"Serviço: {this.GetType().Name}");
            sb.AppendLine($"Operação: {operacao} ");
            sb.AppendLine($"Tempo: {tempo.ElapsedMilliseconds}");
            return sb.ToString();
        }

        protected virtual object[] RetornarParametrosMetodoOperacao(MethodInfo operacao,
                                                                    Dictionary<string, object> parametros)
        {
            var parametrosOperacao = operacao.GetParameters().ToList();
            if (parametrosOperacao.Count != parametros.Count)
            {
                var mensagem = $"O numero de parâmetros da chamada do método operação {operacao.Name} é diferente do serviço {this.GetType().Name} ";
                throw new ErroSeguranca(mensagem, Servicos.EnumTipoLogSeguranca.ParametrosComunicacaoInvalidos);
            }

            var retorno = new List<object>();

            for (var i = 0; i <= parametrosOperacao.Count - 1; i++)
            {
                var parametroMetodo = parametrosOperacao[i];
                var nomeParametroOperacao = parametrosOperacao[i].Name;


                if (!parametros.ContainsKey(nomeParametroOperacao))
                {
                    var mensagem = $"O nome do parâmetro da chamada do método da operação {nomeParametroOperacao} não foi encontrado no dicionario dos parâmetros";
                    throw new ErroSeguranca(mensagem, EnumTipoLogSeguranca.ParametrosComunicacaoInvalidos);
                }

                var valorParametro = parametros[nomeParametroOperacao];

                if (valorParametro == null || (valorParametro.GetType().IsSubclassOf(typeof(BaseDominio)) ||
                                              Object.ReferenceEquals(valorParametro.GetType(), parametroMetodo.ParameterType)))
                {
                    retorno.Add(valorParametro);
                }
                else if (parametroMetodo.ParameterType.IsEnum)
                {
                    retorno.Add(Convert.ToInt32(valorParametro));
                }
                else if (ReflexaoUtil.TipoRetornaColecao(parametroMetodo.ParameterType))
                {
                    if ((!Object.ReferenceEquals(parametroMetodo.ParameterType.GetGenericTypeDefinition(), typeof(List<>))))
                    {
                        throw new ErroSeguranca("Suportado somente tipo List(Of T ) T = sub classe de BaseDominio  ", EnumTipoLogSeguranca.ParametrosComunicacaoInvalidos);
                    }

                    if (!ReflexaoUtil.TipoRetornaColecao(valorParametro.GetType()))
                    {
                        throw new ErroSeguranca("A valor do parâmetro não retorna lista como esperado no tipo do parâmetro do método", EnumTipoLogSeguranca.ParametrosComunicacaoInvalidos);
                    }


                    var tipoItemColecao = parametroMetodo.ParameterType.GetGenericArguments().Single();

                    if (!(tipoItemColecao.IsSubclassOf(typeof(BaseDominio)) || ReflexaoUtil.TipoRetornaTipoPrimario(tipoItemColecao)))
                    {
                        throw new ErroSeguranca("O tipo não é suportado  " + tipoItemColecao.Name, EnumTipoLogSeguranca.ParametrosComunicacaoInvalidos);
                    }

                    var tipoLista = typeof(List<>);
                    var tipoListaTipado = tipoLista.MakeGenericType(tipoItemColecao);

                    var lista = (IList)Activator.CreateInstance(tipoListaTipado);
                    foreach (var item in (IList)valorParametro)
                    {
                        lista.Add(ConverterUtil.Converter(item, tipoItemColecao));
                    }
                    retorno.Add(lista);

                }
                else
                {
                    try
                    {
                        var valorConvertido = ConverterUtil.Converter(valorParametro, parametroMetodo.ParameterType);
                        //var valorConvertido = Convert.ChangeType(valorParametro, parametroMetodo.ParameterType, System.Globalization.CultureInfo.InvariantCulture);
                        retorno.Add(valorConvertido);
                    }
                    catch (Exception ex)
                    {
                        var mensagem = $"O erro no converter o tipo do Parâmetro na chamada do método operação {operacao.Name} é diferente do serviço {this.GetType().Name},  Nome do parâmetro '{nomeParametroOperacao}' ";
                        throw new ErroComunicacao(mensagem, ex);
                    }
                }
            }
            return retorno.ToArray();
        }

        protected virtual object[] NormalizarParametros(object[] parametros)
        {
            return parametros;
        }

        protected virtual object NormalizarResultadoOperacao(object resultadoOperacao)
        {
            return resultadoOperacao;
        }

        protected ResultadoChamada RetornarResultadoChamada(Requisicao requisicao,
                                                            HttpContext httpContext,
                                                            object[] parametros)
        {
            var metodoOperacao = this.RetornarMetodo(requisicao.Operacao);
            var tempo = Stopwatch.StartNew();
            try
            {
                var resultadoOperacao = this.RetornarResultadoOperacao(requisicao, metodoOperacao, parametros);
                resultadoOperacao = this.NormalizarResultadoOperacao(resultadoOperacao);
                tempo.Stop();

                if (tempo.ElapsedMilliseconds > this.RetonarTempoLimiteOperacao(requisicao))
                {
                    this.NotificarLogLentidaoAsync(requisicao, tempo);
                }

                var resultadoChamada = this.RetornarResultadoChamadaInterno(resultadoOperacao);
                resultadoChamada.TempoOperacao = (int)tempo.ElapsedMilliseconds;
                return resultadoChamada;
            }
            catch (Exception ex)
            {

                if (ex is ErroSessaoUsuarioExpirada erroSessaoUsuarioExpirada)
                {
                    return new ResultadoSessaoUsuarioInvalida(erroSessaoUsuarioExpirada.EstadoSessao, this.IdentificadorSessaoUsuario);
                }
                var erroInterno = ex.InnerException;
                if (erroInterno is ErroSessaoUsuarioExpirada erroInternoTipado)
                {
                    return new ResultadoSessaoUsuarioInvalida(erroInternoTipado.EstadoSessao, this.IdentificadorSessaoUsuario);
                }

                var host = httpContext.Request.Url.Host;
                if (host.EndsWith(ConstantesDominioSuperior.DOMIMIO_SUPERIOR_LOCALHOST) ||
                    host.EndsWith(ConstantesDominioSuperior.DOMIMIO_SUPERIOR_INTERNO))
                {
                    var mensagemErro = ErroUtil.RetornarDescricaoCompletaErro(ex,
                        metodoOperacao.Name, "", 0);

                    return new ResultadoChamadaErroInternoServidor
                    {
                        MensagemErro = mensagemErro,
                        TempoOperacao = (int)tempo.ElapsedMilliseconds
                    };
                }
                throw ex;
            }
        }

        private ResultadoChamada RetornarResultadoChamadaInterno(object resultado)
        {
            if (resultado == null)
            {
                return new ResultadoChamadaVazio();
            }

            var tipo = resultado.GetType();
            if (ReflexaoUtil.TipoRetornaTipoPrimario(tipo))
            {
                return new ResultadoChamadaTipoPrimario
                {
                    TipoPrimarioEnum = ReflexaoUtil.RetornarTipoPrimarioEnum(tipo),
                    Valor = resultado
                };
            }

            if (tipo.IsEnum)
            {
                return new ResultadoChamadaEnum
                {
                    NamespaceEnum = tipo.Namespace,
                    NomeTipoEnum = tipo.Name,
                    Valor = Convert.ToInt32(resultado)
                };
            }

            if (tipo.IsSubclassOf(typeof(BaseDominio)))
            {
                return new ResultadoChamadaBaseDominio
                {
                    BaseDominio = (BaseDominio)resultado,
                };
            }

            if (ReflexaoUtil.TipoRetornaColecao(tipo) && tipo.IsGenericType && tipo.GetGenericArguments().Count() == 1)
            {
                var tipoItem = tipo.GetGenericArguments().Single();

                var resultadoLista = this.RetornarResultadoChamadaLista((ICollection)resultado, tipoItem);
                resultadoLista.AssemblyQualifiedName = tipoItem.AssemblyQualifiedName;
                return resultadoLista;
            }

            throw new ErroNaoSuportado(String.Format("O tipo do resultado da operação não é suportado {0} ", tipo.Name));

        }

        private ResultadoChamadaLista RetornarResultadoChamadaLista(ICollection valores, Type tipoItem)
        {
            if (ReflexaoUtil.TipoRetornaTipoPrimario(tipoItem))
            {
                return new ResultadoChamadaListaTipoPrimario
                {
                    TipoPrimarioEnum = ReflexaoUtil.RetornarTipoPrimarioEnum(tipoItem),
                    Valores = valores.Cast<object>().ToList()
                };
            }

            if (tipoItem.IsEnum)
            {
                return new ResultadoChamadaListaEnum
                {
                    NamespaceEnum = tipoItem.Namespace,
                    NomeTipoEnum = tipoItem.Name,
                    Valores = valores.Cast<int>().ToList()
                };
            }

            if (tipoItem.IsSubclassOf(typeof(BaseDominio)))
            {
                return new ResultadoChamadaListaBaseDominio
                {
                    NomeTipoBaseDominio = tipoItem.Name,
                    NomeNamespaceTipoBaseDominio = tipoItem.Name,
                    BasesDominio = valores.Cast<BaseDominio>().ToList()
                };
            }

            throw new ErroNaoSuportado(String.Format("O tipo item da lista coleção do resultado da operação não é suportado {0} ", tipoItem.Name));
        }

        public bool IsReusable
        {
            get { return false; }
        }



        protected virtual void AntesProcessRequest(HttpContext context)
        {
        }

        #endregion

        #region Métodos privados

        private MethodInfo RetornarMetodo(string nomeMetodo)
        {

            var metodos = this.GetType().GetMethods().Where(x => x.Name.Equals(nomeMetodo, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (metodos.Count == 0)
            {
                throw new ErroMetodoOperacaoNaoFoiEncontrado(nomeMetodo, String.Format("Não existe o método {0} implementado no serviço {1}", nomeMetodo, this.GetType().Name));
            }

            if (metodos.Count > 1)
            {
                throw new ErroMetodoOperacaoNaoFoiEncontrado(nomeMetodo, String.Format("Existe mais de um método {0} implementado no serviço {1}", nomeMetodo, this.GetType().Name));
            }
            return metodos.Single();
        }

        private object RetornarObjetoBloqueioThreadCache(Requisicao requisicao)
        {
            var chaveRequisica = this.RetornarChaveCache(requisicao);
            return ThreadUtil.RetornarBloqueio(chaveRequisica);
        }

        protected virtual object RetornarObjetoBloqueioThread()
        {

            throw new Erro($" esse método deve ser sobre escrito quando {nameof(this.IsBloqueiarThreadSessaoUsuario)} for true .");
        }

        #endregion

        #region IBaseServico

        public DateTime RetornarDataHoraUTC()
        {
            return DateTime.Now.ToUniversalTime();
        }

        public DateTime RetornarDataHora()
        {
            return DateTime.Now;
        }

        public bool Ping()
        {
            return true;
        }

        public virtual void Dispose()
        {
        }

        #endregion

    }
}