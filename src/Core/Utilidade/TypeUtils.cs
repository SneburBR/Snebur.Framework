namespace Snebur.Utilidade;

public static class TypeUtils
{
    public static Type GetUnderlyingType(Type type)
    {
        if (type.IsEnum)
            return Enum.GetUnderlyingType(type);

        return Nullable.GetUnderlyingType(type) ?? type;
    }


}
