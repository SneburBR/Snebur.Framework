using System;
using System.IO;
using System.Web;
using Snebur.Dominio;
using Snebur.Net;
using Snebur.Utilidade;

#if NET50
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
#endif

namespace Snebur.ServicoArquivo
{
    public abstract class BaseFonteStream<TArquivoFonte> : IHttpHandler where TArquivoFonte : Entidade
    {
        public bool IsReusable => true;

#if NET50
        public Task  ProcessRequestAsync(HttpContext context)
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
            var response = context.Response;
            var caminhoFonte = this.RetornarCaminhoArquivoFonte(context);
            if (!String.IsNullOrEmpty(caminhoFonte) && File.Exists(caminhoFonte))
            {
                var mineTypeFonte = this.RetornarMineTypeFonte(context);
                response.WriteFile(caminhoFonte);
                response.ContentType = mineTypeFonte;
            }
            else
            {
                response.SubStatusCode = 5;
                response.StatusCode = 500;
            }
        }


        public string RetornarCaminhoArquivoFonte(ZyonHttpContext httpContext)
        {
            var idArquivoFonte = this.RetornarIdArquivoFonte(httpContext);
            if (idArquivoFonte > 0)
            {
                var diretorioFonte = this.RetornarDiretorioArquivo(idArquivoFonte);
                return ServicoArquivoUtil.RetornarCaminhoCompletoArquivo(diretorioFonte, idArquivoFonte);
            }
            return null;
        }

        public string RetornarDiretorioArquivo(long idArquivo)
        {
            var info = new InformacaoRepositorioArquivo(idArquivo, typeof(TArquivoFonte).Name);
            var repositorioArquvos = this.RetornarRepositoArquivos(info);
            return ServicoArquivoUtil.RetornarCaminhoDiretorioArquivo(repositorioArquvos, idArquivo);
        }

        #region Métodos privados

        protected long RetornarIdArquivoFonte(ZyonHttpContext httpContext)
        {
            var idArquivoFonte = Convert.ToInt64(this.RetornarValorParametro(ConstantesServicoArquivo.ID_ARQUIVO, httpContext));
            if (!(idArquivoFonte > 0))
            {
                throw new Exception(String.Format("Parâmetro '{0}' não foi definido.", ConstantesServicoArquivo.ID_ARQUIVO));
            }
            return idArquivoFonte;
        }

        private string RetornarMineTypeFonte(ZyonHttpContext httpContext)
        {
            var formatoFonte = (EnumFormatoArquivoFonte)Convert.ToInt32(this.RetornarValorParametro(ConstantesServicoFonte.NOME_FORMATO_FONTE, httpContext));
            if (!Enum.IsDefined(typeof(EnumFormatoArquivoFonte), formatoFonte))
            {
                throw new Exception(String.Format("Parâmetro '{0}' não suportado.", ConstantesServicoFonte.NOME_FORMATO_FONTE));
            }
            switch (formatoFonte)
            {
                case EnumFormatoArquivoFonte.Ttf:

                    return "application/font-ttf";

                case EnumFormatoArquivoFonte.Woff:

                    return "application/font-woff";

                case EnumFormatoArquivoFonte.Woff2:

                    return "application/font-woff2";

                case EnumFormatoArquivoFonte.Svg:

                    return "image/svg-xml";

                case EnumFormatoArquivoFonte.Eot:

                    return "application/font-opentype";

                default:

                    throw new Erro("Formato da fonte não é suportado");

            }

        }

        private string RetornarValorParametro(string parametro, ZyonHttpContext zyonHttpContext)
        {
            var parametroBase64 = Base64Util.Encode(parametro);
            return Base64Util.Decode(zyonHttpContext.Request.QueryString[parametroBase64]);
        }
        #endregion

        protected abstract string RetornarRepositoArquivos(IInformacaoRepositorioArquivo informacaoRepositorio);

        
    }


}