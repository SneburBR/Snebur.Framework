using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System.Linq;

namespace System.Reflection
{
    public static class AssemblyExtensao
    {
        public static bool IsVersaoTeste(this Assembly assembly)
        {
            var atributoVersao = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            if (atributoVersao != null)
            {
                var ultimaParte = atributoVersao.Version.Split('.').Last();
                if (ultimaParte.Length > 1)
                {
                    return ultimaParte.StartsWith("0");
                }
            }
            return false;
        }

        public static string RetornarVersao(this Assembly assembly)
        {
            var atributoVersao = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            if (atributoVersao != null)
            {
                return atributoVersao.Version;
            }
            return "0.0.0.0";
        }

        public static bool IsAssemblySnebur(this Assembly assembly)
        {
            return assembly.FullName.StartsWith("Snebur") ||
                   assembly.FullName.StartsWith("Snebur");
            //var atributo = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            //if (atributoVersao != null)
            //{
            //    return atributo.Company = ""
            //}
            //return "0.0.0.0";
            //IsAssemblySnebur
        }

        public static bool IsAssemblyEntidades(this Assembly assembly)
        {
            if (assembly.FullName.StartsWith("System"))
            {
                return false;
            }

            if(assembly.GetCustomAttribute<AssemblyEntidadesAttribute>() != null)
            {
                return true;
            }

            if (DebugUtil.IsAttached)
            {
                if (assembly.FullName.Contains("Entidades"))
                {
                    throw new Exception($"Adicione o atributos {nameof(AssemblyEntidadesAttribute)} no assembly {assembly.FullName}");
                }
            }

            var atributos = assembly.GetCustomAttributes();
            return atributos.Any(x => x.GetType().Name == nameof(AssemblyEntidadesAttribute));
        }
    }
}
