using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Snebur;

public static class PropertyInfoExtensions
{
    public static PropertyInfo? GetOneToOneNavigationProperty(this PropertyInfo propertyInfo)
    {
        var declaringType = propertyInfo.DeclaringType;
        Guard.NotNull(declaringType);

        var proprities = declaringType.GetProperties()
            .Where(p => p.GetCustomAttribute<ForeignKeyAttribute>(false)?.Name == propertyInfo.Name)
            .ToArray();

        if (proprities.Length == 0)
            return null;

        if (proprities.Length > 1)
        {
            throw new InvalidOperationException(
                $"The property '{propertyInfo.Name}' that is was a foreign key " +
                $"But more than one related property with {nameof(ForeignKeyAttribute)}(nameof({propertyInfo.Name})) one to one was found ");
        }
        return proprities[0];
    }

    public static PropertyInfo GetRequiredOneToOneNavigationProperty(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetOneToOneNavigationProperty()
            ?? throw new InvalidOperationException(
                $"The property '{propertyInfo.Name}' that is was a foreign key " +
                $"But the related property with {nameof(ForeignKeyAttribute)}(nameof({propertyInfo.Name})) one to one was not found ");
    }

    public static TInterface? GetAttributeImplementingInterface<TAttribute, TInterface>(
        this PropertyInfo propertyInfo,
        bool inherit = false)
        where TAttribute : Attribute
        where TInterface : class
    {
        var attributes = propertyInfo.GetCustomAttributes(typeof(TAttribute), inherit);
        if (attributes?.Length > 0)
        {
            foreach (var attribute in attributes)
            {
                if (attribute is TInterface interfaceAttribute)
                {
                    return interfaceAttribute;
                }
            }
        }
        return null;
    }

    public static bool IsSameProperty(this PropertyInfo? property, PropertyInfo? other)
    {
        if (property is null && other is null)
            return true;

        if (property is null || other is null)
            return false;

        if (property == other || property.Equals(other))
            return true;

        return property.DeclaringType.IsSameType(other.DeclaringType) &&
               property.Name == other.Name;

    }

    public static Type GetRequiredDeclaringType(this PropertyInfo propertyInfo)
    {
        var declaringType = propertyInfo.DeclaringType;
        if (declaringType is null)
        {
            throw new InvalidOperationException(
                $"The property '{propertyInfo.Name}' does not have a declaring type.");
        }
        return declaringType;
    }

    public static bool IsOverridden(this PropertyInfo property)
    {
        // Check one accessor (get or set) to see if it overrides
        var accessor = property.GetMethod ?? property.SetMethod;
        if (accessor == null)
            return false;

        // Get base definition of this method
        var baseDef = accessor.GetBaseDefinition();

        // If base definition declaring type is different, it's overridden
        return baseDef.DeclaringType != accessor.DeclaringType;
    }

    public static bool IsObsolete(this PropertyInfo property)
    {
        return property.GetCustomAttribute<ObsoleteAttribute>(true) != null;
    }

    public static bool IsRequired(this PropertyInfo property)
    {
        return property.GetCustomAttributes(typeof(RequiredMemberAttribute), inherit: false).Any();
    }

    public static bool IsNotMapped(this PropertyInfo property)
    {
        return property.GetCustomAttribute<NotMappedAttribute>(false) != null;
    }

    public static bool IsRelation(this PropertyInfo property)
    {
        var type = property.PropertyType;
        return CustomAttributeExtensions.GetCustomAttribute<BaseRelacaoAttribute>(type) is not null ||
               CustomAttributeExtensions.GetCustomAttribute<RelacaoPaiExternaAttribute>(type) is not null;
    }

    public static bool IsComputed(this PropertyInfo property)
    {
        return property.GetCustomAttribute<PropriedadeComputadaBancoAttribute>(false) != null;
    }

    public static int GetRankOrder(this PropertyInfo property)
    {
        if (property.IsNotMapped())
        {
            return 10 + GetRankOrder(property.PropertyType);
        }
        if (property.IsProxyProperty())
        {
            return 20 + GetRankOrder(property.PropertyType);
        }

        if (property.IsStaticProperty())
        {
            return 30 + GetRankOrder(property.PropertyType);
        }
        return GetRankOrder(property.PropertyType);
    }

    private static int GetRankOrder(Type type)
    {
        if (ReflexaoUtil.TipoRetornaTipoPrimario(type))
        {
            if (type.IsValueType)
                return 1;
            return 2;
        }
        if (type == typeof(Type))
        {
            return 3;
        }

        if (type.IsSubclassOf<BaseTipoComplexo>())
        {
            return 4;
        }

        if (type.IsCollectionType())
        {
            var itemType = type.GetCollectionItemType();
            if (itemType.IsSubclassOf<Entidade>())
                return 7;

            return 5;
        }

        if (type.IsCollectionType())
        {
            var itemType = type.GetCollectionItemType();
            if (itemType.IsSubclassOf<Entidade>())
                return 9;
            return 5;
        }

        if (type.IsDictionaryType())
        {
            var valueType = type.GetDictionaryValueType();
            if (valueType.IsSubclassOf<Entidade>())
                return 10;
            return 6;
        }

        if (type.IsSubclassOf<Entidade>())
        {
            return 7;
        }

        if (type.ImplementsInterface<IEntidade>())
        {
            return 8;
        }

        if (type.IsInterface)
        {
            return 9;
        }

        return int.MaxValue;

        //throw new NotSupportedException($"Property type '{type.Name}' is not supported for RankOrder");
        //return 10;
    }

    public static bool IsStaticProperty(this PropertyInfo property)
    {
        var getMethod = property.GetMethod;
        var setMethod = property.SetMethod;
        return (getMethod != null && getMethod.IsStatic) ||
               (setMethod != null && setMethod.IsStatic);
    }

    public static bool IsProxyProperty(this PropertyInfo property)
    {
        return property.GetCustomAttribute<PropriedadeInterfaceAttribute>(false) is not null;
    }
}

