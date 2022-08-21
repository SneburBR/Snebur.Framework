using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using Snebur.AcessoDados;
using Snebur.Dominio;
using Snebur.Seguranca;
using Snebur.Servicos;
using Snebur.Utilidade;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

namespace Snebur.Comunicacao
{
    public abstract partial class BaseComunicacaoServidor : IDisposable, IBaseServico
    {
        private static readonly long TEMPO_LIMITE_PADRAO_LOG_DESEMPEHO = (Debugger.IsAttached) ? (long)TimeSpan.FromSeconds(5).TotalMilliseconds : (long)TimeSpan.FromSeconds(2).TotalMilliseconds;

        #region Propriedades

        protected abstract CredencialServico CredencialServico { get; }
        public CredencialUsuario CredencialUsuario { get; private set; }
        public Guid IdentificadorSessaoUsuario { get; private set; }
        public InformacaoSessaoUsuario InformacaoSessaoUsuario { get; private set; }
        public string IdentificadorProprietario { get; protected internal set; }
        public bool IsBloqueiarThreadSessaoUsuario { get; protected internal set; } = false;
        public bool IsTratarErro { get; protected internal set; } = false;

        #endregion

        #region Construtor

        public BaseComunicacaoServidor()
        {

        }

        #endregion

        #region  IHttpHandler - Construtor 

        public async Task ProcessRequestAsyc(HttpContext httpContext)
        {
            var response = httpContext.Response;
            try
            {
                this.AntesProcessRequest(httpContext);

                //Sem cache
                //response.CacheControl = "no-cache";
                response.Headers.Add("Pragma", "no-cache");
                response.ContentType = "application/octet-stream";

                var responseHeaders = httpContext.Response.GetTypedHeaders();
                responseHeaders.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                {
                    MaxAge = TimeSpan.FromDays(-365),
                    NoCache = true,
                    NoStore = true,
                };
                responseHeaders.Expires = DateTime.Now.AddYears(-1);

                //response.Cache.SetNoStore();
                //response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                //response.Cache.SetExpires(DateTime.Now.AddYears(-1));

                using (var requisicao = new Requisicao(this.CredencialServico, this.GetType().Name))
                {
                    await requisicao.ProcessarRequisicaoAsync(httpContext);

                    if (requisicao.CredencialServicoValida())
                    {

                        this.InformacaoSessaoUsuario = requisicao.InformacaoSessaoUsuario;
                        this.CredencialUsuario = requisicao.CredencialUsuario;
                        this.IdentificadorSessaoUsuario = requisicao.InformacaoSessaoUsuario.IdentificadorSessaoUsuario;
                        this.Inicializar(requisicao);

                        try
                        {
                            var resultadoSerializado = this.RetornarResultadoChamadaSerializado(requisicao, httpContext);
                            if (!response.HasStarted)
                            {
                                response.StatusCode = (int)HttpStatusCode.OK;
                            }

                            if (!requisicao.ContratoChamada.Async && requisicao.SerializarJavascript)
                            {
                                response.ContentType = "application/json; charset=UTF-8";
                                await response.WriteAsync(resultadoSerializado, Encoding.UTF8);
                            }
                            else
                            {
                                var conteudo = PacoteUtil.CompactarPacote(resultadoSerializado);
                                await httpContext.Response.Body.WriteAsync(conteudo);
                            }

                        }

                        catch (Exception ex)
                        {
                            throw new ErroRequisicao(ex, requisicao);
                        }

                    }
                    else
                    {
                        if (!response.HasStarted)
                        {
                            response.StatusCode = (int)HttpStatusCode.NotFound;
                        }

                        var mensagemSeguranca = $"A credencial do serviço não autorizada '{this.GetType().Name}' '{this.CredencialServico.IdentificadorUsuario}' - '{this.CredencialServico.Senha}' ";
                        LogUtil.SegurancaAsync(mensagemSeguranca, EnumTipoLogSeguranca.CredencialServicoNaoAutorizada);

                    }
                }
            }
            catch (ErroDeserializarContrato)
            {
                if (!response.HasStarted)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }

                //response.SubStatusCode = Convert.ToInt32(Servicos.EnumTipoLogSeguranca.ContratoInvalido);

                var mensagemSeguranca = $"O contrato da chamada é invalido '{this.GetType().Name}' '{this.CredencialServico.IdentificadorUsuario} - {this.CredencialServico.Senha}'";
                LogUtil.SegurancaAsync(mensagemSeguranca, EnumTipoLogSeguranca.ContratoInvalido);

            }
            catch (ErroMetodoOperacaoNaoFoiEncontrado erro)
            {
                if (!response.HasStarted)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                //response.SubStatusCode = Convert.ToInt32(Servicos.EnumTipoLogSeguranca.MetodoOperacaoNaoEncontrado);

                var mensagemSeguranca = $"O método '{ erro.NomeMetodo }' não foi encontrado no serviço '{this.GetType().Name}'";
                LogUtil.SegurancaAsync(mensagemSeguranca, Servicos.EnumTipoLogSeguranca.MetodoOperacaoNaoEncontrado);
            }
            catch (ErroRequisicao)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string RetornarResultadoChamadaSerializado(Requisicao requisicao, HttpContext httpContext)
        {
            var metodoOperacao = this.RetornarMetodo(requisicao.Operacao);
            var parametros = this.RetornarParametrosMetodoOperacao(metodoOperacao, requisicao.Parametros);
            parametros = this.NormalizarParametros(parametros);

            var conteudoEmCahce = this.RetornarConteudoCache(requisicao);
            if (conteudoEmCahce != null)
            {
                return conteudoEmCahce;
            }

            var (resultadoChamada, resultadoOperacao) = this.RetornarResultadoChamada(requisicao, httpContext, parametros);
            resultadoChamada.DataHora = DateTime.UtcNow;
            resultadoChamada.NomeServico = this.GetType().Name;

            var resultado = JsonUtil.Serializar(resultadoChamada, requisicao.SerializarJavascript);
            if (this.IsManterCache && !this.OperacoesIgnorarCaches.Contains(requisicao.Operacao))
            {
                if(this.IsManaterResultadoEmCache(metodoOperacao, resultadoOperacao, parametros))
                {
                    this.SalvarChace(requisicao, resultado);
                }
            }
            return resultado;
        }

        protected virtual object RetornarResultadoOperacao(MethodInfo metodoOperacao, object[] parametros)
        {
            try
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
            catch (Exception ex)
            {
                if (this.IsTratarErro)
                {
                    return this.TratarErro(metodoOperacao, parametros, ex);
                }
                throw;
            }

        }



        protected virtual void Inicializar(Requisicao requisicao)
        {
        }

        protected virtual long RetonarTempoLimiteOperacao(Requisicao requisicao)
        {
            return BaseComunicacaoServidor.TEMPO_LIMITE_PADRAO_LOG_DESEMPEHO;
        }

        private void NotifcarLogLentidaoAsync(Requisicao requisicao, Stopwatch tempo)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                var mensagem = this.RetornarMensagemLogLentidao(requisicao.Operacao, tempo);
                LogUtil.DesempenhoAsync(mensagem, tempo, Servicos.EnumTipoLogDesempenho.LentidaoServicoComunicacao, false,
                (Guid identificador) =>
                {
                    var json = JsonUtil.Serializar(requisicao, false);
                    var nomeArquivo = identificador.ToString() + ".json";
                    var caminho = Path.Combine(ConfiguracaoUtil.CaminhoAppDataAplicacaoLogs, "Desempenho", nomeArquivo);
                    ArquivoUtil.SalvarArquivoTexto(caminho, json, true);
                });
            }

        }

        protected virtual string RetornarMensagemLogLentidao(string operacao, Stopwatch tempo)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Lentidão");
            sb.AppendLine($"Servico: {this.GetType().Name}");
            sb.AppendLine($"Operação: { operacao} ");
            sb.AppendLine($"Tempo: {tempo.ElapsedMilliseconds}");
            return sb.ToString();
        }

        protected virtual object[] RetornarParametrosMetodoOperacao(MethodInfo operacao,
                                                                    Dictionary<string, object> parametros)
        {
            var parametrosOperacao = operacao.GetParameters().ToList();
            if (parametrosOperacao.Count != parametros.Count)
            {
                var mensagem = $"O numero de parametros da chamada do metodo operação { operacao.Name} é diferente do serviço {this.GetType().Name} ";
                throw new ErroSeguranca(mensagem, Servicos.EnumTipoLogSeguranca.ParametrosComunicacaoInvalidos);
            }

            var retorno = new List<object>();

            for (var i = 0; i <= parametrosOperacao.Count - 1; i++)
            {
                var parametroMetodo = parametrosOperacao[i];
                var nomeParametroOperacao = parametrosOperacao[i].Name;


                if (!parametros.ContainsKey(nomeParametroOperacao))
                {
                    var mensagem = $"O nome do parametro da chamada do metodo da operação {nomeParametroOperacao} não foi encotrado no dicionario dos parametros";
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
                        throw new ErroSeguranca("A valor da parametro não retorna lista como esperado no tipo do parametro do metodo", EnumTipoLogSeguranca.ParametrosComunicacaoInvalidos);
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
                        var mensagem = $"O erro no converter o tipo do Parametro na chamada do metodo operação {operacao.Name} é diferente do servico {this.GetType().Name},  Nome do parametro '{nomeParametroOperacao}' ";
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

        protected (ResultadoChamada, object) RetornarResultadoChamada(Requisicao requisicao,
                                                            HttpContext httpContext,
                                                            object[] parametros)
        {

            var metodoOperacao = this.RetornarMetodo(requisicao.Operacao);
            var tempo = Stopwatch.StartNew();
            try
            {

                var resultadoOperacao = this.RetornarResultadoOperacao(metodoOperacao, parametros);
                resultadoOperacao = this.NormalizarResultadoOperacao(resultadoOperacao);
                tempo.Stop();

                if (tempo.ElapsedMilliseconds > this.RetonarTempoLimiteOperacao(requisicao))
                {
                    this.NotifcarLogLentidaoAsync(requisicao, tempo);
                }

                var resultadoChamada = this.RetornarResultadoChamadaInterno(resultadoOperacao);
                resultadoChamada.TempoOperacao = (int)tempo.ElapsedMilliseconds;
                return (resultadoChamada, resultadoOperacao);


            }
            catch (Exception ex)
            {

                if (ex is ErroSessaoUsuarioExpirada erroSessaoUsuarioExpirada)
                {
                    return (new ResultadoSessaoUsuarioInvalida(erroSessaoUsuarioExpirada.EstadoSessao, this.IdentificadorSessaoUsuario), null);
                }
                var erroInterno = ex.InnerException;
                if (erroInterno is ErroSessaoUsuarioExpirada erroInternoTipado)
                {
                    return (new ResultadoSessaoUsuarioInvalida(erroInternoTipado.EstadoSessao, this.IdentificadorSessaoUsuario), null);
                }

                var host = httpContext.Request.GetTypedHeaders()?.Referer?.Host;
                if (host.EndsWith(ConstantesDominioSuperior.DOMIMIO_SUPERIOR_LOCALHOST) ||
                    host.EndsWith(ConstantesDominioSuperior.DOMIMIO_SUPERIOR_INTERNO))
                {
                    var mensagemErro = ErroUtil.RetornarDescricaoCompletaErro(ex,
                        metodoOperacao.Name, "", 0);

                    return (new ResultadoChamadaErroInternoServidor
                    {
                        MensagemErro = mensagemErro,
                        TempoOperacao = (int)tempo.ElapsedMilliseconds
                    }, null);
                }

                throw;
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
                throw new ErroMetodoOperacaoNaoFoiEncontrado(nomeMetodo, String.Format("Não existe o metodo {0} implementado no servico {1}", nomeMetodo, this.GetType().Name));
            }

            if (metodos.Count > 1)
            {
                throw new ErroMetodoOperacaoNaoFoiEncontrado(nomeMetodo, String.Format("Existe mais de um metodo {0} implementado no servico {1}", nomeMetodo, this.GetType().Name));
            }
            return metodos.Single();
        }


        protected virtual object RetornarObjetoBloqueioThread()
        {
            throw new Erro($" esse metodo deve ser sobre escrito quando {nameof(this.IsBloqueiarThreadSessaoUsuario)} for true .");
        }

        protected virtual object TratarErro(MethodInfo metodoOperacao, object[] parametros, Exception erro)
        {
            throw new Erro($" esse metodo deve ser sobre escrito quando {nameof(this.IsTratarErro)} for true .");
        }

        protected virtual bool IsManaterResultadoEmCache(MethodInfo metodoOperacao, object resultadoOperacao, object[] parametros)
        {
            throw new Erro($" esse metodo deve ser sobre escrito em . {this.GetType().Name} quando {nameof(this.IsManterCache)} for true ");
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