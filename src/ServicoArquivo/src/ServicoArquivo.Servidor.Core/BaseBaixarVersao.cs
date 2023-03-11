using System;
using System.IO;
using System.Text;
using System.Web;
using Snebur.Dominio;
using Snebur.Net;
using Snebur.Utilidade;

#if NET7_0
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
#endif

namespace Snebur.ServicoArquivo
{
    public abstract class BaseBaixarVersao<TArquivoVersao> : IHttpHandler where TArquivoVersao : Entidade
    {
        public bool IsReusable => true;

#if NET7_0
        public Task ProcessRequestAsync(HttpContext context)
        {
            using (var zyonHttpContext = new ZyonHttpContextCore(context))
            {
                this.ProcessRequest(zyonHttpContext);
            }
            return null;
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

        public virtual string RetornarCaminhoImagem(ZyonHttpContext zyonHttpContext)
        {
            var idArquivo = this.RetornarIdArquivoVersao(zyonHttpContext);
            var nomeTipoArquivo = typeof(TArquivoVersao).Name;
            var caminhoDiretorioImagem = this.RetornarDiretorioImagem(idArquivo, nomeTipoArquivo );
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

        protected long RetornarIdArquivoVersao(ZyonHttpContext zyonHttpContext)
        {
            var idImagem = Convert.ToInt64(this.RetornarValorParametro(ConstantesServicoArquivo.ID_ARQUIVO, zyonHttpContext));
            if (!(idImagem > 0))
            {
                throw new Exception(String.Format("Parâmetro '{0}' não foi definido.", ConstantesServicoArquivo.ID_ARQUIVO));
            }
            return idImagem;
        }

          
        protected string RetornarValorParametro(string parametro, ZyonHttpContext zyonHttpContext)
        {
            var parametroBase64 = Base64Util.Encode(parametro);
            return Base64Util.Decode(zyonHttpContext.Request.QueryString[parametroBase64]);
        }
        #endregion

        protected abstract string RetornarRepositorioArquivo(IInformacaoRepositorioArquivo informacaoRepositorio);


    }


}