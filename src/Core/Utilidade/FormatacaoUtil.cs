using Snebur.Dominio;
using Snebur.UI;
using System;
using System.Globalization;
using System.IO;

namespace Snebur.Utilidade
{
    public static class FormatacaoUtil
    {
        private const int CASAS_DECIMAL_PADRAO = 2;
        public static string Formatar(object valor, EnumFormatacao formatar)
        {
            if (valor == null)
            {
                return String.Empty;
            }

            switch (formatar)
            {
                case EnumFormatacao.Bytes:

                    return FormatacaoUtil.FormatarBytes(Convert.ToInt64(valor));

                case EnumFormatacao.Cep:
                    return FormatacaoUtil.FormatarCep(Convert.ToInt64(valor));
                case EnumFormatacao.Cpf:
                case EnumFormatacao.Cnpj:

                    return FormatacaoUtil.FormatarCpfCnpj(valor.ToString());
                case EnumFormatacao.Telefone:
                    return FormatacaoUtil.FormatarTelefone(valor.ToString());
                case EnumFormatacao.Moeda:
                    return FormatacaoUtil.FormatarMoeda(Convert.ToDecimal(valor));

                case EnumFormatacao.MoedaComSinal:
                case EnumFormatacao.Inteiro:
                    return Convert.ToInt32(valor).ToString();

                case EnumFormatacao.Decimal:

                    return FormatacaoUtil.FormatarDecimal(Convert.ToDouble(valor));
                case EnumFormatacao.Decimal1:
                    return FormatacaoUtil.FormatarDecimal(Convert.ToDouble(valor), 1);
                case EnumFormatacao.Decimal3:
                    return FormatacaoUtil.FormatarDecimal(Convert.ToDouble(valor), 3);
                case EnumFormatacao.Data:
                    return FormatacaoUtil.FormatarData(Convert.ToDouble(valor), 3);
                case EnumFormatacao.Hora:
                    return FormatacaoUtil.FormatarHora(Convert.ToDouble(valor), 3);
                case EnumFormatacao.Dimensao:
                    return FormatacaoUtil.FormatarDimensao((Dimensao)valor);
                case EnumFormatacao.DimensaoCm:
                    return FormatacaoUtil.FormatarDimensaoCm((Dimensao)valor);
                case EnumFormatacao.DimensaoPixels:
                    return FormatacaoUtil.FormatarDimensaoPixels((Dimensao)valor);
                case EnumFormatacao.DataHora:

                case EnumFormatacao.HoraDescricao:

                case EnumFormatacao.HoraDescricaoMin:
                    break;
                case EnumFormatacao.DataSemantica:
                    break;
                case EnumFormatacao.DataHoraSemantica:
                    break;
                case EnumFormatacao.DataSemanticaHora:
                    break;
                case EnumFormatacao.SimNao:
                    break;
                case EnumFormatacao.Trim:
                    break;
                case EnumFormatacao.TamanhoArquivo:
                    break;
                case EnumFormatacao.Porcentagem:
                    break;
                case EnumFormatacao.Porcentagem1:
                    break;
                case EnumFormatacao.Porcentagem2:
                    break;
                case EnumFormatacao.NaoQuebrar:
                    break;
                case EnumFormatacao.Pixel:
                    break;
                case EnumFormatacao.Tempo:
                    break;
                case EnumFormatacao.TempoSemantico:
                    break;
                case EnumFormatacao.Centimetro:
                    break;
                case EnumFormatacao.Grau:
                    break;
                case EnumFormatacao.Grau1:
                    break;
                case EnumFormatacao.Grau2:
                    break;
                case EnumFormatacao.PrimeiraLetraMaiuscula:
                    break;
                case EnumFormatacao.CaixaAlta:
                    break;
                case EnumFormatacao.CaixaBaixa:
                    break;
                case EnumFormatacao.Nome:
                    break;
                case EnumFormatacao.PositivoNegativo:
                    break;
                case EnumFormatacao.PositivoNegativoDecimal:
                    break;
                case EnumFormatacao.PortentagemPositivoNegativo:
                    break;
                case EnumFormatacao.Portentagem1PositivoNegativo:
                    break;
                case EnumFormatacao.Absoluto:
                    break;
                case EnumFormatacao.EntreParentes:
                    break;
                case EnumFormatacao.QuebrarLinhasHtml:
                    break;
                case EnumFormatacao.Peso:
                    break;
                case EnumFormatacao.Prazo:
                    break;
                default:
                    break;
            }

            throw new NotImplementedException();
        }

        private static string FormatarCep(long v)
        {
            throw new NotImplementedException();
        }

        private static string FormatarData(double v1, int v2)
        {
            throw new NotImplementedException();
        }

        private static string FormatarHora(double v1, int v2)
        {
            throw new NotImplementedException();
        }

        private static string FormatarDimensaoCm(Dimensao valor)
        {
            throw new NotImplementedException();
        }

        public static string FormatarDecimal(double? valor)
        {
            return FormatarDecimal(valor, CASAS_DECIMAL_PADRAO);
        }

        public static string FormatarDecimal(double valor)
        {
            return FormatarDecimal(valor, CASAS_DECIMAL_PADRAO);
        }
        public static string FormatarDecimal(double? valor,
                                             EnumDivisorDecimal divisorDecimal = EnumDivisorDecimal.CulturaAtual)
        {
            return FormatarDecimal(valor, CASAS_DECIMAL_PADRAO, divisorDecimal);
        }
        public static string FormatarDecimal(double valor,
                                             EnumDivisorDecimal divisorDecimal = EnumDivisorDecimal.CulturaAtual)
        {
            return FormatarDecimal(valor, CASAS_DECIMAL_PADRAO, divisorDecimal);
        }
        public static string FormatarDecimal(double? valor, 
                                             int casasDecimal,
                                             EnumDivisorDecimal divisorDecimal = EnumDivisorDecimal.CulturaAtual)
        {
            if (valor == null)
            {
                return "0";
            }
            return FormatarDecimal(valor.Value, casasDecimal, divisorDecimal);
        }

        public static string FormatarDecimal(double valor,
                                             int casasDecimal,
                                             EnumDivisorDecimal divisorDecimal= EnumDivisorDecimal.CulturaAtual)
        {
            return FormatarDecimal(Convert.ToDecimal(valor), casasDecimal, divisorDecimal);
        }
        private static string FormatarDecimal(decimal valor, 
                                              int casasDecimal, 
                                              EnumDivisorDecimal divisorDecimal)
        {
            var resultado = FormatarDecimalInterno(valor, casasDecimal);
            if(divisorDecimal== EnumDivisorDecimal.CulturaAtual)
            {
                return resultado;
            }

            if(divisorDecimal == EnumDivisorDecimal.Ponto)
            {
                return resultado.Replace(",", ".");
            }
            return resultado.Replace(".", ",");

        }

        private static string FormatarDecimalInterno(decimal valor, int digitos)
        {
            if (digitos == 0 || valor % 1 == 0)
            {
                return $"{valor:0}";
            }

            if (digitos == 1 ||
                ((valor - Math.Floor(valor)) * 10) % 1 == 0)
            {
                return $"{valor:0.0}";
            }

            if (digitos == 2 ||
                 ((valor - Math.Floor(valor)) * 100) % 1 == 0)
            {
                return $"{valor:0.00}";
            }

            if (digitos == 3 ||
                ((valor - Math.Floor(valor)) * 1000) % 1 == 0)
            {
                return $"{valor:0.000}";
            }

            string formatar = "0." + new string('0', digitos);
            return valor.ToString(formatar);

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
            nomeArquivoFormatado = TextoUtil.RemoverAcentos(nomeArquivoFormatado);
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

        public static string FormatarTelefone(string telefone)
        {
            var somenteNumeros = TextoUtil.RetornarSomenteNumeros(telefone);
            if (somenteNumeros.Length == 10)
            {
                return String.Format("{0:(##) ####-####}", somenteNumeros);
            }
            if (somenteNumeros.Length == 11)
            {
                return String.Format("{0:(##) #####-####}", somenteNumeros);
            }
            return telefone;
        }

        public static string FormatarRota(string nome)
        {
            var rota = TextoUtil.RetornarSomentesLetrasNumeros(nome.Trim(), true, "-_/".ToCharArray()).Replace(" ", "-").ToLower();
            while (rota.Contains("--"))
            {
                rota = rota.Replace("--", "-");
            }
            return UriUtil.AjustarBarraInicialFinal(rota);
        }

        public static string FormatarCentimetro(double espessura)
        {
            return $"{espessura.ToString("0.0", new CultureInfo("pt-BR"))}cm";
        }


    }

  public  enum EnumDivisorDecimal
    {
        CulturaAtual,
        Ponto,
        Virgula
    }
}