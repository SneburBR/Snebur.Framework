using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
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
                throw new NotImplementedException();
            }
            var tipo = ReflexaoUtil.RetornarTipoSemNullable(valor.GetType());
            if (tipo.IsEnum)
            {
                return Convert.ToInt32(valor).ToString();
            }
            switch (valor)
            {
                case string valorTipado:

                    return valorTipado;

                case decimal valorTipado:

                    return valorTipado.ToString(CultureInfo.InvariantCulture);

                case double valorTipado:

                    return valorTipado.ToString(CultureInfo.InvariantCulture);

                case float valorTipado:

                    return valorTipado.ToString(CultureInfo.InvariantCulture);

                case DateTime valorTipado:

                    return valorTipado.ToString(CultureInfo.InvariantCulture);

                default:

                    if (!tipo.IsValueType)
                    {
                        throw new Erro("Tipo não suportado");
                    }
                    return Convert.ToString(valor);
            }
        }
    }
}