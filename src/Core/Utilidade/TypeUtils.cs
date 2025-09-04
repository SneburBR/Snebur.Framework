namespace Snebur.Utilidade;

public static class TypeUtils
{
    public static Type GetUnderlyingType(Type type)
    {
        if (type.IsEnum)
            return Enum.GetUnderlyingType(type);

        return Nullable.GetUnderlyingType(type) ?? type;
    }

    public static bool IsNumberType(Type type)
    {
        type = ReflexaoUtil.RetornarTipoSemNullable(type);
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
}
