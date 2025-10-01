using System.Reflection;

namespace Snebur;

public static class MemberInfoExtensions
{
    public static T GetRequiredCustomAttribute<T>(this MemberInfo element) where T : Attribute
    {
        var attribute = CustomAttributeExtensions.GetCustomAttribute<T>(element);
        if (attribute is null)
        {
            throw new InvalidOperationException(
                $"The attribute '{typeof(T).Name}' is not defined on '{element.Name}'.");
        }
        return attribute;
    }

    public static TAttribute? GetCustomMemberAttribute<TAttribute>(
        this ICustomAttributeProvider memberInfo,
        bool inherit = true)
        where TAttribute : Attribute
    {
        return memberInfo.GetCustomMemberAttribute<TAttribute>(typeof(TAttribute), false) as TAttribute;
    }

    public static Attribute? GetCustomMemberAttribute<TAttribute>(
        this ICustomAttributeProvider memberInfo,
        Type attributeType,
        bool inherit = true)
    {
        if (!attributeType.IsSubclassOf<Attribute>())
            throw new ArgumentException("The attributeType must be a subclass of Attribute.", nameof(attributeType));

        if (memberInfo is MemberInfo mi)
            return CustomAttributeExtensions.GetCustomAttribute(mi, attributeType, inherit);

        return memberInfo.GetCustomAttributes(attributeType, inherit).FirstOrDefault() as Attribute;
    }

    public static Type GetMemberType(this ICustomAttributeProvider memberInfo)
    {
        return memberInfo switch
        {
            PropertyInfo pi => pi.PropertyType,
            ParameterInfo pi => pi.ParameterType,
            FieldInfo fi => fi.FieldType,
            EventInfo ei => ei.EventHandlerType ?? throw new InvalidOperationException($"The event '{ei.Name}' does not have an event handler type."),
            MethodInfo mi => mi.ReturnType,
            Type type => type,
            _ => throw new NotSupportedException($"The member type '{memberInfo.GetType().Name}' is not supported."),
        };
    }

    public static string GetMemberName(this ICustomAttributeProvider memberInfo)
    {
        return memberInfo switch
        {
            PropertyInfo pi => pi.Name,
            ParameterInfo pi => pi.Name ?? throw new InvalidOperationException("The parameter does not have a name."),
            FieldInfo fi => fi.Name,
            EventInfo ei => ei.Name,
            MethodInfo mi => mi.Name,
            Type type => type.Name,
            _ => throw new NotSupportedException($"The member type '{memberInfo.GetType().Name}' is not supported."),
        };
    }

    public static string GetDeclaringTypeName(this ICustomAttributeProvider memberInfo)
    {
        return memberInfo switch
        {
            PropertyInfo pi => pi.DeclaringType?.Name ?? "<UnknownDeclaringType>",
            ParameterInfo pi => pi.Member.DeclaringType?.Name ?? "<UnknownDeclaringType>",
            FieldInfo fi => fi.DeclaringType?.Name ?? "<UnknownDeclaringType>",
            EventInfo ei => ei.DeclaringType?.Name ?? "<UnknownDeclaringType>",
            MethodInfo mi => mi.DeclaringType?.Name ?? "<UnknownDeclaringType>",
            Type type => type.Name ?? "<UnknownType>",
            _ => throw new NotSupportedException($"The member type '{memberInfo.GetType().Name}' is not supported."),
        };
    }
}

