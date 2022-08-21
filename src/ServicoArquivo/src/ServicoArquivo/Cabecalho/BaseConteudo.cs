//using System;
//using System.Linq;
//using System.Text;
//using System.Collections.Generic;
//using System.Web;
//using System.IO;
//using Snebur.Utilidade;

//namespace Snebur.Seguranca
//{

//    public abstract class BaseConteudo
//    {

//        public BaseConteudo()
//        {
//        }

//        public Dictionary<string, string> Dicionario { get; set; }

//        public BaseConteudo(System.IO.Stream inputStream)
//        {
//            if (inputStream != null)
//            {
//                using (inputStream)
//                {
//                    using (StreamReader sr = new StreamReader(inputStream))
//                    {
//                        string _conteudo = sr.ReadToEnd();
//                        this.Dicionario = UriUtil.RetornarPartesConsulta(_conteudo);
//                    }
//                }
//            }
//        }

//        protected string RetornarString(string chave, HttpContext context)
//        {

//            if (context == null)
//            {
//                context = HttpContext.Current;
//            }

//            if (context != null)
//            {
//                if (!string.IsNullOrWhiteSpace(context.Request.QueryString[chave]))
//                {
//                    return context.Request.QueryString[chave];
//                }

//                if (!string.IsNullOrWhiteSpace(context.Request.Headers[chave]))
//                {
//                    return context.Request.Headers[chave];
//                }

//                if (this.Dicionario != null)
//                {
//                    return this.Dicionario.GetValueOrDefault(chave);
//                }

//            }
//            return null;

//        }

//        protected long RetornarLong(string chave, HttpContext context)
//        {
//            string valor = this.RetornarString(chave, context);
//            long resultado;
//            if (valor != null && long.TryParse(valor.ToString(), out resultado) && resultado > 0)
//            {
//                return resultado;
//            }
//            return -1;
//        }

//        protected int RetornarInteger(string chave, HttpContext context)
//        {
//            string valor = this.RetornarString(chave, context);
//            int resultado;
//            if (valor != null && int.TryParse(valor, out resultado) && resultado > 0)
//            {
//                return resultado;
//            }
//            return 0;
//        }

//        protected decimal RetornarDecimal(string chave, HttpContext context)
//        {
//            string valor = this.RetornarString(chave, context);
//            decimal resultado;
//            if (valor != null && decimal.TryParse(valor, out resultado) && resultado > 0)
//            {
//                return resultado;
//            }
//            return 0;
//        }

//        protected System.DateTime? RetornarData(string chave, HttpContext context)
//        {
//            object valor = this.RetornarString(chave, context);
//            System.DateTime resultado;
//            if (valor != null && Util.IsDate(valor) && System.DateTime.TryParse((string)valor, out resultado))
//            {
//                return resultado;
//            }
//            return null;
//        }

//        protected bool RetornarBoolean(string chave, HttpContext context)
//        {
//            var valor = this.RetornarString(chave, context);
//            bool resultado;
//            if (valor != null && bool.TryParse((string)valor, out resultado))
//            {
//                return resultado;
//            }
//            return false;
//        }

//        protected Guid? RetornarGuid(string chave, HttpContext context)
//        {
//            var valor = this.RetornarString(chave, context);
//            Guid resultado;
//            if (!String.IsNullOrEmpty(valor) && Guid.TryParse(valor, out resultado) && resultado != Guid.Empty)
//            {
//                return resultado;
//            }
//            return null;
//        }

//        //RetornarTamanho

//    }
//}