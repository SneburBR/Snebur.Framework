namespace Snebur.AcessoDados;

internal class TipoEntidadeUtil
{

    private static object Bloqueio = new object();
    /// <summary>
    /// Chave, String.Formar("{0}.{1}", Namespace, NomeTipoEntidade
    /// </summary>
    /// <returns></returns>
    internal static Dictionary<string, Type> TiposEntidade
    {
        get
        {
            lock (Bloqueio)
            {
                if (field is not null)
                {
                    return field;
                }
                field = new Dictionary<string, Type>();

                var assemblies = RetornarTodosAssemblies();
                if (assemblies.Count() > 0)
                {

                    foreach (var assembly in assemblies)
                    {
                        if (assembly.IsAssemblySnebur() ||
                            assembly.IsAssemblyEntidades())
                        {
                            var tipos = RetornarTiposEntidade(assembly);
                            if (tipos != null)
                            {
                                foreach (var tipo in tipos)
                                {
                                    var chave = String.Format("{0}.{1}", tipo.Namespace, tipo.Name);
                                    if (field.ContainsKey(chave))
                                    {
                                        throw new Erro(String.Format("Chave duplicada {0}", chave));
                                    }
                                    field.Add(chave, tipo);
                                }
                            }
                        }
                    }
                    if (field.Count == 0)
                    {
                        throw new Erro($"Nenhum tipo entidade foi encontrado, adicione o atributo {nameof(AssemblyEntidadesAttribute)} no projetos das Entidades ");
                    }
                }
            }
            return field!;
        }
    }

    private static IEnumerable<Assembly> RetornarTodosAssemblies()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            LoadReferencedAssembly(assembly);
        }
        return AppDomain.CurrentDomain.GetAssemblies();
    }

    private static void LoadReferencedAssembly(Assembly assembly)
    {
        foreach (var name in assembly.GetReferencedAssemblies())
        {
            if (name.Name?.Contains("Entidades") == true &&
               !AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName == name.FullName))
            {
                LoadReferencedAssembly(Assembly.Load(name));
            }
        }
    }

    private static List<Type>? RetornarTiposEntidade(Assembly assembly)
    {
        try
        {
            if (assembly.GetCustomAttribute<AssemblyEntidadesAttribute>() != null)
            {
                return assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Entidade))).ToList();
            }
            return null;
        }
        catch (Exception ex)
        {
            LogUtil.ErroAsync(new Erro($"Não foi possivel carregar os tipos entidade do assemply {assembly.FullName}", ex));
            return null;
        }
    }

    public static Type RetornarTipoEntidade(string tipoEntiadeAssemblyQualifiedName)
    {
        var tipoEntidade = Type.GetType(tipoEntiadeAssemblyQualifiedName);
        if (tipoEntidade == null)
        {
            throw new Exception($"O tipo da entidade não foi encontrado {tipoEntiadeAssemblyQualifiedName} ");
        }
        var chave = String.Format($"{tipoEntidade.Namespace}.{tipoEntidade.Name}");

        if (!TiposEntidade.ContainsKey(chave))
        {
            throw new Exception($"O tipo da entidade não foi encontrado na coleção do repositorio {tipoEntiadeAssemblyQualifiedName}");
        }
        return tipoEntidade;
    }
}