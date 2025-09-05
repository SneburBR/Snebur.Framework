#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif  

namespace Snebur.ServicoArquivo;

public class Cabecalho
{
    private HttpContext HttpContext { get; set; }

    public Cabecalho(HttpContext context)
    {
        this.HttpContext = context;
        if (context == null)
        {
            throw new ErroNaoDefinido(" ZyoncHttpContext.Current não foi definido");
        }
    }

    protected string? RetornarString(string chave)
    {
        if (!String.IsNullOrWhiteSpace(this.HttpContext.Request.Headers[chave]))
        {
            var valor = Uri.UnescapeDataString(this.HttpContext.Request.Headers[chave].ToString());
            if (!String.IsNullOrEmpty(valor))
            {
                return Base64Util.Decode(valor);
            }
        }
        return null;
    }

    protected long RetornarLong(string chave)
    {
        var valor = this.RetornarString(chave);
        long resultado;
        if (valor != null && Int64.TryParse(valor.ToString(), out resultado) && resultado > 0)
        {
            return resultado;
        }
        return -1;
    }

    protected int RetornarInteger(string chave)
    {
        var valor = this.RetornarString(chave);
        int resultado;
        if (valor != null && Int32.TryParse(valor, out resultado) && resultado > 0)
        {
            return resultado;
        }
        return 0;
    }

    protected decimal RetornarDecimal(string chave)
    {
        var valor = this.RetornarString(chave);
        decimal resultado;
        if (valor != null && Decimal.TryParse(valor, out resultado) && resultado > 0)
        {
            return resultado;
        }
        return 0;
    }

    protected double RetornarDouble(string chave)
    {
        var valor = this.RetornarString(chave);
        double resultado;
        if (valor != null && Double.TryParse(valor, out resultado) && resultado > 0)
        {
            return resultado;
        }
        return 0;
    }

    protected DateTime? RetornarData(string chave)
    {
        var valor = this.RetornarString(chave);
        DateTime resultado;
        if (valor != null && Util.IsDate(valor) && DateTime.TryParse(valor, out resultado))
        {
            return resultado;
        }
        return null;
    }

    protected bool RetornarBoolean(string chave)
    {
        var valor = this.RetornarString(chave);
        bool resultado;
        if (valor != null && Boolean.TryParse((string)valor, out resultado))
        {
            return resultado;
        }
        return false;
    }

    protected Guid? RetornarGuid(string chave)
    {
        var valor = this.RetornarString(chave);
        if (!String.IsNullOrEmpty(valor) && Guid.TryParse(valor, out var resultado) && resultado != Guid.Empty)
        {
            return resultado;
        }
        return null;
    }
    //RetornarTamanho
}