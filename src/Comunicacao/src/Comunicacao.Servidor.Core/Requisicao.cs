using Microsoft.AspNetCore.Http;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Snebur.Dominio;
using Snebur.Reflexao;
using Snebur.Seguranca;
using Snebur.Utilidade;

namespace Snebur.Comunicacao
{
    public class Requisicao : IDisposable
    {
        #region Propriedades

        public Cabecalho Cabecalho { get; set; }

        public ContratoChamada ContratoChamada { get; set; }

        public CredencialServico CredencialServico { get; set; }

        public CredencialUsuario CredencialUsuario { get; set; }

        public CredencialUsuario CredencialAvalisata { get; set; }

        public InformacaoSessaoUsuario InformacaoSessaoUsuario { get; set; }

        public string Operacao { get; set; }

        public DateTime DataHoraChamada { get; set; }

        public Dictionary<string, object> Parametros { get; set; }

        public string NomeManipulador { get; }
        public bool SerializarJavascript { get; set; }
        public string Json { get; private set; }

        #endregion

        #region Construtor

        public Requisicao(CredencialServico credencialServico,
                          string nomeManipulador)
        {
            this.CredencialServico = credencialServico;
            this.Parametros = new Dictionary<string, object>();
            this.NomeManipulador = nomeManipulador;
            //this.DeserializarPacote(httpContext);
        }

        #endregion

        #region Metodos

        public bool CredencialServicoValida()
        {
            var credencialServicoRequesicao = this.Cabecalho.CredencialServico;

            return (this.CredencialServico.IdentificadorUsuario == credencialServicoRequesicao.IdentificadorUsuario &&
                    this.CredencialServico.Senha == credencialServicoRequesicao.Senha);
        }



        #endregion

        #region Metodos privados

        public async Task ProcessarRequisicaoAsync(HttpContext context)
        {

            var streamBufferizada = await this.RetornarInputStreamBufferizado(context);
            using (var streamRequisicao = streamBufferizada)
            {
                try
                {
                    var json = PacoteUtil.DescompactarPacote(streamRequisicao);
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        this.Json = json;
                    }
                    this.ContratoChamada = JsonUtil.Deserializar<ContratoChamada>(json, false);
                    this.Cabecalho = this.ContratoChamada.Cabecalho;
                    this.InformacaoSessaoUsuario = this.ContratoChamada.InformacaoSessaoUsuario;
                    this.CredencialUsuario = this.ContratoChamada.Cabecalho.CredencialUsuario;
                    this.CredencialAvalisata = this.ContratoChamada.Cabecalho.CredencialAvalista;
                    this.Operacao = this.ContratoChamada.Operacao;
                    this.DataHoraChamada = this.ContratoChamada.DataHora;

                    this.SerializarJavascript = (this.InformacaoSessaoUsuario.TipoAplicacao == EnumTipoAplicacao.Web || this.InformacaoSessaoUsuario.TipoAplicacao == EnumTipoAplicacao.ApacheCordova);

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

        private async Task<Stream> RetornarInputStreamBufferizado(HttpContext context)
        {
            try
            {
                var cancellationToken = context.RequestAborted;
                var reader = context.Request.Body;
                var ms = new MemoryStream();
                var buffer = new byte[32 * 1024];
                while (!cancellationToken.IsCancellationRequested)
                {
                    var lidos = await reader.ReadAsync(buffer, 0, buffer.Length);
                    if(lidos == 0)
                    {
                        break;
                    }

                    ms.Write(buffer, 0, lidos);
                }
                return ms;
            }
            catch
            {
                throw new ErroReceberStreamCliente("Erro ao receber a stream bufferizada, a conexão foi fechada pelo cliente");
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
                    foreach (var valor in parametroChamadaListaTipoPrimario.Lista)
                    {
                        listaPrimario.Add(ConverterUtil.ConverterTipoPrimario(valor, tipoListaPrimarioEnum));
                    }
                    return listaPrimario;

                default:

                    throw new Erro("Parametro não suportado ");


            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {

        }

        #endregion
    }
}
