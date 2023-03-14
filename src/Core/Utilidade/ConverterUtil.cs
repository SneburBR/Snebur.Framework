using Snebur.Reflexao;
using System;
using System.Linq;

namespace Snebur.Utilidade
{
    public class ConverterUtil
    {
        #region Convert

        #endregion

        public static T Converter<T>(object valor)
        {
            return (T)ConverterUtil.Converter(valor, typeof(T));
        }

        public static object Converter(object valor, Type tipo)
        {
            if (valor == null || valor == DBNull.Value)
            {
                if (tipo.IsValueType)
                {
                    if (ReflexaoUtil.IsTipoNullable(tipo))
                    {
                        return null;
                    }
                    return ConverterUtil.RetornarValorNuloPadrao(ReflexaoUtil.RetornarTipoPrimarioEnum(tipo));
                }
                return null;
            }
            if (ReflexaoUtil.IsTipoNullable(tipo))
            {
                tipo = ReflexaoUtil.RetornarTipoSemNullable(tipo);
            }
            if (ReflexaoUtil.TipoIgualOuHerda(valor.GetType(), tipo))
            {
                return valor;
            }
            else
            {
                if (tipo.IsValueType || tipo == typeof(String))
                {
                    if (tipo.IsEnum)
                    {
                        return Enum.ToObject(tipo, Convert.ToInt32(valor));
                    }
                    return ConverterUtil.ConverterTipoPrimario(valor, ReflexaoUtil.RetornarTipoPrimarioEnum(tipo));
                }
                else
                {
                    throw new ErroConverter(String.Format("Não possível converter o tipo {0} para {1} ", valor.GetType().Name, tipo.Name));
                }
            }
        }

        public static object ConverterTipoPrimario(object valor, EnumTipoPrimario tipoPrimarioEnum)
        {
            if (ConverterUtil.IsValorVazioOuNull(valor))
            {
                return ConverterUtil.RetornarValorNuloPadrao(tipoPrimarioEnum);
            }
            else
            {
                switch (tipoPrimarioEnum)
                {
                    case EnumTipoPrimario.String:

                        return Convert.ToString(valor);

                    case EnumTipoPrimario.Boolean:

                        return ConverterUtil.ParaBoolean(valor);

                    case EnumTipoPrimario.Integer:

                        return ConverterUtil.ParaInt32(valor);

                    case EnumTipoPrimario.Long:

                        return Convert.ToInt64(valor);

                    case EnumTipoPrimario.Decimal:

                        return Convert.ToDecimal(valor);

                    case EnumTipoPrimario.Double:

                        return Convert.ToDouble(valor);

                    case EnumTipoPrimario.Guid:

                        return Guid.Parse(valor.ToString());

                    case EnumTipoPrimario.TimeSpan:

                        return TimeSpan.Parse(valor.ToString());

                    case EnumTipoPrimario.DateTime:

                        return Convert.ToDateTime(valor);

                    case EnumTipoPrimario.Single:

                        return Convert.ToSingle(valor);

                    //case EnumTipoPrimario.Uri:

                    //    return new Uri(valor.ToString());

                    case EnumTipoPrimario.EnumValor:

                        return Int32.Parse(valor.ToString());

                    case EnumTipoPrimario.Char:

                        return Char.Parse(valor.ToString());

                    default:
                        throw new ErroNaoSuportado(String.Format("O tipo {0} não é suportado", EnumUtil.RetornarDescricao(tipoPrimarioEnum)));
                }
            }
        }

        public static bool ParaBoolean(object valor)
        {
            if (valor == null || valor == DBNull.Value || String.IsNullOrWhiteSpace(valor.ToString()))
            {
                return false;
            }
            else
            {
                if (valor is string)
                {
                    var valorString = valor.ToString().ToLower().Trim();
                    switch (valorString)
                    {
                        case "true":
                        case "yes":
                        case "sim":

                            return true;

                        case "false":
                        case "no":
                        case "nao":
                        case "não":

                            return false;

                        default:

                            throw new ErroNaoSuportado(String.Format("Não é possivel converter o valor {0} em boleano", valorString));
                    }
                }
                else
                {
                    return Convert.ToBoolean(valor);
                }
            }
        }

        public static object Para(object valor, Type tipo)
        {
            if (valor == null || DBNull.Value == valor)
            {
                return default;
            }

            if (valor.GetType() == ReflexaoUtil.RetornarTipoSemNullable( tipo))
            {
                return valor;
            }

            var tipoPrimario = ReflexaoUtil.RetornarTipoPrimarioEnum(tipo);
            if (tipoPrimario == EnumTipoPrimario.Desconhecido)
            {
                return Convert.ChangeType(valor, tipo);
            }
            return ConverterTipoPrimario(valor, tipoPrimario);
        }

        public static T Para<T>(object valor)
        {
            if (valor == null || DBNull.Value == valor)
            {
                return default;
            }

            var tipo = typeof(T);
            var tipoPrimario = ReflexaoUtil.RetornarTipoPrimarioEnum(tipo);
            if (tipoPrimario == EnumTipoPrimario.Desconhecido)
            {
                return (T)Convert.ChangeType(valor, tipo);
            }

            return (T)ConverterTipoPrimario(valor, tipoPrimario);
        }

        public static int ParaInt32(object valor)
        {
            if (valor is int)
            {
                return (int)valor;
            }
            if (ConverterUtil.IsValorVazioOuNull(valor))
            {
                return 0;
            }
            return Convert.ToInt32(valor);
        }

        public static long ParaInt64(object valor)
        {
            if (valor is long lvalor)
            {
                return lvalor;
            }
            if (ConverterUtil.IsValorVazioOuNull(valor))
            {
                return 0L;
            }
            return Convert.ToInt64(valor);
        }

        public static decimal ParaDecimal(object valor)
        {
            if (valor is decimal d)
            {
                return d;
            }
            if (ConverterUtil.IsValorVazioOuNull(valor))
            {
                return 0M;
            }
            return Convert.ToDecimal(valor, System.Globalization.CultureInfo.InvariantCulture);
        }

        public static double ParaDouble(object valor)
        {
            if (valor is double d)
            {
                return d;
            }
            if (ConverterUtil.IsValorVazioOuNull(valor))
            {
                return 0D;
            }
            return Convert.ToDouble(valor, System.Globalization.CultureInfo.InvariantCulture);
        }

        public static DateTime ParaDateTime(object valor)
        {
            if (valor is DateTime d)
            {
                return d;
            }
            if (ConverterUtil.IsValorVazioOuNull(valor))
            {
                return DateTime.Now;
            }
            var dataString = valor.ToString();

            if (DateTime.TryParse(dataString, out DateTime data))
            {
                return data;
            }
            if (Int64.TryParse(dataString, out long ticks) && ticks > DateTime.Now.AddYears(-100).Ticks)
            {
                return new DateTime(ticks);
            }
            if (!(dataString.Contains("/") ^ dataString.Contains('-')))
            {
                throw new Erro(String.Format("O valor não pode ser convertido para data {0}", dataString));
            }
            var divisorData = dataString.Contains('/') ? '/' : '-';
            var divisorHora = ':';

            var partes = dataString.Split(' ');
            var partaDataString = partes[0].Trim();
            var parteHoraString = partes.Length > 1 ? partes[1].Trim() : null;

            var partesData = partaDataString.Split(divisorData);
            var partesHora = (parteHoraString == null) ? null : parteHoraString.Split(divisorHora);

            if (partesData.Length < 3)
            {
                throw new Erro(String.Format("O valor não pode ser convertido para data {0}", dataString));
            }
            var parteAno = partesData[0];
            var parteMes = partesData[1];
            var parteDia = partesData[2];

            if (int.TryParse(parteAno, out int ano) &&
                int.TryParse(parteMes, out int mes) &&
                int.TryParse(parteDia, out int dia))
            {
                if (partesHora == null)
                {
                    return new DateTime(ano, mes, dia);
                }
            }
            throw new NotImplementedException();
        }

        private static bool IsValorVazioOuNull(object valor)
        {
            return valor == null ||
                   valor == DBNull.Value ||
                   String.IsNullOrWhiteSpace(valor.ToString());
        }

        public static object RetornarValorNuloPadrao(EnumTipoPrimario tipoPrimarioEnum)
        {
            switch (tipoPrimarioEnum)
            {
                case EnumTipoPrimario.Desconhecido:

                    return null;

                case EnumTipoPrimario.String:

                    return null;

                case EnumTipoPrimario.Boolean:

                    return false;

                case EnumTipoPrimario.Integer:

                    return 0;

                case EnumTipoPrimario.Long:

                    return 0L;

                case EnumTipoPrimario.Decimal:

                    return 0M;

                case EnumTipoPrimario.Double:

                    return 0D;

                case EnumTipoPrimario.Guid:

                    return Guid.Empty;

                case EnumTipoPrimario.TimeSpan:

                    return DateTime.Now.TimeOfDay;

                case EnumTipoPrimario.DateTime:

                    return DateTime.Now;

                //case EnumTipoPrimario.Uri:
                //    return null;
                //case EnumTipoPrimario.Object:
                //    return null;
                case EnumTipoPrimario.EnumValor:

                    return 0;

                case EnumTipoPrimario.Single:

                    return 0F;

                case EnumTipoPrimario.Char:

                    return ' ';

                case EnumTipoPrimario.Byte:

                    return Byte.MinValue;

                default:

                    throw new ErroNaoSuportado(String.Format("O tipo {0} não é suportado", EnumUtil.RetornarDescricao(tipoPrimarioEnum)));
            }
        }

        public static string ParaString(object valorTipado, bool aceitarNulo = false)
        {
            if (valorTipado == null && !aceitarNulo)
            {
                return String.Empty;
            }
            return Convert.ToString(valorTipado);
        }

        public static string ConverterHexaParaRgbaInterno(string corHexa)
        {
            if (corHexa.StartsWith("#"))
            {
                corHexa = corHexa.Substring(1);
            }

            if (corHexa.Length == 3)
            {
                var r = (byte)Convert.ToInt32(corHexa.Substring(0, 1), 16);
                var g = (byte)Convert.ToInt32(corHexa.Substring(1, 1), 16);
                var b = (byte)Convert.ToInt32(corHexa.Substring(2, 1), 16);
                return ConverterUtil.RetornarRgbaCorInterno(r, g, b, 255);
            }
            else
            {
                var strR = corHexa.Substring(0, 2) ?? "00";
                var strG = corHexa.Substring(2, 2) ?? "00";
                var strB = corHexa.Substring(4, 2) ?? "00";
                var strA = corHexa.Length == 8 ? corHexa.Substring(6, 8) : "FF";


                var r = (byte)Convert.ToInt32(strR, 16);
                var g = (byte)Convert.ToInt32(strG, 16);
                var b = (byte)Convert.ToInt32(strB, 16);
                var a = (byte)Convert.ToInt32(strA, 16);

                return ConverterUtil.RetornarRgbaCorInterno(r, g, b, a);
            }
            //return null;
        }
        private static string RetornarRgbaCorInterno(byte red, byte green, byte blue, byte alpha)
        {
            var alphaDecimal = (alpha / (decimal)byte.MaxValue);
            return $"rgba({red},{green},{blue},{Math.Round(alphaDecimal, 2)})";
        }
    }
}