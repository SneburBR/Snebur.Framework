using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
 

namespace Snebur.Dominio;
internal static class EntidadeInfoUtils
{
    private static readonly ConcurrentDictionary<Type, EntidadeInfo> _cache = new();

    public static EntidadeInfo GetEntidadeInfo(Type tipoEntidade)
    {
        if (!_cache.TryGetValue(tipoEntidade, out var entidadeInfo))
        {
            entidadeInfo = new EntidadeInfo(tipoEntidade);
            _cache[tipoEntidade] = entidadeInfo;
        }
        return entidadeInfo;
    }
}

internal class EntidadeInfo
{
    internal PropertyInfo[] PropriedadesTipoComplexo { get; }
    internal PropertyInfo PropriedadeChavePrimaria { get; }
    internal bool IsIdentity { get; }

    internal EntidadeInfo(Type tipoEntidade)
    {
        var propriedades = tipoEntidade.GetProperties();

        PropriedadesTipoComplexo = propriedades
            .Where(x => x.PropertyType.IsSubclassOf(typeof(BaseTipoComplexo)))
            .Where(x => x.GetMethod is not null && x.SetMethod is not null && x.GetCustomAttribute<NotMappedAttribute>() is null)
            .ToArray();

        var tipoIdentity = GetBaseIdentity(tipoEntidade);
        PropriedadeChavePrimaria = tipoIdentity.GetProperties()
            .FirstOrDefault(x => x.GetCustomAttribute<KeyAttribute>(false) is not null)
            ?? throw new Exception($"A entidade {tipoEntidade.FullName} não possui uma propriedade marcada com o atributo {nameof(KeyAttribute)}");

        var dbAttr = PropriedadeChavePrimaria.GetCustomAttribute<DatabaseGeneratedAttribute>(false)
            ?? throw new Exception($"A propriedade chave primária {PropriedadeChavePrimaria.Name} da entidade {tipoEntidade.FullName} não possui o atributo {nameof(DatabaseGeneratedAttribute)}");

        IsIdentity = dbAttr.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity;
    }

    private Type GetBaseIdentity(Type tipoEntidade)
    {
        var current = tipoEntidade;
        while(current.BaseType!= null)
        {
            if(current.BaseType == typeof(Entidade) )
            {
                return current;
            }
            current = current.BaseType;
        }
        throw new Exception($"A entidade {tipoEntidade.FullName} não herda de {nameof(Entidade)}");
    }
}
