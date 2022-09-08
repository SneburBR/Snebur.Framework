using Snebur.Utilidade;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace System
{
    public static class StringExtensao
    {
        public static string RetornarInicio(this string str, string seperador, bool isIncluirSeperador = false)
        {
            if (str.Contains(seperador))
            {
                var adicionar = isIncluirSeperador ? seperador.Length : 0;
                var fim = str.IndexOf(seperador) + adicionar;
                return str.Substring(0, fim);
            }
            return str;
        }

        public static string RetornarFim(this string str, string seperador, bool isIncluirSeperador = false)
        {
            if (str.Contains(seperador))
            {
                var remover = isIncluirSeperador ? seperador.Length : 0;
                var inicio = str.LastIndexOf(seperador) - remover;
                return str.Substring(inicio);
            }
            return str;
        }
        public static string RetornarPrimeirosCaracteres(this string str, int numeroCaracteres)
        {
            return TextoUtil.RetornarPrimeirosCaracteres(str, numeroCaracteres);
        }

        public static string RetornarPrimeirosCaracteres(this string str, int numeroCaracteres, bool removerLinhasTabulacoes)
        {
            return TextoUtil.RetornarPrimeirosCaracteres(str, numeroCaracteres, removerLinhasTabulacoes);
        }

        public static string RetornarUtlimosCaracteres(this string str, int numeroCaracteres)
        {
            return TextoUtil.RetornarUtlimosCaracteres(str, numeroCaracteres);
        }

        public static List<string> ToLines(this string str, bool isIgnorarLinhasBranco = false)
        {
            return TextoUtil.RetornarLinhas(str, isIgnorarLinhasBranco);
        }

        public static string RemoverQuebraLinhas(this string str, bool removerEspacosDuplo = true)
        {
            return TextoUtil.RemoverQuebraLinhas(str, removerEspacosDuplo);

        }

        public static int TotalLinhas(this string str)
        {
            int totalLinhas = 0;
            using (var sr = new StringReader(str))
            {
                while (sr.ReadLine() != null)
                {
                    totalLinhas += 1;
                }
            }
            return totalLinhas;
        }

        public static string Replace(this string str, string[] partes, string novoValor)
        {
            string retorno = str;
            foreach (var parte in partes)
            {
                retorno = retorno.Replace(parte, novoValor);
            }
            return retorno;
        }

        public static bool Contains(this string source, string value, CompareOptions options)
        {
            return CultureInfo.InvariantCulture.CompareInfo.IndexOf(source, value, options) >= 0;
        }

        public static string NullIfEmpty(this string s)
        {
            return string.IsNullOrEmpty(s) ? null : s;
        }
        public static string NullIfWhiteSpace(this string s)
        {
            return string.IsNullOrWhiteSpace(s) ? null : s;
        }

    }

    public static class EncodingUtil
    {
        public static Encoding ISO_8859_1
        {
            get
            {
                return Encoding.GetEncoding("ISO-8859-1");
            }
        }

        public static byte[] ConverterUf8ParaIso8859(byte[] utfBytes)
        {
            return Encoding.Convert(Encoding.UTF8, ISO_8859_1, utfBytes);
        }

    }

}