using Snebur.Dominio.Atributos;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Snebur.Utilidade;

public static partial class ReflexaoUtil
{
    public static object? RetornarValorPadraoPropriedade(PropertyInfo propriedade)
    {
        var aceitaNulo = !propriedade.PropertyType.IsValueType || IsTipoNullable(propriedade.PropertyType);

        var atributoValorPadrao = propriedade.GetCustomAttribute<ValorPadraoAttribute>();
        if (atributoValorPadrao != null)
        {
            return ConverterUtil.Converter(atributoValorPadrao.ValorPadrao, propriedade.PropertyType);
        }
        if (aceitaNulo)
        {
            return null;
        }
        else
        {
            var tipoPrimarioEnum = RetornarTipoPrimarioEnum(propriedade.PropertyType);
            return ConverterUtil.RetornarValorNuloPadrao(tipoPrimarioEnum);
        }
    }

    public static string RetornarRotulo(PropertyInfo? pi)
    {
        if (pi is null)
        {
            return string.Empty;
        }

        var atributoRotulo = pi.GetCustomAttribute<RotuloAttribute>();
        if (atributoRotulo is null)
        {
            return pi.Name;
        }
        else
        {
            return atributoRotulo.Rotulo;
        }
    }

    public static bool TipoPossuiPropriedade(Type tipo, PropertyInfo propriedade)
    {
        return propriedade.DeclaringType == tipo ||
              (propriedade.DeclaringType is not null && tipo.IsSubclassOf(propriedade.DeclaringType));
    }

    public static string RetornarPluralClasse(Type tipoClasse)
    {
        var atributoRotulo = tipoClasse.GetCustomAttribute<RotuloAttribute>(false);
        if (atributoRotulo is null)
        {
            return string.Concat(tipoClasse.Name, "s");
        }

        if (string.IsNullOrWhiteSpace(atributoRotulo.RotuloPlural))
        {
            return string.Concat(tipoClasse.Name, "s");
        }
        return atributoRotulo.RotuloPlural;
    }

    public static List<PropertyInfo> RetornarPropriedadesEntidade(Type tipo)
    {
        return RetornarPropriedades(tipo, true).
                                Where(x =>
                                {
                                    if (IsPropriedadeRetornaTipoPrimario(x, true) &&
                                        x.GetGetMethod()?.IsPublic == true &&
                                        x.GetSetMethod()?.IsPublic == true)
                                    {
                                        var atrituboNaoMapear = x.GetCustomAttribute<NaoMapearAttribute>();
                                        return (atrituboNaoMapear == null);
                                    }
                                    return false;

                                }).ToList();
    }
}