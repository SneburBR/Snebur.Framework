
using Snebur.Dominio;
using Snebur.Reflexao;
using Snebur.Seguranca;
using Snebur.Utilidade;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;

#if NET7_0
using Microsoft.AspNetCore.Http;
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
        public InformacaoSessaoUsuario InformacaoSessaoUsuario { get; private set; }
        public string IdentificadorProprietario { get; private set; }
        public string Operacao { get; private set; }
        public DateTime DataHoraChamada { get; private set; }
        public Dictionary<string, object> Parametros { get; private set; }
        public string NomeManipulador { get; }
        public bool IsSerializarJavascript { get; set; }
        public EnumTipoSerializacao TipoSerializacao => this.IsSerializarJavascript ? EnumTipoSerializacao.Javascript : 
                                                                                      EnumTipoSerializacao.DotNet;
        public HttpContext HttpContext { get; private set; }

        #endregion

        #region Construtor

        public Requisicao(HttpContext httpContext,
                          CredencialServico credencialServico,
                          string identificadorProprietario,
                          string nomeManipulador )
        {
            this.HttpContext = httpContext;
            this.CredencialServico = credencialServico;
            this.Parametros = new Dictionary<string, object>();
            this.IdentificadorProprietario = identificadorProprietario;
            this.NomeManipulador = nomeManipulador;
            this.DeserializarPacote(httpContext);
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

        private void DeserializarPacote(HttpContext context)
        {

            var streamBufferizada = this.RetornarInputStreamBufferizado(context);
            using (var streamRequisicao = streamBufferizada)
            {
                try
                {
                    var json = PacoteUtil.DescompactarPacote(streamRequisicao);
                    if (DebugUtil.IsAttached)
                    {
                        this._jsonRequisacao = json;
                    }
                    this.ContratoChamada = JsonUtil.Deserializar<ContratoChamada>(json, EnumTipoSerializacao.DotNet);
                    this.Cabecalho = this.ContratoChamada.Cabecalho;
                    this.InformacaoSessaoUsuario = this.ContratoChamada.InformacaoSessaoUsuario;
                    this.CredencialUsuario = this.ContratoChamada.Cabecalho.CredencialUsuario;
                    this.CredencialAvalisata = this.ContratoChamada.Cabecalho.CredencialAvalista;
                    this.Operacao = this.ContratoChamada.Operacao;
                    this.DataHoraChamada = this.ContratoChamada.DataHora;
                    this.AdicionarItensrequisicaoAtual();

                    this.IsSerializarJavascript = (this.InformacaoSessaoUsuario.TipoAplicacao == EnumTipoAplicacao.Typescript  );


                    foreach (var parametro in this.ContratoChamada.Parametros)
                    {
                        this.Parametros.Add(parametro.Nome, this.RetornarValorParametroChamada(parametro));
                    }
                }
                catch (Exception erro)
                {
                    throw new ErroContratoChamada(erro);
                }
            }
        }

        private Stream RetornarInputStreamBufferizado(HttpContext context)
        {
            try
            {
#if NET7_0
                return context.Request.Body;
#else
                return StreamUtil.RetornarMemoryStreamBuferizada(context.Request.GetBufferedInputStream());
                //return context.Request.InputStream;
#endif

            }
            catch
            {
                throw new ErroReceberStreamCliente("Erro ao receber a stream buffered, a conexão foi fechada pelo cliente");
            }
        }

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

                    //var tipoBaseDominio = ReflexaoUtil.RetornarTipo(parametroListaBaseDominio.NomeNamespaceTipoBaseDominio, parametroListaBaseDominio.NomeTipoBaseDominio);
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

        private void AdicionarItensrequisicaoAtual( )
        {
            var context = this.HttpContext;
            lock ((context.Items as ICollection).SyncRoot)
            {
                context.AdicionrItem(ConstantesItensRequsicao.CHAVE_INFORMACAO_SESSAO_ATUAL, this.InformacaoSessaoUsuario);
                context.AdicionrItem(ConstantesItensRequsicao.CHAVE_CREDENCIAL_USUARIO, this.CredencialUsuario);
                context.AdicionrItem(ConstantesItensRequsicao.CHAVE_IDENTIFICADOR_PROPRIETARIO, this.IdentificadorProprietario);

                if (this.CredencialAvalisata != null)
                {
                    context.AdicionrItem(ConstantesItensRequsicao.CHAVE_CREDENCIAL_USUARIO_AVALISTA, this.CredencialUsuario);
                }
                //if (!context.Items.Contains(InformacaoSessaoUsuario.CHAVE_INFORMACAO_SESSAO_ATUAL))
                //{
                //    context.Items.Remove(informacaoSessaoUsuario);
                //}
            }
        }
        private void RemoverItensRequisicaoAtual()
        {
            var context = this.HttpContext;
            lock ((context.Items as ICollection).SyncRoot)
            {
                context.RemoverItem(ConstantesItensRequsicao.CHAVE_INFORMACAO_SESSAO_ATUAL);
                context.RemoverItem(ConstantesItensRequsicao.CHAVE_CREDENCIAL_USUARIO);
                context.RemoverItem(ConstantesItensRequsicao.CHAVE_CREDENCIAL_USUARIO_AVALISTA);
                context.RemoverItem(ConstantesItensRequsicao.CHAVE_IDENTIFICADOR_PROPRIETARIO);
            }
        }

        #region IDisposable

        public void Dispose()
        {
            this.RemoverItensRequisicaoAtual();
            this.HttpContext = null;
        }

        #endregion
    }

    
}
