using Snebur.Dominio;
using System;
using System.IO;

namespace Snebur.Utilidade
{
    public static class FormatacaoUtil
    {
        public static string FormtarDecimal(double valor, int digitos = 2)
        {
            string formatar = "0.00";

            if (digitos != 2)
            {
                formatar = "0";
                if (digitos > 0)
                {
                    formatar += ".";
                }
                for (var i = 0; i < digitos; i++)
                {
                    formatar += "0";
                }
            }
            var ret = valor.ToString(formatar);
            return ret;
        }

        public static string FormatarBytes(long totalBytes)
        {
            return FormatarByteUtil.Formatar(totalBytes);
        }

        public static string FormatarBytes(long totalBytes, EnumFormatacaoBytes formato)
        {
            return FormatarByteUtil.Formatar(totalBytes, formato);
        }

        public static string FormatarTipoQualificado(string nomeTipoQualificado)
        {
            var posicao = nomeTipoQualificado.IndexOf(", Version=");
            return nomeTipoQualificado.Substring(0, posicao);
        }

        public static string FormatarNomeArquivoParaWeb(string nomeArquivo, int maximoCaracteres = 35)
        {
            if (maximoCaracteres < 20)
            {
                throw new ErroNaoSuportado(String.Format("O máximo de caracteres deve ser superior a 20"));
            }
            var nomeArquivoFormatado = nomeArquivo;
            //var nomeArquivoAtual = nomeArquivoFormatado.ToLower();

            while (nomeArquivoFormatado.Contains("  "))
            {
                nomeArquivoFormatado = nomeArquivoFormatado.Replace("  ", " ");
            }
            nomeArquivoFormatado = nomeArquivoFormatado.Replace(" ", "_");
            nomeArquivoFormatado = TextoUtil.RemoverAcentos(nomeArquivoFormatado.ToLower());
            nomeArquivoFormatado = TextoUtil.RemoverCaracteresEspecial(nomeArquivoFormatado, '_');

            nomeArquivoFormatado = TextoUtil.RemoverCaracteres(nomeArquivoFormatado, Path.GetInvalidFileNameChars());

            if (nomeArquivoFormatado.Length > maximoCaracteres)
            {
                nomeArquivoFormatado = String.Format("{0}...{1}", nomeArquivoFormatado.Substring(0, maximoCaracteres - 15),
                                                                  nomeArquivoFormatado.Substring(nomeArquivoFormatado.Length - 10, 10));
            }
            nomeArquivoFormatado = nomeArquivoFormatado.Trim();
            return nomeArquivoFormatado;
        }

        public static string FormatarNome(string nome)
        {
            return FormatacaoNomeUtil.FormatarNome(nome);
        }
        public static (string, string) FormatarNomeSobreNome(string nomeCompleto)
        {
            return FormatacaoNomeUtil.FormatarNomeSobrenome(nomeCompleto);
            //string retornarSobreNome(List<string> partes)
            //{
            //    if (partes.Count > 1)
            //    {
            //        return String.Join(" ", partes.Skip(1).Select(x => TextoUtil.RetornarPrimeiraLetraMaiuscula(x)));
            //    }
            //    return String.Empty;
            //}

            //if (!String.IsNullOrEmpty(nomeCompleto))
            //{
            //    var partes = nomeCompleto.Trim().Split(' ').Where(x => !String.IsNullOrWhiteSpace(x)).ToList();
            //    var nome = TextoUtil.RetornarPrimeiraLetraMaiuscula(partes.First());
            //    var sobrenome = retornarSobreNome(partes);
            //    return (nome, sobrenome);

            //}
            //return (String.Empty, String.Empty);
        }

        public static string FormatarMoeda(decimal valorPedidoInterface)
        {
            return String.Format(ConfiguracaoUtil.CulturaPortuguesBrasil, "{0:C}", valorPedidoInterface);
        }

        public static string FormatarDimensao(Dimensao dimensao)
        {
            return $"{dimensao.Largura.ToString("0.0")} x {dimensao.Altura.ToString("0.0")}";
        }

        public static string FormatarDimensaoPixels(Dimensao dimensao)
        {
            return $"{dimensao.Largura.ToString("0")} x {dimensao.Altura.ToString("0")}";
        }

        public static string FormatarMilimetros(double espessura)
        {
            return $"{espessura.ToString("0.0")} mm";
        }

        public static string FormatarCpfCnpj(string cpfCnpj, bool isIgnoarErro = false)
        {
            var cpfCnpjNumeros = TextoUtil.RetornarSomenteNumeros(cpfCnpj);

            if (cpfCnpjNumeros.Length == 11)
            {
                return String.Format(@"{0:000\.000\.000-00}", Convert.ToInt64(cpfCnpjNumeros));
            }

            if (cpfCnpjNumeros.Length == 14)
            {
                return String.Format(@"{0:00\.000\.000/0000-00}", Convert.ToInt64(cpfCnpjNumeros));
            }
            if (isIgnoarErro)
            {
                return cpfCnpjNumeros;
            }

            throw new Exception(String.Format("O valor {0} não está em um formato de CPF ou CNPJ válido.", cpfCnpj));
        }

    }
}