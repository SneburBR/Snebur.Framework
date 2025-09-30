#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Http;
using Snebur.Helpers;
#endif

namespace Snebur.ServicoArquivo;

public abstract class BaseFonteStream<TArquivoFonte> : IHttpHandler where TArquivoFonte : Entidade
{
    public bool IsReusable => true;

#if NET6_0_OR_GREATER

    public virtual async Task ProcessRequestAsync(HttpContext httpContext)
    {
        var response = httpContext.Response;
        var caminhoFonte = this.RetornarCaminhoArquivoFonte(httpContext);
        if (!String.IsNullOrEmpty(caminhoFonte) &&
            File.Exists(caminhoFonte))
        {
            var mineTypeFonte = this.RetornarMineTypeFonte(httpContext);
            response.ContentType = mineTypeFonte;

            var formatoFonte = (EnumFormatoArquivoFonte)Convert.ToInt32(this.RetornarValorParametro(ConstantesServicoFonte.NOME_FORMATO_FONTE, httpContext));
            response.Headers.Append("Content-Disposition", $"attachment; filename=\"fonte.{formatoFonte}\"");

            using (var fs = StreamUtil.OpenRead(caminhoFonte))
            {
                await StreamUtil.SalvarStreamBufferizadaAsync(fs, response.Body);
            }
        }
        else
        {
            response.StatusCode = 500;
        }
    }

#else
    public virtual void ProcessRequest(HttpContext context)

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
#endif

    public string? RetornarCaminhoArquivoFonte(HttpContext httpContext)
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

    protected long RetornarIdArquivoFonte(HttpContext httpContext)
    {
        var idArquivoFonte = Convert.ToInt64(this.RetornarValorParametro(ConstantesServicoArquivo.ID_ARQUIVO, httpContext));
        if (!(idArquivoFonte > 0))
        {
            throw new Exception(String.Format("Parâmetro '{0}' não foi definido.", ConstantesServicoArquivo.ID_ARQUIVO));
        }
        return idArquivoFonte;
    }

    private string RetornarMineTypeFonte(HttpContext httpContext)
    {
        var formatoFonte = (EnumFormatoArquivoFonte)Convert.ToInt32(this.RetornarValorParametro(ConstantesServicoFonte.NOME_FORMATO_FONTE, httpContext));
        if (!EnumHelpers.IsDefined(typeof(EnumFormatoArquivoFonte), formatoFonte))
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

    private string RetornarValorParametro(string parametro, HttpContext httpContext)
    {
#if NET6_0_OR_GREATER
        throw new NotImplementedException();
#else
        var parametroBase64 = Base64Util.Encode(parametro);
        return Base64Util.Decode(httpContext.Request.QueryString[parametroBase64]);
#endif
    }
    #endregion

    protected abstract string RetornarRepositoArquivos(IInformacaoRepositorioArquivo informacaoRepositorio);

}