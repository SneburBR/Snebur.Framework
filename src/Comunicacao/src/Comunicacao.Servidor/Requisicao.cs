
using Snebur.Dominio;
using Snebur.Reflexao;
using Snebur.Seguranca;
using Snebur.Utilidade;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Snebur.Linq;
using Newtonsoft.Json;

#if NET6_0_OR_GREATER
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif

namespace Snebur.Comunicacao
{
    public class Requisicao : IDisposable
    {
        private string _jsonRequisacao;

        #region Propriedades

        public Cabecalho Cabecalho { get; private set; }
        public ContratoChamada ContratoChamada { get; private set; }
        public CredencialServico CredencialServico { get; private set; }
        public CredencialUsuario CredencialUsuario { get; private set; }
        public CredencialUsuario CredencialAvalisata { get; private set; }
        public InformacaoSessao InformacaoSessaoUsuario { get; private set; }

        public Guid IdentificadorSessaoUsuario { get; set; }
        public string CaminhoAplicacao { get; }
        public string IdentificadorProprietario { get; private set; }
        public string Operacao { get; private set; }
        public DateTime DataHoraChamada { get; private set; }
        public Dictionary<string, object> Parametros { get; } = new Dictionary<string, object>();
        public string NomeManipulador { get; }
        public bool IsSerializarJavascript { get; set; }
        public EnumTipoSerializacao TipoSerializacao => this.IsSerializarJavascript ? EnumTipoSerializacao.Javascript :
                                                                                      EnumTipoSerializacao.DotNet;

        [XmlIgnore, JsonIgnore]
        public HttpContext HttpContext { get; private set; }

        #endregion

        #region Construtor

        public Requisicao(HttpContext httpContext,
                          CredencialServico credencialServico,
                          string identificadorProprietario,
                          string nomeManipulador)
        {
#if NET6_0_OR_GREATER
            this.CaminhoAplicacao = httpContext.Items[ConstantesItensRequsicao.CAMINHO_APLICACAO]?.ToString();
#else
            this.CaminhoAplicacao = httpContext.Server.MapPath("~");
#endif
            this.HttpContext = httpContext;
            this.CredencialServico = credencialServico;
            this.IdentificadorProprietario = identificadorProprietario;
            this.NomeManipulador = nomeManipulador;

            if (!Directory.Exists(this.CaminhoAplicacao))
            {
                throw new DirectoryNotFoundException($"Caminho da aplicação não encontrado {this.CaminhoAplicacao}");
            }

        }

#if NET6_0_OR_GREATER
        public async Task ExtrairDadosRequisicaoAsync()
        {
            var httpContext = this.HttpContext;
            using (var streamRequisicao = await this.RetornarInputStreamBufferizado(httpContext))
            {
                this.ExtrairDadosRequisicao(streamRequisicao);
            }

        }

        private async Task<MemoryStream> RetornarInputStreamBufferizado(HttpContext context)
        {
            try
            {
                var resultado = new MemoryStream();
                var streamOrigem = context.Request.Body;
                while (true)
                {
                    var buffer = new byte[1024];
                    var bytesRead = await streamOrigem.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    await resultado.WriteAsync(buffer, 0, bytesRead);
                }
                resultado.Position = 0;
                return resultado;

            }
            catch
            {
                throw new ErroReceberStreamCliente("Erro ao receber a stream buffered, a conexão foi fechada pelo cliente");
            }
        }

#else
        public void ExtrairDadosRequisicao()
        {
            var httpContext = this.HttpContext;
            using (var streamRequisicao = this.RetornarInputStreamBufferizado(httpContext))
            {
                this.ExtrairDadosRequisicao(streamRequisicao);
            }
        }

        private MemoryStream RetornarInputStreamBufferizado(HttpContext context)
        {
            try
            {
                return StreamUtil.RetornarMemoryStreamBuferizada(context.Request.GetBufferedInputStream());
            }
            catch
            {
                throw new ErroReceberStreamCliente("Erro ao receber a stream buffered, a conexão foi fechada pelo cliente");
            }
        }


#endif

        private void ExtrairDadosRequisicao(MemoryStream streamRequisicao)
        {
            var json = PacoteUtil.DescompactarPacote(streamRequisicao);
            if (DebugUtil.IsAttached)
            {
                this._jsonRequisacao = json;
            }
            this.ContratoChamada = JsonUtil.Deserializar<ContratoChamada>(json, EnumTipoSerializacao.DotNet);
            this.Cabecalho = this.ContratoChamada.Cabecalho;
            this.InformacaoSessaoUsuario = this.ContratoChamada.InformacaoSessao;
            this.IdentificadorSessaoUsuario = this.ContratoChamada.IdentificadorSessaoUsuario;
            this.CredencialUsuario = this.ContratoChamada.Cabecalho.CredencialUsuario;
            this.CredencialAvalisata = this.ContratoChamada.Cabecalho.CredencialAvalista;
            this.Operacao = this.ContratoChamada.Operacao;
            this.DataHoraChamada = this.ContratoChamada.DataHora;
            this.AdicionarItensrequisicaoAtual();

            this.IsSerializarJavascript = (this.InformacaoSessaoUsuario.TipoAplicacao == EnumTipoAplicacao.Typescript);

            foreach (var parametro in this.ContratoChamada.Parametros)
            {
                this.Parametros.Add(NormalizacaoUtil.NormalizarNomeParametro(parametro.Nome), this.RetornarValorParametroChamada(parametro));
            }

            if(this.IdentificadorSessaoUsuario == Guid.Empty)
            {
                throw new Erro("Identificador da sessão do usuário não foi definido");
            }
        }

        #endregion

        #region Métodos

        public bool CredencialServicoValida()
        {
            var credencialServicoRequesicao = this.Cabecalho.CredencialServico;

            return (this.CredencialServico.IdentificadorUsuario == credencialServicoRequesicao.IdentificadorUsuario &&
                    this.CredencialServico.Senha == credencialServicoRequesicao.Senha);
        }

        #endregion

        #region Métodos privados
         
        private object RetornarValorParametroChamada(ParametroChamada parametro)
        {
            switch (parametro)
            {
                case ParametroChamadaNulo parametroChamadaNulo:

                    return null;

                case ParametroChamadaBaseDominio parametroBaseDominio:

                    return parametroBaseDominio.BaseDominio;

                case ParametroChamadaEnum parametroChamadaEnum:

                    return parametroChamadaEnum.Valor;

                case ParametroChamadaTipoPrimario parametroChamadaTipoPrimario:
                    var tipoPrimarioEnum = parametroChamadaTipoPrimario.TipoPrimarioEnum;

                    if (tipoPrimarioEnum == EnumTipoPrimario.EnumValor)
                    {
                        throw new InvalidOperationException("Usar ParametroChamadaEnum");
                    }
                    return ConverterUtil.ConverterTipoPrimario(parametroChamadaTipoPrimario.Valor, tipoPrimarioEnum);

                case ParametroChamadaListaBaseDominio parametroChamadaListaBaseDominio:

                    var basesDominio = parametroChamadaListaBaseDominio.BasesDominio;

                    var tipoBaseDominio = Type.GetType(parametro.AssemblyQualifiedName);
                    var tipoLista = typeof(List<>).MakeGenericType(tipoBaseDominio);

                    var lista = (IList)Activator.CreateInstance(tipoLista);
                    foreach (var baseDominio in parametroChamadaListaBaseDominio.BasesDominio)
                    {
                        lista.Add(baseDominio);
                    }
                    return lista;

                case ParametroChamadaListaEnum parametroListaEnum:

                    var tipoEnum = Type.GetType(parametroListaEnum.AssemblyQualifiedName);
                    var tipoListaEnum = typeof(List<>).MakeGenericType(tipoEnum);
                    var listaEnum = (IList)Activator.CreateInstance(tipoListaEnum);
                    foreach (var valorEnum in parametroListaEnum.Valores)
                    {
                        listaEnum.Add(valorEnum);
                    }
                    return listaEnum;

                case ParametroChamadaListaTipoPrimario parametroChamadaListaTipoPrimario:

                    var tipoListaPrimarioEnum = parametroChamadaListaTipoPrimario.TipoPrimarioEnum;
                    var tipoPrimario = ReflexaoUtil.RetornarTipoPrimario(tipoListaPrimarioEnum);
                    var tipoListaPrimario = typeof(List<>).MakeGenericType(tipoPrimario);

                    var listaPrimario = (IList)Activator.CreateInstance(tipoListaPrimario);
                    if (parametroChamadaListaTipoPrimario.Lista != null)
                    {
                        foreach (var valor in parametroChamadaListaTipoPrimario.Lista)
                        {
                            listaPrimario.Add(ConverterUtil.ConverterTipoPrimario(valor, tipoListaPrimarioEnum));
                        }
                    }
                    return listaPrimario;

                default:

                    throw new Erro("Parâmetro não suportado ");
            }
        }

        #endregion

        private void AdicionarItensrequisicaoAtual()
        {
            var context = this.HttpContext;
            lock (context?.Items.SyncLock())
            {
                context.AdicionrItem(ConstantesItensRequsicao.CHAVE_INFORMACAO_SESSAO_ATUAL, this.InformacaoSessaoUsuario);
                context.AdicionrItem(ConstantesItensRequsicao.CHAVE_CREDENCIAL_USUARIO, this.CredencialUsuario);
                context.AdicionrItem(ConstantesItensRequsicao.CHAVE_IDENTIFICADOR_PROPRIETARIO, this.IdentificadorProprietario);

                if (this.CredencialAvalisata != null)
                {
                    context.AdicionrItem(ConstantesItensRequsicao.CHAVE_CREDENCIAL_USUARIO_AVALISTA, this.CredencialUsuario);
                }
            }
        }

        private void RemoverItensRequisicaoAtual()
        {
            var context = this.HttpContext;
            lock (context?.Items.SyncLock())
            {
                context.RemoverItem(Dominio.ConstantesItensRequsicao.CHAVE_INFORMACAO_SESSAO_ATUAL);
                context.RemoverItem(Dominio.ConstantesItensRequsicao.CHAVE_CREDENCIAL_USUARIO);
                context.RemoverItem(Dominio.ConstantesItensRequsicao.CHAVE_CREDENCIAL_USUARIO_AVALISTA);
                context.RemoverItem(Dominio.ConstantesItensRequsicao.CHAVE_IDENTIFICADOR_PROPRIETARIO);
            }
        }

        #region IDisposable

        public void Dispose()
        {
            this.RemoverItensRequisicaoAtual();
            this.HttpContext = null;
        }

        public void Processar()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
