using Snebur.Utilidade;
using System;
using System.Web;

namespace Snebur.ServicoArquivo
{
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

        
        protected string RetornarString(string chave)
        {
            if (!String.IsNullOrWhiteSpace(this.HttpContext.Request.Headers[chave]))
            {
                var valor = this.HttpContext.Request.Headers[chave];
                if (!String.IsNullOrEmpty(valor))
                {
                    return Base64Util.Decode(valor);
                }
            }
            return null;
        }

        protected long RetornarLong(string chave)
        {
            string valor = this.RetornarString(chave);
            long resultado;
            if (valor != null && Int64.TryParse(valor.ToString(), out resultado) && resultado > 0)
            {
                return resultado;
            }
            return -1;
        }

        protected int RetornarInteger(string chave)
        {
            string valor = this.RetornarString(chave);
            int resultado;
            if (valor != null && Int32.TryParse(valor, out resultado) && resultado > 0)
            {
                return resultado;
            }
            return 0;
        }

        protected decimal RetornarDecimal(string chave)
        {
            string valor = this.RetornarString(chave);
            decimal resultado;
            if (valor != null && Decimal.TryParse(valor, out resultado) && resultado > 0)
            {
                return resultado;
            }
            return 0;
        }

        protected double RetornarDouble(string chave)
        {
            string valor = this.RetornarString(chave);
            double resultado;
            if (valor != null && Double.TryParse(valor, out resultado) && resultado > 0)
            {
                return resultado;
            }
            return 0;
        }

        protected System.DateTime? RetornarData(string chave)
        {
            object valor = this.RetornarString(chave);
            System.DateTime resultado;
            if (valor != null && Util.IsDate(valor) && System.DateTime.TryParse((string)valor, out resultado))
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

        protected Guid RetornarGuid(string chave)
        {
            var valor = this.RetornarString(chave);
            if (!String.IsNullOrEmpty(valor) && Guid.TryParse(valor, out var resultado) && resultado != Guid.Empty)
            {
                return resultado;
            }
            return Guid.Empty;
        }
        //RetornarTamanho
    }
}