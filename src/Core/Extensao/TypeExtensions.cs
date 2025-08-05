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
}
