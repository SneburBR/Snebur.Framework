using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Snebur.Utilidade
{
    public static partial class ReflexaoUtil
    {
        public static System.Reflection.BindingFlags BindingFlags
        {
            get
            {
                return BindingFlags.IgnoreCase |
                       BindingFlags.GetProperty |
                       BindingFlags.Instance |
                       BindingFlags.Public |
                       BindingFlags.NonPublic;
            }
        }
        #region Metodo

        public static List<MethodInfo> RetornarMetodos(Type tipo, BindingFlags bindingFlags)
        {
            return tipo.GetMethods(bindingFlags).ToList();
        }

        public static List<MethodInfo> RetornarMetodos(Type tipo, bool ignorarMetodosTipoBase = false, bool ignorarMetodosGetSet = true)
        {
            var metodos = ReflexaoUtil.RetornarMetodos(tipo, ReflexaoUtil.BindingFlags, ignorarMetodosTipoBase);
            if (ignorarMetodosGetSet)
            {
                metodos = metodos.Where(x => !x.Name.StartsWith("get_") && !x.Name.StartsWith("set_")).ToList();
            }
            return metodos;
        }

        public static List<MethodInfo> RetornarMetodos(Type tipo, BindingFlags bindingFlags, bool ignorarPropriedadesTipoBase = false)
        {
            var metodos = ReflexaoUtil.RetornarMetodos(tipo, bindingFlags);
            if (ignorarPropriedadesTipoBase && tipo.BaseType != null)
            {
                return metodos.Where(x => Object.ReferenceEquals(x.DeclaringType, tipo)).ToList();
            }
            else
            {
                return metodos;
            }
        }
        #endregion

        #region Construtor

        public static bool ConstrutorPossuiAtributo(ConstructorInfo construtor, Type tipoAtributo, bool herdado = true)
        {
            return construtor.GetCustomAttributes(tipoAtributo, herdado).FirstOrDefault() != null;
        }

        public static bool ExisteConstrutorVazio(Type tipo)
        {
            return tipo.GetConstructors().Any(x => x.IsPublic && x.GetParameters().Count() == 0);
        }
        #endregion

        #region Constantes

        public static List<FieldInfo> RetornarConstantes(Type tipo, bool ignorarTipoBase = false)
        {
            var constantes = tipo.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).ToList();
            //constantes = constantes.Where(x => x.IsLiteral && !x.IsInitOnly).ToList();
            if (ignorarTipoBase && tipo.BaseType != null)
            {
                constantes = constantes.Where(x => x.DeclaringType == tipo).ToList();
            }
            return constantes;
        }
        #endregion
    }
}
/*  --------------- DOMINIO 

public static string RetornarRotulo(Type tipoClasse)
{
    RotuloAttribute atributoRotulo = tipoClasse.GetCustomAttributes(typeof(RotuloAttribute), true).FirstOrDefault;
    if (atributoRotulo == null)
    {
        return tipoClasse.Name;
    }
    else
    {
        return atributoRotulo.Rotulo;
    }
}

public static string RetornarRotulo(PropertyInfo pi)
{
    RotuloAttribute atributoRotulo = pi.GetCustomAttributes(typeof(RotuloAttribute), true).FirstOrDefault;
    if (atributoRotulo == null)
    {
        return pi.Name;
    }
    else
    {
        return atributoRotulo.Rotulo;
    }
}

public static string RetornarPluralClasse(Type tipoClasse)
{
    RotuloAttribute atributoRotulo = tipoClasse.GetCustomAttributes(typeof(RotuloAttribute), true).FirstOrDefault;
    if (atributoRotulo == null)
    {
        return string.Concat(tipoClasse.Name, "s");
    }
    else
    {
        if (string.IsNullOrEmpty(atributoRotulo.Plural))
        {
            return string.Concat(tipoClasse.Name, "s");
        }
        else
        {
            return atributoRotulo.Plural;
        }
    }
}
    ---- */