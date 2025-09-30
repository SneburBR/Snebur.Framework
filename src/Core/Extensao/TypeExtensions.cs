using System.Collections;
using System.Reflection;
using System.Threading.Tasks;

namespace Snebur.Extensao;

public static class TypeExtensions
{
    public static Type GetUnderlyingType(this Type type)
    {
        if (type.IsNullableType())
        {
            return Nullable.GetUnderlyingType(type)
                ?? type;
        }

        if (type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            return type.GetGenericArguments()[0];
        }

        if (type == typeof(Task))
            return typeof(void);

        return type;
    }

    public static bool IsSubclassOfOrEqual<T>(this Type type)
    {
        return type.IsSubclassOfOrEqual(typeof(T));
    }

    public static bool IsSubclassOfOrEqual(this Type type, Type baseType)
    {
        if (type is null || baseType is null)
        {
            return false;
        }
        if (type == baseType)
        {
            return true;
        }
        if (baseType.IsInterface)
        {
            return type.GetInterfaces().Contains(baseType);
        }

        while (type is not null && type != typeof(object))
        {
            if (type == baseType)
            {
                return true;
            }
            type = type.BaseType!;
        }
        return false;
    }
    public static bool IsConcrete(this Type type)
    {
        Guard.NotNull(type);

        return !type.IsAbstract && !type.IsInterface;
    }
    public static bool IsObsolete(this Type type)
    {
        return type.GetCustomAttribute<ObsoleteAttribute>(true) != null;
    }

    public static bool IsEntityType(this Type type)
    {
        return type.IsSubclassOfOrEqual<Entidade>();
    }
    public static bool IsSameType(this Type? type, Type? other)
    {
        if (type is null && other is null)
            return true;

        if (type is null || other is null)
            return false;

        if (type == other || type.Equals(other))
            return true;

        //When the type came from different AppDomains
        return type.FullName == other.FullName
               && type.Assembly.IsSameAssembly(other.Assembly);
    }

    public static string GetFriendlyName(this Type? type)
    {
        if (type is null)
        {
            return "null";
        }

        if (type.IsGenericType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            var genericArguments = type.GetGenericArguments();
            var genericArgumentNames = string.Join(", ", genericArguments.Select(t => t.GetFriendlyName()));
            var typeName = genericTypeDefinition.Name;
            var indexLastBacktick = typeName.LastIndexOf('`');
            if (indexLastBacktick >= 0)
            {
                typeName = typeName.Substring(0, indexLastBacktick);
            }
            return $"{typeName}<{genericArgumentNames}>";
        }
        return type.Name;
    }

    public static PropertyInfo GetRequiredProperty(this Type type, string name)
    {
        var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        if (property is null)
        {
            throw new InvalidOperationException(
                $"The property '{name}' was not found in type '{type.FullName}'.");
        }
        return property;

    }

    public static bool IsNumberType(this Type type)
    {
        type = type.GetUnderlyingType();
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
                return true;
            default:
                return false;
        }
    }
    //public static bool IsNumberType(this Type type)
    //{
    //    var underlyingType = type.GetUnderlyingType();
    //    return underlyingType == typeof(byte)
    //           || underlyingType == typeof(sbyte)
    //           || underlyingType == typeof(short)
    //           || underlyingType == typeof(ushort)
    //           || underlyingType == typeof(int)
    //           || underlyingType == typeof(uint)
    //           || underlyingType == typeof(long)
    //           || underlyingType == typeof(ulong)
    //           || underlyingType == typeof(float)
    //           || underlyingType == typeof(double)
    //           || underlyingType == typeof(decimal);
    //}

    public static string GetDisplayName(this Type type, bool excludeNestedTypeNames = false)
    {
        Guard.NotNull(type);

        if (type.IsGenericType)
        {
            var genericArguments = type.GetGenericArguments()
                .Select(x => GetDisplayName(x, excludeNestedTypeNames));

            return $"{type.Name.Split('`')[0]}<{string.Join(", ", genericArguments)}>";
        }

        if (!excludeNestedTypeNames && type.IsNested && type.DeclaringType is not null)
        {
            return $"{type.DeclaringType.Name}.{type.Name}";
        }
        return type.Name;
    }

    public static bool IsSubclassOf<T>(this Type type)
    {
        return type.IsSubclassOf(typeof(T));
    }

    public static bool ImplementsInterface<T>(this Type type)
    {
        var interfaceType = typeof(T);
        if (type == interfaceType)
            return true;

        return type.GetInterfaces().Contains(interfaceType);
    }

    public static bool ImplementsGenericInterface(this Type type, Type genericType)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
            return true;

        return type.GetInterfaces()
                   .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericType);
    }
    public static bool IsNullableType(this Type type)
        => type.IsGenericType
        && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    public static bool IsCollectionType(this Type type)
    {
        type = type.GetUnderlyingType();
        if (type.IsArray)
        {
            return true;
        }

        if (type.IsDictionaryType())
        {
            return false;
        }
        if (type == typeof(string))
        {
            return false;
        }

        if (typeof(ICollection).IsAssignableFrom(type))
            return true;

        if (ImplementsGenericInterface(type, typeof(ICollection<>)))
            return true;

        if (ImplementsGenericInterface(type, typeof(IReadOnlyCollection<>)))
            return true;

        if (ImplementsGenericInterface(type, typeof(IEnumerable<>)))
            return true;

        return false;
    }

    public static bool IsDictionaryType(this Type type)
    {
        type = type.GetUnderlyingType();
        return typeof(IDictionary).IsAssignableFrom(type) && type.IsGenericType;
    }

    public static bool IsEnumerableType(this Type type)
    {
        type = type.GetUnderlyingType();
        if (type.IsArray)
        {
            return true;
        }

        if (type == typeof(string))
        {
            return false;
        }
        return typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType;
    }

    public static Type GetCollectionItemType(this Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType()
                ?? throw new InvalidOperationException($"Cannot get the element type of the array type '{type.FullName}'.");
        }

        if (type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type))
        {
            var arguments = type.GetGenericArguments();
            if (arguments.Length == 1)
                return arguments[0];

            if (arguments.Length > 1)
            {
                throw new InvalidOperationException(
                    $"The collection type '{type.FullName}' has more than one generic argument.\r\n" +
                    $"Arguments : {string.Join(", ", arguments.Select(a => a.FullName))}");
            }
        }

        throw new Exception($"The type '{type.FullName}' is not a collection type." +
                            $"Use the method 'IsCollectionType' to check if the type is a collection type before calling this method.");
    }
    public static Type GetDictionaryKeyType(this Type type)
    {
        if (type.IsGenericType && typeof(IDictionary).IsAssignableFrom(type))
        {
            var arguments = type.GetGenericArguments();
            if (arguments.Length == 2)
                return arguments[0];
            if (arguments.Length > 2)
            {
                throw new InvalidOperationException(
                    $"The dictionary type '{type.FullName}' has more than two generic arguments.\r\n" +
                    $"Arguments : {string.Join(", ", arguments.Select(a => a.FullName))}");
            }
        }
        throw new Exception($"The type '{type.FullName}' is not a dictionary type." +
                            $"Use the method 'IsDictionaryType' to check if the type is a dictionary type before calling this method.");
    }

    public static Type GetDictionaryValueType(this Type type)
    {
        if (type.IsGenericType && typeof(IDictionary).IsAssignableFrom(type))
        {
            var arguments = type.GetGenericArguments();
            if (arguments.Length == 2)
                return arguments[1];

            if (arguments.Length > 2)
            {
                throw new InvalidOperationException(
                    $"The dictionary type '{type.FullName}' has more than two generic arguments.\r\n" +
                    $"Arguments : {string.Join(", ", arguments.Select(a => a.FullName))}");
            }
        }

        throw new Exception($"The type '{type.FullName}' is not a dictionary type." +
                            $"Use the method 'IsDictionaryType' to check if the type is a dictionary type before calling this method.");

    }
    public static bool IsDomainAttribute(this Type type)
    {
        if (!type.IsClass ||
            type.GetCustomAttribute<IgnorarAtributoTSAttribute>() is not null ||
            type.Namespace?.StartsWith("System") == true)
        {
            return false;
        }
        return
               type.IsSubclassOfOrEqual(typeof(BaseAtributoDominio)) ||
               type.ImplementsInterface<IAtributoValidacao>();

    }
}
