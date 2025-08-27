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
}
