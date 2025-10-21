using Snebur.Linq;
using Snebur.Reflexao;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;

namespace Snebur.Extensao;

public static class TypeExtensions
{
    private const string VERSION_MARK = "Version=";
    private const string START_GENERIC_MARK = "[[";
    private const string END_GENERIC_MARK = "]]";

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

    public static bool IsSubclassOf<T>(this Type type)
    {
        return IsSubclassOfInternal(type, typeof(T));
    }

    public static bool IsSubclassOfOrEqual<T>(this Type type)
    {
        return type.IsSubclassOfOrEqual(typeof(T));
    }

    public static bool IsSubclassOfOrEqual(
        this Type type,
        Type baseType)
    {
        if (type is null || baseType is null)
        {
            return false;
        }

        if (type == baseType)
        {
            return true;
        }
        return IsSubclassOfInternal(type, baseType);
    }

    private static bool IsSubclassOfInternal(
        this Type type,
        Type baseType)
    {
        if (type == baseType)
            return false;

        if (type.IsGenericType && baseType.IsGenericTypeDefinition)
        {
            var current = type;
            if (current != type)
                throw new InvalidOperationException();

            while (current is not null)
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == baseType)
                {
                    return true;
                }
                current = current.BaseType;
            }
            return false;
        }

        if (baseType.IsInterface)
        {
            return type.GetInterfaces().Contains(baseType);
        }
        return type.IsSubclassOf(baseType);
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

            return $"{type.GetSimpleName()}<{string.Join(", ", genericArguments)}>";
        }

        if (!excludeNestedTypeNames && type.IsNested && type.DeclaringType is not null)
        {
            return $"{type.DeclaringType.Name}.{type.Name}";
        }
        return type.Name;
    }
     
    public static string GetSimpleName(this Type type)
    {
        if (!type.IsGenericType)
            return type.Name;

        return type.Name.Split('`')[0];
    }

    public static string GetDisplayAssemblyQualifiedName(this Type type)
    {
        return $"{type.GetFullNameInternal()}, {type.Assembly.GetDisplayName()}";
    }

    public static string GetDisplayName(this Assembly assembly)
    {
        var assemblyName = assembly.GetName().Name;
        if (string.IsNullOrEmpty(assemblyName))
        {
            return assembly.FullName ?? "UnknownAssembly";
        }

        var startVersionIndex = assemblyName.IndexOf(VERSION_MARK, StringComparison.Ordinal);
        if (startVersionIndex == -1)
        {
            return assemblyName;
        }
        return assemblyName.Substring(0, startVersionIndex - 1);
    }

    private static string GetFullNameInternal(this Type type)
    {
        var fullName = type.FullName ?? type.Name;
        var indexVersion = fullName.IndexOf(VERSION_MARK, StringComparison.Ordinal);
        if (indexVersion == -1)
            return fullName;

        if (type.IsGenericType)
        {
            var indexStartGeneric = fullName.IndexOf(START_GENERIC_MARK, StringComparison.Ordinal) + 1;
            var indexEndGeneric = fullName.LastIndexOf(END_GENERIC_MARK, StringComparison.Ordinal);
            var fullNameWithoutVersion = fullName.Substring(0, indexStartGeneric) + fullName.Substring(indexEndGeneric, 1);

            var genericArguments = type.GetGenericArguments()
              .Select(x => $"[{GetDisplayAssemblyQualifiedName(x)}]")
              .StringJoin(", ");

            return fullNameWithoutVersion.Insert(indexStartGeneric, genericArguments);

            //"System.Collections.Generic.List`1[[
        }
        return fullName.Substring(0, indexVersion - 1);
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

    public static bool IsHashSetType(this Type type)
    {
        type = type.GetUnderlyingType();
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>);
    }

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
            !type.IsSubclassOf<Attribute>() ||
             CustomAttributeExtensions.GetCustomAttribute<IgnorarAtributoTSAttribute>(type) is not null ||
             type.Namespace?.StartsWith("System") == true)
        {
            return false;
        }

        //if (type == typeof(BaseAtributoValidacaoAsync))
        //    return false;

        return type.IsSubclassOf(typeof(BaseAtributoDominio)) ||
               type.ImplementsInterface<IDomainAtributo>();

    }

    public static int GetAbstractLevel(this Type type)
    {
        if (!type.IsClass)
        {
            return 0;
        }
        var count = 0;
        var current = type.BaseType;
        while (current is not null && current != typeof(object))
        {
            count++;
            current = current.BaseType;
        }
        return count;
    }

    public static bool IsStatic(this Type type)
        => type.IsAbstract && type.IsSealed;

    public static bool IsDomainPrimitiveType(this Type type)
    {
        var underlyingType = type.GetUnderlyingType();
        if (type.IsPrimitive)
            return true;

        if (type.IsEnum)
            return false;

        return type == typeof(Guid)
            || type == typeof(string)
            || type == typeof(DateTime)
            || type == typeof(DateTimeOffset)
            || type == typeof(TimeSpan)
            || type == typeof(decimal);
    }

    public static EnumTipoPrimario GetPrimitiveTypeEnum(this Type type)
    {
        return ReflexaoUtil.RetornarTipoPrimarioEnum(type);
    }
}
