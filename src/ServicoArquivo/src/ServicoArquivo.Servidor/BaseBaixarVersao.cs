using Snebur.Dominio;
using System;
using System.IO;

#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
#endif

namespace Snebur.ServicoArquivo
{
    public abstract class BaseBaixarVersao<TArquivoVersao> : IHttpHandler where TArquivoVersao : Entidade
    {
        public bool IsReusable => true;

#if NET6_0_OR_GREATER
        public Task ProcessRequestAsync(HttpContext context)
        {
            throw new NotImplementedException();
        }

#else
        public virtual void ProcessRequest(HttpContext context)
        {
            var caminhoArquivoVersao = this.RetornarCaminhoImagem(context);
            var response = context.Response;

            if (File.Exists(caminhoArquivoVersao))
            {
                response.WriteFile(caminhoArquivoVersao);
                context.Response.ContentType = "application/octet-stream";
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.WriteFile(caminhoArquivoVersao);
            }
            else
            {
                LogUtil.ErroAsync(new ErroArquivoNaoEncontrado(caminhoArquivoVersao));
                response.SubStatusCode = 5;
                response.StatusCode = 405;
            }
        }
#endif 
        public virtual string RetornarCaminhoImagem(HttpContext zyonHttpContext)
        {
            var idArquivo = this.RetornarIdArquivoVersao(zyonHttpContext);
            var nomeTipoArquivo = typeof(TArquivoVersao).Name;
            var caminhoDiretorioImagem = this.RetornarDiretorioImagem(idArquivo, nomeTipoArquivo);
            var nomeArquivo = ServicoArquivoUtil.RetornarNomeArquivo(idArquivo, ServicoArquivoUtil.EXTENCAO_ARQUIVO);
            var caminhoImagem = Path.Combine(caminhoDiretorioImagem, nomeArquivo);
            return caminhoImagem;
        }

        public string RetornarDiretorioImagem(long idArquivo, string nomeTipoArquivo)
        {
            var info = new InformacaoRepositorioArquivo(idArquivo, nomeTipoArquivo);
            return ServicoArquivoUtil.RetornarCaminhoDiretorioArquivo(this.RetornarRepositorioArquivo(info), idArquivo);
        }

        #region Métodos privados

        protected long RetornarIdArquivoVersao(HttpContext zyonHttpContext)
        {
            var idImagem = Convert.ToInt64(this.RetornarValorParametro(ConstantesServicoArquivo.ID_ARQUIVO, zyonHttpContext));
            if (!(idImagem > 0))
            {
                throw new Exception(String.Format("Parâmetro '{0}' não foi definido.", ConstantesServicoArquivo.ID_ARQUIVO));
            }
            return idImagem;
        }

        protected string RetornarValorParametro(string parametro, HttpContext httpContext)
        {
#if NET6_0_OR_GREATER
            throw new NotImplementedException();
#else
           var parametroBase64 = Base64Util.Encode(parametro);
            return Base64Util.Decode(httpContext.Request.QueryString[parametroBase64]);
#endif

        }
        #endregion

        protected abstract string RetornarRepositorioArquivo(IInformacaoRepositorioArquivo informacaoRepositorio);

    }

}