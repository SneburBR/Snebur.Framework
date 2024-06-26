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
using System.Threading.Tasks;
using System.Net.Http;

#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Http;
#endif

namespace Snebur.Comunicacao
{
    public abstract partial class BaseComunicacaoServidor : IHttpHandler, IDisposable, IBaseServico
    {
        private static readonly long TEMPO_LIMITE_PADRAO_LOG_DESEMPEHO = (DebugUtil.IsAttached) ? (long)TimeSpan.FromSeconds(5).TotalMilliseconds : (long)TimeSpan.FromSeconds(2).TotalMilliseconds;
        protected bool _isPodeDispensarServico = true;

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

        private string RetornarResultadoChamadaSerializado(Requisicao requisicao,
                                                           HttpContext httpContext)
        {
            var tempo = Stopwatch.StartNew();
            var resultado = this.RetornarResultadoChamadaSerializadoInterno(requisicao, httpContext);
            if (tempo.ElapsedMilliseconds > this.RetonarTempoLimiteOperacao(requisicao))
            {
                this.NotificarLogLentidaoAsync(requisicao, tempo);
            }
            return resultado;
        }

        private string RetornarResultadoChamadaSerializadoInterno(Requisicao requisicao, HttpContext httpContext)
        {
            var parametros = requisicao.Parametros;
            var operacao = requisicao.Operacao;

            try
            {
                if (this.IsManterCache && !this.OperacoesIgnorarCaches.Contains(requisicao.Operacao))
                {
                    return this.RetornarResultadoChamadaSerializadoCache(parametros, operacao, requisicao.TipoSerializacao);
                }
                return this.RetornarResultadoChamadaSerializadoInterno(parametros,
                                                                        operacao,
                                                                        requisicao.TipoSerializacao);
            }
            catch (Exception ex)
            {
                var resultadoErro = this.RetornarResultadoChamadaErroInterno(ex, operacao);
                if (!(ex is Erro))
                {
                    LogUtil.ErroAsync(new ErroComunicacao(resultadoErro.MensagemErro, ex));
                }
                return JsonUtil.Serializar(resultadoErro, requisicao.TipoSerializacao);
            }
        }

        private ResultadoChamadaErro RetornarResultadoChamadaErroInterno(Exception ex, string operacao)
        {
            if (ex is ErroSessaoUsuarioExpirada erroSessaoUsuarioExpirada)
            {
                return new ResultadoSessaoUsuarioInvalida(erroSessaoUsuarioExpirada.StatusSessao,
                                                          this.IdentificadorSessaoUsuario,
                                                          "Sessão expirada");
            }
            var erroInterno = ex.InnerException;
            if (erroInterno is ErroSessaoUsuarioExpirada erroInternoTipado)
            {
                return new ResultadoSessaoUsuarioInvalida(erroInternoTipado.StatusSessao,
                                                          this.IdentificadorSessaoUsuario,
                                                          "Sessão inválida");
            }

            var mensagemErro = ErroUtil.RetornarDescricaoCompletaErro(ex,
                                                                     operacao,
                                                                     "",
                                                                     0);

            return new ResultadoChamadaErroInternoServidor
            {
                MensagemErro = mensagemErro,
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }

        private string RetornarResultadoChamadaSerializadoInterno(Dictionary<string, object> dicionarParametros,
                                                                 string operacao,
                                                                 EnumTipoSerializacao tipoSerializacao)
        {
            var metodoOperacao = this.RetornarMetodo(operacao);
            var parametros = this.RetornarParametrosMetodoOperacao(metodoOperacao, dicionarParametros);
            parametros = this.NormalizarParametros(parametros);

            var resultadoChamada = this.RetornarResultadoChamada(operacao, parametros);
            resultadoChamada.DataHora = DateTime.UtcNow;
            resultadoChamada.NomeServico = this.GetType().Name;
            return JsonUtil.Serializar(resultadoChamada, tipoSerializacao);
        }

        protected virtual object RetornarResultadoOperacao(MethodInfo metodoOperacao,
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

        private void NotificarLogLentidaoAsync(Requisicao requisicao,
                                               Stopwatch tempo)
        {
            if (!DebugUtil.IsAttached)
            {
                var mensagem = this.RetornarMensagemLogLentidao(requisicao.Operacao, tempo);
                LogUtil.DesempenhoAsync(mensagem, tempo, EnumTipoLogDesempenho.LentidaoServicoComunicacao, false,
                (Guid identificador) =>
                {
                    var json = JsonUtil.Serializar(requisicao, EnumTipoSerializacao.Javascript);
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
                var nomeParametroOperacao = NormalizacaoUtil.NormalizarNomeParametro(parametrosOperacao[i].Name);

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
                else if (ReflexaoUtil.IsTipoRetornaColecao(parametroMetodo.ParameterType))
                {
                    if (!parametroMetodo.ParameterType.IsGenericType)
                    {
                        throw new Exception("O tipo do parâmetro não é suportado, somente List<T> T = sub classe de BaseDominio ou do tipo primário");
                    }
                    var definitionType = parametroMetodo.ParameterType.GetGenericTypeDefinition();

                    if ((!Object.ReferenceEquals(definitionType, typeof(List<>))) &&
                        (!Object.ReferenceEquals(definitionType, typeof(IEnumerable<>))))
                    {
                        throw new ErroSeguranca("Suportado somente tipo List(Of T ) T = sub classe de BaseDominio  ", EnumTipoLogSeguranca.ParametrosComunicacaoInvalidos);
                    }

                    if (!ReflexaoUtil.IsTipoRetornaColecao(valorParametro.GetType()))
                    {
                        throw new ErroSeguranca("A valor do parâmetro não retorna lista como esperado no tipo do parâmetro do método", EnumTipoLogSeguranca.ParametrosComunicacaoInvalidos);
                    }


                    var tipoItemColecao = parametroMetodo.ParameterType.GetGenericArguments().Single();
                    if (!(tipoItemColecao.IsSubclassOf(typeof(BaseDominio)) ||
                          typeof(IEntidade).IsAssignableFrom(tipoItemColecao) ||
                          ReflexaoUtil.TipoRetornaTipoPrimario(tipoItemColecao)))
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

        private ResultadoChamada RetornarResultadoChamada(string operacao,
                                                          object[] parametros)
        {
            var metodoOperacao = this.RetornarMetodo(operacao);


            var resultadoOperacao = this.RetornarResultadoOperacao(metodoOperacao, parametros);
            resultadoOperacao = this.NormalizarResultadoOperacao(resultadoOperacao);

            return this.RetornarResultadoChamadaInterno(resultadoOperacao);

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

            if (ReflexaoUtil.IsTipoRetornaColecao(tipo) && tipo.IsGenericType && tipo.GetGenericArguments().Count() == 1)
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

        private object RetornarObjetoBloqueioThreadCache(Dictionary<string, object> parametros,
                                                        string operacao,
                                                        EnumTipoSerializacao tipoSerializacao)
        {
            var chaveRequisica = this.RetornarChaveCache(parametros, operacao, tipoSerializacao);
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