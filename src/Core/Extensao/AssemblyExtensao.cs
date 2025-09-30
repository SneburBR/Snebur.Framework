using System.IO;
using System.Reflection;
using System.Text;

namespace Snebur.Extensao;

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
        return assembly.FullName?.StartsWith("Snebur") == true ||
               assembly.FullName?.StartsWith("Snebur") == true;
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
        if (assembly.FullName?.StartsWith("System") == true)
        {
            return false;
        }

        if (assembly.GetCustomAttribute<AssemblyEntidadesAttribute>() != null)
        {
            return true;
        }

        if (DebugUtil.IsAttached)
        {
            if (assembly.FullName?.Contains("Entidades") == true)
            {
                throw new Exception($"Adicione o atributos {nameof(AssemblyEntidadesAttribute)} no assembly {assembly.FullName}");
            }
        }

        var atributos = assembly.GetCustomAttributes();
        return atributos.Any(x => x.GetType().Name == nameof(AssemblyEntidadesAttribute));
    }

    public static Type[] GetLoadableTypes(this Assembly assembly)
    {
        if (assembly == null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t is not null)
                .Cast<Type>()
                .ToArray();
        }
    }

    public static string GetResourceAsString(
        this Assembly assembly,
        string resource,
        Encoding? encoding = null)
    {
        encoding = encoding ?? Encoding.UTF8;
        using (var ms = new MemoryStream())
        {
            using (var manifestResourceStream = assembly.GetManifestResourceStream(resource))
            {
                manifestResourceStream?.CopyTo(ms);
            }
            return encoding.GetString(ms.GetBuffer()).Replace('\0', ' ').Trim();
        }
    }

    public static FileInfo GetAssemblyFile(this Assembly assembly)
    {
        return new FileInfo(new Uri(assembly.Location).LocalPath);
    }

    public static bool IsSameAssembly(this Assembly? assembly, Assembly? other)
    {
        if (assembly is null && other is null)
            return true;

        if (assembly is null || other is null)
            return false;

        if (assembly == other || assembly.Equals(other))
            return true;

        return assembly.FullName == other.FullName
               && assembly.GetName().Version == other.GetName().Version;
    }
}
