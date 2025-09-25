using System.Reflection;

namespace Snebur.Extensao;

public static class TypeExtensions
{
    public static Type GetUnderlyingType(this Type type)
    {

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return Nullable.GetUnderlyingType(type)
                ?? type;
        }
        return type;

    }

    public static bool IsSubclassOrEqualTo(this Type type, Type baseType)
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

    public static bool IsObsolete(this Type type)
    {
        return type.GetCustomAttribute<ObsoleteAttribute>(false) != null;
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
}
