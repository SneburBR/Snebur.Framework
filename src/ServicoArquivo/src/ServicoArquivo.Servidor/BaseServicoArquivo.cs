using Snebur.ServicoArquivo.Servidor;
using Snebur.Utilidade;
using System;
using System.IO;
using System.Text;
using System.Web;

#if NET6_0_OR_GREATER
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
            (AplicacaoSnebur.Atual as BaseAplicacaoServicoArquivo).AcessarRede();
        }

        #endregion

        #region IHttpHandler

#if NET6_0_OR_GREATER
     
        public async Task ProcessRequestAsync(HttpContext context)
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
                    await this.IniciarAsync(context, cabecalho, inputStream);
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
                    var respostaString = JsonUtil.Serializar(resposta, EnumTipoSerializacao.Javascript);
                    await context.Response.WriteAsync(respostaString);
                }
            }
        }
#else

        public void ProcessRequest(HttpContext context)
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


                    var respostaString = JsonUtil.Serializar(resposta, EnumTipoSerializacao.Javascript);
                    context.Response.Write(respostaString);
                }
            }
        }

#endif

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

        protected virtual TCabecalhoServicoArquivo RetornarCabecalhoServicoArquivo(HttpContext httpContext)
        {
            return new CabecalhoServicoArquivo(httpContext) as TCabecalhoServicoArquivo;
        }

        protected virtual ComunicacaoServicoArquivoCliente RetornarServicoArquivoCliente(TCabecalhoServicoArquivo cabecalho)
        {
            var urlServicoArquivoCliente = this.RetornarUrlServicoArquivoCliente();
            return new ComunicacaoServicoArquivoCliente(urlServicoArquivoCliente,
                                                        cabecalho.CredencialRequisicao,
                                                        cabecalho.IdentificadorSessaoUsuario,
                                                        cabecalho.IdentificadorProprietario,
                                                        this.NormalizarOrigem);
        }
        #endregion

        #region Métodos abstratos

#if NET6_0_OR_GREATER
        protected abstract Task IniciarAsync(HttpContext context, TCabecalhoServicoArquivo cabecalho, MemoryStream inputStream);
#else
        protected abstract void Iniciar(HttpContext context, TCabecalhoServicoArquivo cabecalho, MemoryStream inputStream);
#endif
        protected abstract string RetornarRepositoArquivo(TInformacaoRepositorio informacaoRepositorio);

        protected abstract string RetornarUrlServicoArquivoCliente();

        protected abstract string NormalizarOrigem(string origem);


#endregion

        #region Métodos privados

#if NET6_0_OR_GREATER == false
        private MemoryStream RetornarInputStream(HttpContext httpContext)
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
#endif

#if NET6_0_OR_GREATER
        private async Task<MemoryStream> RetornarInputStreamAsync(HttpContext httpContext)
        {
            try
            {
                var cancellationToken = httpContext.RequestAborted;
                var reader = httpContext.Request.Body;
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
            catch
            {
                throw new Erro("Erro ao receber a stream bufferizada, a conexão foi fechada pelo cliente");
            }
        }
#endif


        #endregion

        #region IDisposable

        public void Dispose()
        {
            //ZyonHttpContext.Current?.Dispose();
        }

        #endregion
    }
}