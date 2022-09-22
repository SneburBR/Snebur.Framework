﻿using Snebur.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Snebur.Utilidade
{
    public static partial class TextoUtil
    {
        public static HashSet<char> Numeros => TextoUtilConstantes.Numeros;
        public static HashSet<char> Letras => TextoUtilConstantes.Letras;
        public static HashSet<char> LetrasNumeros => TextoUtilConstantes.LetrasNumeros;
        public static HashSet<char> CaracteresPadrao => TextoUtilConstantes.CaracteresPadrao;
        public static HashSet<char> LinhasTabulacoes => TextoUtilConstantes.LinhasTabulacoes;

        public static HashSet<char> PontosSinais => TextoUtilConstantes.PontosSinais;

        public static string RemoverAcentos(string texto)
        {
            if (texto == null)
            {
                return null;
            }
            var sb = new StringBuilder();
            var acentosMapeado = TextoUtilConstantes.AcentosMapeado;

            for (int i = 0; i < texto.Length; i++)
            {
                var caracter = texto[i];
                if (acentosMapeado.ContainsKey(caracter))
                {
                    sb.Append(acentosMapeado[caracter]);
                }
                else
                {
                    sb.Append(caracter);
                }
            }
            return sb.ToString();
            //return texto;
        }

        public static string RemoverAcentosObsoleto(string texto)
        {
            string[] letras = { "a", "e", "i", "o", "u", "c", "n", "A", "E", "I", "O", "U", "C", "N" };
            string[] acentos = { "(à|á|â|ã|ä){1}", "(è|é|ê|ë){1}", "(ì|í|î|ï){1}", "(ò|ó|ô|õ|ö){1}", "(ù|ú|û|ü){1}", "ç{1}", "ñ{1}", "(À|Á|Â|Ã){1}", "(È|É|Ê){1}", "(Ì|Í|Î){1}", "(Ò|Ó|Ô|Õ){1}", "(Ù|Ú|Û|Ü){1}", "Ç{1}", "Ñ{1}" };

            for (int i = 0; i < letras.Length; i++)
            {
                texto = System.Text.RegularExpressions.Regex.Replace(texto, acentos[i], letras[i]);
            }
            return texto;
        }

        public static List<string> RetornarIntervaloLinhas(List<string> linhas, string contemInicio, string contemFim)
        {
            var retorno = new List<string>();
            foreach (var linha in linhas)
            {
                if (linha.Contains(contemInicio) || retorno.Count > 0)
                {
                    retorno.Add(linha);
                }
                if (linha.Contains(contemFim))
                {
                    break;
                }
            }
            return retorno;
        }

        public static bool IsSomenteTexto(string palavra)
        {
            foreach (var c in palavra)
            {
                if (IsNumero(c))
                {
                    return false;
                }
            }
            return true;
        }

        internal static string RetornarStringUTF8(string texto)
        {
            var bytes = Encoding.Default.GetBytes(texto);
            return Encoding.UTF8.GetString(bytes);

            //var bytesString = new byte[texto.Length];
            //for (int ix = 0; ix < texto.Length; ++ix)
            //{
            //    char caracter = texto[ix];
            //    bytesString[ix] = (byte)caracter;
            //}
            //return Encoding.UTF8.GetString(bytesString, 0, texto.Length);
        }

        public static string[] DividirLetraMaiuscula(string descricao)
        {
            var partes = new List<string>();
            var parteAtual = new StringBuilder();
            var len = descricao.Length;
            for (var i = 0; i < len; i++)
            {
                var caracter = descricao[i];
                var isFim = (i == (len - 1));
                if (Char.IsUpper(caracter) || isFim)
                {
                    if (isFim)
                    {
                        parteAtual.Append(caracter);
                    }
                    var parte = parteAtual.ToString();
                    if (!String.IsNullOrEmpty(parte))
                    {
                        partes.Add(parte);
                    }
                    parteAtual.Clear();
                }
                parteAtual.Append(caracter);
            }
            return partes.ToArray();
        }
        /// <summary>
        /// Remove os caracteres especiais de uma string
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="substituirCaracteres">Substituro por underline</param>
        public static string RemoverCaracteresEspecial(string texto, char? substituirPor = null)
        {
            if (String.IsNullOrEmpty(texto))
            {
                return texto;
            }
            return TextoUtil.RetornarTextoCaracteresPermitido(texto, TextoUtil.CaracteresPadrao, true, substituirPor);
        }

        public static string RemoverCaracteres(string texto, string caracteresRemover)
        {
            return TextoUtil.RemoverCaracteres(texto, caracteresRemover.ToArray().ToHashSet());
        }

        public static string RemoverCaracteres(string texto, char[] caracteresRemover)
        {
            return TextoUtil.RemoverCaracteres(texto, caracteresRemover.ToHashSet());
        }

        public static string RemoverCaracteres(string texto, HashSet<char> caracteresRemover)
        {
            return TextoUtil.RemoverCarecteres(texto, caracteresRemover, null);
        }

        public static string RemoverCarecteres(string texto, HashSet<char> caracteresRemover, char? substituirPor = null)
        {
            if (String.IsNullOrEmpty(texto))
            {
                return texto;
            }
            var sb = new StringBuilder();
            for (int i = 0; i < texto.Length; i++)
            {
                var c = texto[i];
                if (!caracteresRemover.Contains(c))
                {
                    sb.Append(c);
                }
                else
                {
                    if (substituirPor.HasValue)
                    {
                        sb.Append(substituirPor.Value);
                    }
                }
            }
            return sb.ToString();
        }

        public static string RetornarSomenteLetras(string texto, IEnumerable<char> extra)
        {
            if (extra.Count() > 0)
            {
                var filtro = RetoranrFiltroCache(TextoUtil.Letras, extra);
                var isContemEspaco = extra.Contains(' ');
                return TextoUtil.RetornarTextoCaracteresPermitido(texto, filtro, isContemEspaco);
            }
            return TextoUtil.RetornarTextoCaracteresPermitido(texto, TextoUtil.Letras, false);

        }

        public static string RetornarSomenteLetras(string texto)
        {
            return TextoUtil.RetornarTextoCaracteresPermitido(texto, TextoUtil.Letras, false);
        }

        public static string RetornarSomenteNumeros(string texto)
        {
            return TextoUtil.RetornarTextoCaracteresPermitido(texto, TextoUtil.Numeros, false);
        }

        public static string RetornarSomenteNumeros(object cep)
        {
            throw new NotImplementedException();
        }

        public static string RetornarSomenteNumeros(string texto, bool isAceitarVirgual)
        {
            var caracterExtra = (isAceitarVirgual) ? "," : String.Empty;
            return TextoUtil.RetornarSomenteNumeros(texto, caracterExtra);
        }

        public static string RetornarSomenteNumeros(string texto, char caracterExtra)
        {
            return TextoUtil.RetornarSomenteNumeros(texto, new char[] { caracterExtra });
        }

        public static string RemoverCaracteresInicial(string texto, string caracter)
        {
            while (texto.StartsWith(caracter))
            {
                texto = texto.Substring(caracter.Length);
            }
            return texto;
        }

        public static string RetornarSomenteNumeros(string texto, string caracteresExtra)
        {
            return TextoUtil.RetornarSomenteNumeros(texto, caracteresExtra.ToCharArray());
        }

        public static string RetornarSomenteNumeros(string texto, IEnumerable<char> caractesExtras, bool isPermitirEspacoBranco = false)
        {
            if (caractesExtras?.Count() > 0)
            {
                var filtro = RetoranrFiltroCache(TextoUtil.Numeros, caractesExtras);
                var isContemEspaco = caractesExtras.Contains(' ');
                return TextoUtil.RetornarTextoCaracteresPermitido(texto, filtro, isContemEspaco || isPermitirEspacoBranco);
            }
            return TextoUtil.RetornarTextoCaracteresPermitido(texto, TextoUtil.Numeros, isPermitirEspacoBranco);
        }

        public static string RetornarSomentesLetrasNumeros(string texto,
                                                           bool isPermitirEspacoBranco = true,
                                                           IEnumerable<char> caractesExtras = null)
        {
            if (caractesExtras?.Count() > 0)
            {
                var filtro = RetoranrFiltroCache(TextoUtil.LetrasNumeros, caractesExtras);
                var isContemEspaco = caractesExtras.Contains(' ');
                return TextoUtil.RetornarTextoCaracteresPermitido(texto, filtro, isContemEspaco || isPermitirEspacoBranco);
            }
            return TextoUtil.RetornarTextoCaracteresPermitido(texto, TextoUtil.LetrasNumeros, isPermitirEspacoBranco);
        }
        public static int RetornarOcorrenciasParteTexto(string texto, string letra)
        {
            var cont = 0;
            var quant = 0;
            var pos = -1;
            var pos_ant = -1;

            while (cont < texto.Length)
            {
                pos = texto.IndexOf(letra, cont);
                if (pos != pos_ant && pos != -1)
                {
                    quant += 1;
                }
                cont += 1;
                pos_ant = pos;
            }
            return quant;
        }

        public static int RetornarOcorrenciasLetraTexto(string texto, char letra)
        {
            var cont = 0;
            var quant = 0;
            var pos = -1;
            var pos_ant = -1;

            while (cont < texto.Length)
            {
                pos = texto.IndexOf(letra, cont);
                if (pos != pos_ant && pos != -1)
                {
                    quant += 1;
                }
                cont += 1;
                pos_ant = pos;
            }
            return quant;
        }
        //public static string RetornarPrimeiroNome(string nomeCompleto)
        //{
        //    var nomes = nomeCompleto.Trim().Split(" ".ToCharArray());
        //    if (nomes.Any(x => x.Length > 2))
        //    {
        //        return TextoUtil.RetornaNomeFormatadoPrimeiraLetraMaiuscula(nomes.Where(x => x.Length > 2).First());
        //    }
        //    else
        //    {
        //        return nomes.First();
        //    }
        //}

        //public static string RetornaNomeFormatadoPrimeiraLetraMaiuscula(string nomeCompleto)
        //{
        //    if (nomeCompleto != null & nomeCompleto.Trim().Length > 0)
        //    {
        //        nomeCompleto = nomeCompleto.Trim().ToLower();
        //        if (nomeCompleto.Contains(" "))
        //        {
        //            var partes = nomeCompleto.Split(" ".ToCharArray());

        //            System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //            foreach (string nome in partes)
        //            {
        //                if (nome.Trim().Length > 0)
        //                {
        //                    sb.Append(" ");
        //                    if (nome.Length > 2)
        //                    {
        //                        sb.Append(TextoUtil.RetornarPrimeiraLetraMaiuscula(nome));
        //                    }
        //                    else if (nome.Length == 2 & nome.Last() == Convert.ToChar("."))
        //                    {
        //                        sb.Append(TextoUtil.RetornarPrimeiraLetraMaiuscula(nome));
        //                    }
        //                    else if (nome.Length == 1)
        //                    {
        //                        sb.Append(TextoUtil.RetornarPrimeiraLetraMaiuscula(nome));
        //                    }
        //                    else
        //                    {
        //                        sb.Append(nome);
        //                    }
        //                }
        //            }
        //            return sb.ToString().Trim();
        //        }
        //        else
        //        {
        //            return RetornarPrimeiraLetraMaiuscula(nomeCompleto);
        //        }
        //    }
        //    else
        //    {
        //        return String.Empty;
        //    }
        //}

        public static string RetornarPrimeiraLetraMaiuscula(string texto)
        {
            var primeiraLetra = texto.Substring(0, 1).ToUpper();
            var restante = texto.Substring(1, texto.Length - 1);
            return String.Concat(primeiraLetra, restante);
        }

        public static string RetornarPrimeiraLetraMinusculo(string texto)
        {
            var primeiraLetra = texto.Substring(0, 1).ToLower();
            var restante = texto.Substring(1, texto.Length - 1);
            return String.Concat(primeiraLetra, restante);
        }

        public static string RetornarInicioMinusculo(string texto, int quantidadeCaracteres)
        {
            if (quantidadeCaracteres > texto.Length)
            {
                quantidadeCaracteres = texto.Length;
            }
            var parteMinuscula = texto.Substring(0, quantidadeCaracteres).ToLower();
            var restante = texto.Substring(quantidadeCaracteres, texto.Length - quantidadeCaracteres);
            return String.Concat(parteMinuscula, restante);
        }

        public static string RemoverIdentacao(string texto)
        {
            throw new ErroNaoImplementado();
            //texto = string.Concat(texto, string.Empty);
            //texto = texto.Replace(Constants.vbCrLf, string.Empty);
            //texto = texto.Replace(Constants.vbCr, string.Empty);
            //texto = texto.Replace(Constants.vbLf, string.Empty);
            //texto = texto.Replace(System.Environment.NewLine, string.Empty);
            //texto = texto.Replace(Constants.vbTab, string.Empty);
            //texto = texto.Replace(" ", string.Empty);
            //return texto;
        }

        public static string RemoverEspacoDuplo(string texto)
        {
            //var resultado = texto;
            //while (resultado.Contains("  resultado"))
            //{
            //    resultado = resultado.Replace("  ", " ");
            //}
            //return resultado;
            var sb = new StringBuilder();
            bool continuar = false;
            for (int i = 0; i < texto.Length; i++)
            {
                var caracter = texto[i];
                if (Char.IsWhiteSpace(caracter))
                {
                    if (continuar)
                    {
                        continue;
                    }
                    continuar = true;
                }
                else
                {
                    continuar = false;
                }
                sb.Append(caracter);
            }
            var resultado = sb.ToString();
            return resultado.Trim();
        }

        public static string RetornarPrimeirosCaracteres(string texto, int numeroCaracteres)
        {
            if (texto.Length <= numeroCaracteres)
            {
                return texto;
            }
            return texto.Substring(0, numeroCaracteres);
        }

        public static string RetornarPrimeirosCaracteres(string texto, int numeroCaracteres, bool removerLinhasTabulacoes)
        {
            var resultado = TextoUtil.RetornarPrimeirosCaracteres(texto, numeroCaracteres);
            if (removerLinhasTabulacoes)
            {
                return TextoUtil.RemoverLinhasTabulacoes(resultado);
            }
            return resultado;
        }

        public static string RetornarUtlimosCaracteres(string texto, int numeroCaracteres)
        {
            if (texto.Length <= numeroCaracteres)
            {
                return texto;
            }
            return texto.Substring(texto.Length - numeroCaracteres);
        }

        public static List<string> RetornarLinhas(string texto, bool ignorarLinhasBranco = false)
        {
            if (texto == null)
            {
                return null;
            }
            var linhas = new List<string>();
            using (var sr = new StringReader(texto))
            {
                while (true)
                {
                    var linha = sr.ReadLine();
                    if (linha == null)
                    {
                        break;
                    }
                    if (!ignorarLinhasBranco || !String.IsNullOrWhiteSpace(texto))
                    {
                        linhas.Add(linha);
                    }
                }
            }
            return linhas;
        }

        public static string RemoverLinhas(string texto)
        {
            return TextoUtil.RemoverLinhas(texto, null);
        }

        public static string RemoverLinhas(string texto, char? caracterSeparador)
        {
            var sepearador = (caracterSeparador.HasValue) ? caracterSeparador.Value.ToString() : String.Empty;
            return String.Join(sepearador, TextoUtil.RetornarLinhas(texto));
        }

        public static string RemoverLinhasTabulacoes(string texto)
        {
            return TextoUtil.RemoverCaracteres(texto, LinhasTabulacoes);
        }

        public static string RemoverQuebraLinhas(string str, bool removerEspacosDuplo)
        {
            var sb = new StringBuilder();
            using (var sr = new StringReader(str))
            {
                while (true)
                {
                    var linha = sr.ReadLine();
                    if (linha == null)
                    {
                        break;
                    }
                    if (removerEspacosDuplo)
                    {
                        linha = TextoUtil.RemoverEspacoDuplo(linha);
                    }
                    if (linha.Length > 0 && !Char.IsWhiteSpace(linha.Last()))
                    {
                        sb.Append(' ');
                    }
                    sb.Append(linha);

                }
            }
            return sb.ToString();
        }
        public static bool IsNumero(char caracter)
        {
            return TextoUtil.Numeros.Contains(caracter);
        }

        public static bool IsLetra(char caracter)
        {
            return TextoUtil.Numeros.Contains(caracter);
        }

        public static bool IsEspacoBranco(char caracter)
        {
            return Char.IsWhiteSpace(caracter);
        }

        public static bool IsLetraOuNumero(char caracter)
        {
            return TextoUtil.LetrasNumeros.Contains(caracter);
        }

        public static bool IsSomenteNumeros(string texto)
        {
            return TextoUtil.IsTextoValidoInterno(texto, TextoUtilConstantes.Numeros);
        }

        public static bool IsSomenteNumerosPontosSinais(string texto)
        {
            return TextoUtil.IsTextoValidoInterno(texto,
              TextoUtilConstantes.NumerosPontsSinais);
        }

        private static bool IsTextoValidoInterno(string texto, HashSet<char> caracteresPermitidoObjetos)
        {
            if (texto != null)
            {
                for (var i = 0; i < texto.Length; i++)
                {
                    var caracter = texto[i];
                    if (!caracteresPermitidoObjetos.Contains(caracter))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public static string ComprimirTexto(string texto)
        {
            var buffer = Encoding.UTF8.GetBytes(texto);
            using (var memoryStream = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gZipStream.Write(buffer, 0, buffer.Length);
                }
                memoryStream.Position = 0;

                var compressedData = new byte[memoryStream.Length];
                memoryStream.Read(compressedData, 0, compressedData.Length);

                var gZipBuffer = new byte[compressedData.Length + 4];
                Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
                return Convert.ToBase64String(gZipBuffer);
            };
        }

        public static string DescomprimirTexto(string textoComprimdo)
        {
            var gZipBuffer = Convert.FromBase64String(textoComprimdo);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }
                return Encoding.UTF8.GetString(buffer);
            }
        }
        #region  CACHE FILTROS 

        private static object _bloqueio = new object();

        private static Dictionary<string, HashSet<char>> _cacheFiltros;
        public static Dictionary<string, HashSet<char>> CacheFiltros => LazyUtil.RetornarValorLazyComBloqueio(ref _cacheFiltros, () => new Dictionary<string, HashSet<char>>());
        private static HashSet<char> RetoranrFiltroCache(HashSet<char> filtroBase,
                                                          IEnumerable<char> caractesExtras)
        {
            var chave = $"{filtroBase.GetHashCode()}-{String.Join("-", caractesExtras.Select(x => x.GetHashCode()))}";

            if (!CacheFiltros.ContainsKey(chave))
            {
                lock (_bloqueio)
                {
                    if (!CacheFiltros.ContainsKey(chave))
                    {
                        var filtro = filtroBase.ToHashSet();
                        filtro.AddRange(caractesExtras);
                        CacheFiltros.Add(chave, filtro);
                    }
                }
            }
            return CacheFiltros[chave];
        }

        public static string RetornarTextoCaracteresPermitido(string texto, HashSet<char> caracterPermitidos, bool isPermitirEspacoBranco, char? substituirPor = null)
        {
            if (String.IsNullOrEmpty(texto))
            {
                return null;
            }
            ErroUtil.ValidarReferenciaNula(caracterPermitidos, nameof(caracterPermitidos));

            var sb = new StringBuilder();
            for (int i = 0; i < texto.Length; i++)
            {
                var caracter = texto[i];
                if (isPermitirEspacoBranco && Char.IsWhiteSpace(caracter))
                {
                    sb.Append(caracter);
                    continue;
                }
                if (caracterPermitidos.Contains(caracter))
                {
                    sb.Append(caracter);
                }
                else
                {
                    if (substituirPor.HasValue)
                    {
                        sb.Append(substituirPor);
                    }
                }
            }
            return sb.ToString();
        }
        #endregion
    }
}