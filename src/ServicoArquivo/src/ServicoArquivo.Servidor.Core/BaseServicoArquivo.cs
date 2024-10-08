﻿using System;
using System.Text;
using System.Web;
using Snebur.Net;
using Snebur.Utilidade;
using System.IO;

#if NET7_0
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
#endif

namespace Snebur.ServicoArquivo
{

    public abstract class BaseServicoArquivo<TCabecalhoServicoArquivo, TInformacaoRepositorio> : IHttpHandler, IDisposable where TCabecalhoServicoArquivo : CabecalhoServicoArquivo
                                                                                                                           where TInformacaoRepositorio : IInformacaoRepositorioArquivo
    {
        #region Construtores

        public virtual bool IsReponserJson => true;

        public BaseServicoArquivo()
        {
        }

        #endregion

        #region IHttpHandler

#if NET7_0
        public async Task ProcessRequestAsync(HttpContext context)
        {
            using (var zyonHttpContext = new ZyonHttpContextCore(context))
            {
                await this.ProcessRequestAsyc(zyonHttpContext);
            }
        }

        private async Task ProcessRequestAsyc(ZyonHttpContextCore context)
        {
            Exception erro = null;
            var tipoErro = EnumTipoErroServicoArquivo.Desconhecido;
            try
            {
                var cabecalho = this.RetornarCabecalhoServicoArquivo(context);
                if (cabecalho.IsCabecalhoValido())
                {
                    this.ServicoArquivoCliente = this.RetornarServicoArquivoCliente(cabecalho);
                    var inputStream = await this.RetornarInputStreamAsync(context);
                    this.Iniciar(context, cabecalho, inputStream);
                }
            }
            catch (Exception ex)
            {
                erro = ex;
                tipoErro = this.RetornarTipoErro(ex);

            }
            finally
            {

                if (tipoErro == EnumTipoErroServicoArquivo.ArquivoNaoEncontrado)
                {
                    throw erro;
                }

                if (this.IsReponserJson)
                {
                    var resposta = new ResultadoServicoArquivo()
                    {
                        IsSucesso = erro == null
                    };
                    if ((erro != null))
                    {
                        resposta.MensagemErro = erro.Message;
                        resposta.TipoErroServicoArquivo = tipoErro;
                    }
                    //context.Response.AddHeader("Access-Control-Allow-Origin", "*")
                    //context.Response.AddHeader("Access-Control-Allow-Methods", "POST")
                    context.Response.ContentType = "text/json; charset=UTF-8";
                    context.Response.ContentEncoding = Encoding.UTF8;

                    var respostaString = JsonUtil.Serializar(resposta, true);
                    await context.Response.WriteAsync(respostaString);
                }
            }
        }

#endif
        public void ProcessRequest(HttpContext context)
        {
            using (var zyonHttpContext = new ZyonHttpContextCore(context))
            {
                this.ProcessRequest(zyonHttpContext);
            }
        }

        public virtual void ProcessRequest(ZyonHttpContext context)
        {
            Exception erro = null;
            var tipoErro = EnumTipoErroServicoArquivo.Desconhecido;
            try
            {
                var cabecalho = this.RetornarCabecalhoServicoArquivo(context);
                if (cabecalho.IsCabecalhoValido())
                {
                    this.ServicoArquivoCliente = this.RetornarServicoArquivoCliente(cabecalho);
                    var inputStream = this.RetornarInputStream(context);
                    this.Iniciar(context, cabecalho, inputStream);
                }
            }
            catch (Exception ex)
            {
                erro = ex;
                tipoErro = this.RetornarTipoErro(ex);

            }
            finally
            {

                if (tipoErro == EnumTipoErroServicoArquivo.ArquivoNaoEncontrado)
                {
                    throw erro;
                }

                if (this.IsReponserJson)
                {
                    var resposta = new ResultadoServicoArquivo()
                    {
                        IsSucesso = erro == null
                    };
                    if ((erro != null))
                    {
                        resposta.MensagemErro = erro.Message;
                        resposta.TipoErroServicoArquivo = tipoErro;
                    }
                    //context.Response.AddHeader("Access-Control-Allow-Origin", "*.grafis.com.br, *.photosapp.com.br")
                    //context.Response.AddHeader("Access-Control-Allow-Origin", "*")
                    //context.Response.AddHeader("Access-Control-Allow-Methods", "POST")

                    context.Response.ContentType = "text/json";
                    context.Response.ContentEncoding = Encoding.UTF8;


                    var respostaString = JsonUtil.Serializar(resposta, true);
                    context.Response.Write(respostaString);
                }
            }
        }

        private EnumTipoErroServicoArquivo RetornarTipoErro(Exception erro)
        {
            switch (erro)
            {
                case ErroChecksumArquivo erroChecksumArquivo:

                    return EnumTipoErroServicoArquivo.ChecksumArquivoDiferente;

                case ErroChecksumPacote erroChecksumPacote:

                    return EnumTipoErroServicoArquivo.ChecksumPacoteDiferente;

                case ErroTotalBytesDiferente erroTotalBytesDiferente:

                    return EnumTipoErroServicoArquivo.TotalBytesDiferente;

                case ErroArquivoEmUso erroArquivoEmUso:

                    return EnumTipoErroServicoArquivo.ArquivoTempEmUso;

                case ErroArquivoNaoEncontrado erroArquivoNaoEncontrado:

                    return EnumTipoErroServicoArquivo.ArquivoNaoEncontrado;

                case ErroIdArquivoNaoExiste erroArquivoNaoEncontrado:

                    return EnumTipoErroServicoArquivo.IdArquivoNaoExiste;

                default:

                    LogUtil.ErroAsync(erro);
                    return EnumTipoErroServicoArquivo.Desconhecido;
            }
        }

        public bool IsReusable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Metodos protegidos

        protected ComunicacaoServicoArquivoCliente ServicoArquivoCliente { get; set; }

        protected virtual string RetornarCaminhoCompletoArquivo(TInformacaoRepositorio cabecalho)
        {
            return ServicoArquivoUtil.RetornarCaminhoCompletoArquivo(this.RetornarDiretorioArquivo(cabecalho), cabecalho.IdArquivo);
        }

        protected virtual string RetornarDiretorioArquivo(TInformacaoRepositorio cabecalho)
        {
            return ServicoArquivoUtil.RetornarCaminhoDiretorioArquivo(this.RetornarRepositoArquivo(cabecalho), cabecalho.IdArquivo);
        }

        protected virtual TCabecalhoServicoArquivo RetornarCabecalhoServicoArquivo(ZyonHttpContext httpContext)
        {
            return new CabecalhoServicoArquivo(httpContext) as TCabecalhoServicoArquivo;
        }

        protected virtual ComunicacaoServicoArquivoCliente RetornarServicoArquivoCliente(TCabecalhoServicoArquivo cabecalho)
        {
            var urlServicoArquivoCliente = this.RetornarUrlServicoArquivoCliente();
            return new ComunicacaoServicoArquivoCliente(urlServicoArquivoCliente,
                                                        cabecalho.CredencialRequisicao,
                                                        cabecalho.IdentificadorSessaoUsuario,
                                                        this.NormalizarOrigem);
        }
        #endregion

        #region Métodos abstratos

        protected abstract void Iniciar(ZyonHttpContext context, TCabecalhoServicoArquivo cabecalho, MemoryStream inputStream);

        protected abstract string RetornarRepositoArquivo(TInformacaoRepositorio informacaoRepositorio);

        protected abstract string RetornarUrlServicoArquivoCliente();

        protected abstract string NormalizarOrigem(string origem);


        #endregion


        #region Métodos privados

        private MemoryStream RetornarInputStream(ZyonHttpContext httpContext)
        {
            var buffer = new byte[16 * 1024];
            var msPacote = new MemoryStream();

            while (true)
            {
                var lidos = httpContext.Request.InputStream.Read(buffer, 0, buffer.Length);
                if (lidos == 0)
                {
                    break;
                }
                msPacote.Write(buffer, 0, lidos);
            }
            return msPacote;

        }


        private async Task<MemoryStream> RetornarInputStreamAsync(ZyonHttpContext httpContext)
        {
            try
            {
                if (httpContext.ContextoNativo is HttpContext context)
                {
                    var cancellationToken = context.RequestAborted;
                    var reader = context.Request.Body;
                    var ms = new MemoryStream();
                    var buffer = new byte[32 * 1024];
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var lidos = await reader.ReadAsync(buffer, 0, buffer.Length);
                        if (lidos == 0)
                        {
                            break;
                        }

                        ms.Write(buffer, 0, lidos);
                    }
                    return ms;
                }

                throw new Erro("Contexto não suportado");

            }
            catch
            {
                throw new Erro("Erro ao receber a stream bufferizada, a conexão foi fechada pelo cliente");
            }
        }


        #endregion

        #region IDisposable

        public void Dispose()
        {
            //ZyonHttpContext.Current?.Dispose();
        }

        #endregion
    }
}