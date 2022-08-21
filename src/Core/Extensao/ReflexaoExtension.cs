using Snebur.Dominio.Atributos;
using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
    public static class ReflexaoExtension
    {
        public static IEnumerable<Type> GetAccessibleTypes(this Assembly assembly)
        {
            try
            {
                //return assembly.GetTypes();
                return assembly.DefinedTypes.Select(t => t.AsType());
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null);
            }
        }

        public static object TryGetValueOrDefault(this PropertyInfo propriedade, object obj)
        {
            try
            {
                return propriedade.GetValue(obj);
            }
            catch
            {
                return default;
            }
        }

        public static bool IsTipoIguaOuHerda(this Type origem, Type tipo)
        {
            return origem == tipo || origem.IsSubclassOf(tipo);
            //return metodo.GetBaseDefinition().DeclaringType != metodo.DeclaringType;
        }

        public static bool IsOverride(this MethodInfo metodo)
        {
            return metodo.GetBaseDefinition().DeclaringType != metodo.DeclaringType;
        }

        public static bool IsOverride(this PropertyInfo propriedade)
        {
            var getMethod = propriedade.GetGetMethod();
            var setMethod = propriedade.GetSetMethod();
            var getIsOverride = (getMethod?.IsOverride() ?? false);
            var setIsOverride = (setMethod?.IsOverride() ?? false);
            return getIsOverride || setIsOverride;
        }
        /// <summary>
        /// Paliativo, para contornar  a migração do entity framework
        /// </summary>
        /// <param name="propriedade"></param>
        /// <returns></returns>
        public static IChaveEstrangeiraAttribute RetornarAtributoChaveEstrangeira(this PropertyInfo propriedade)
        {
            var atributoChaveEstrangeira = propriedade.GetCustomAttribute<ChaveEstrangeiraAttribute>();
            if (atributoChaveEstrangeira != null)
            {
                return atributoChaveEstrangeira;
            }
            var atributoChaveEstrangeiraRelacaoUmUm = propriedade.GetCustomAttribute<ChaveEstrangeiraRelacaoUmUmAttribute>();
            if (atributoChaveEstrangeiraRelacaoUmUm != null)
            {
                return atributoChaveEstrangeiraRelacaoUmUm;
            }
            throw new Erro(String.Format("Não foi encontrado um chave estrangeira para a propriedade {0} em {1} ", propriedade.Name, propriedade.DeclaringType.Name));
        }
    }
}