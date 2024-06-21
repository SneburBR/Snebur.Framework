using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Snebur.Serializacao
{
    public static class SerializacaoUtil
    {
        public static List<PropertyInfo> RetornarPropriedades(Type tipo)
        {
            var propriedades = ReflexaoUtil.RetornarPropriedades(tipo, false);
            propriedades = propriedades.Where(x => x.CanRead && x.CanWrite &&
                                                   x.GetGetMethod() != null && x.GetGetMethod().IsPublic &&
                                                   x.GetSetMethod() != null && x.GetSetMethod().IsPublic).
                                        Where(x =>
            {
                if (x.GetMethod.GetParameters().Length > 0)
                {
                    return false;
                }
                var atributoPropriedadeInterface = x.GetCustomAttribute(typeof(PropriedadeInterfaceAttribute));
                return atributoPropriedadeInterface == null;

            }).ToList();
            return propriedades;
        }

        public static string SerializarTipoSimples(object valor)
        {
            if (valor == null)
            {
                return null;
            }

            if (valor is BaseTipoComplexo tipoComplexto)
            {
                return JsonUtil.Serializar(tipoComplexto, EnumTipoSerializacao.Javascript);
            }

            var tipoP = ReflexaoUtil.RetornarTipoPrimarioEnum(valor.GetType());
            if (tipoP == Reflexao.EnumTipoPrimario.Desconhecido ||
                tipoP == Reflexao.EnumTipoPrimario.Object)
            {
                throw new Erro($"Tipo {tipoP} não suportado");
            }

            var tipo = ReflexaoUtil.RetornarTipoSemNullable(valor.GetType());
            if (tipo.IsEnum)
            {
                return Convert.ToInt32(valor).ToString();
            }

            switch (valor)
            {
                case Guid valorTipado:

                    return valorTipado.ToString();

                case string valorTipado:

                    return valorTipado;

                case decimal valorTipado:

                    return valorTipado.ToString(CultureInfo.InvariantCulture);

                case double valorTipado:

                    return valorTipado.ToString(CultureInfo.InvariantCulture);

                case float valorTipado:

                    return valorTipado.ToString(CultureInfo.InvariantCulture);

                case DateTime valorTipado:

                    return valorTipado.ToString(Snebur.AplicacaoSnebur.Atual.CulturaPadrao);

                case int valorTipado:

                    return valorTipado.ToString(CultureInfo.InvariantCulture);

                case long valorTipado:

                    return valorTipado.ToString(CultureInfo.InvariantCulture);

                case bool valorTipado:

                    return valorTipado.ToString(CultureInfo.InvariantCulture);

                default:

                    if (!tipo.IsValueType)
                    {
                        throw new Erro("Tipo não suportado");
                    }
                    return Convert.ToString(valor, CultureInfo.InvariantCulture);
            }
        }

        public static object DeserilizarTipoSimples(Type tipo, string valorSerializado)
        {
            if (String.IsNullOrWhiteSpace(valorSerializado))
            {
                return null;
            }

            if (tipo.IsSubclassOf(typeof(BaseTipoComplexo)))
            {
                return JsonUtil.Deserializar(valorSerializado, tipo, EnumTipoSerializacao.Javascript);
            }

            var tipoSemNullable = ReflexaoUtil.RetornarTipoSemNullable(tipo);
            if (tipoSemNullable.IsEnum)
            {
                return Convert.ToInt32(valorSerializado).ToString();
            }

            switch (tipo.Name)
            {
                case nameof(String):

                    return valorSerializado;

                case nameof(Guid):

                    return Guid.Parse(valorSerializado);

                case nameof(Int32):

                    return Int32.Parse(valorSerializado, CultureInfo.InvariantCulture);

                case nameof(Int64):

                    return Int64.Parse(valorSerializado, CultureInfo.InvariantCulture);

                case nameof(Boolean):

                    return Boolean.Parse(valorSerializado);

                case nameof(Decimal):

                    return Decimal.Parse(valorSerializado, CultureInfo.InvariantCulture);

                case nameof(Double):

                    return Double.Parse(valorSerializado, CultureInfo.InvariantCulture);

                case nameof(DateTime):

                    return DateTime.Parse(valorSerializado, CultureInfo.InvariantCulture);

                case nameof(TimeSpan):

                    return TimeSpan.Parse(valorSerializado, CultureInfo.InvariantCulture);


                case nameof(Single):

                    return Single.Parse(valorSerializado, CultureInfo.InvariantCulture);

                case nameof(Char):

                    return Char.Parse(valorSerializado);

                default:
                    if (!tipo.IsValueType)
                    {
                        throw new Erro("Tipo não suportado");
                    }
                    return Convert.ChangeType(valorSerializado, tipo, CultureInfo.InvariantCulture);
            }
        }
    }
}