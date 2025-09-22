using Snebur.Extensao;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Snebur;

public static class PropertyInfoExtensions
{
    public static PropertyInfo GetRequiredOneToOneNavigationProperty(this PropertyInfo propertyInfo)
    {
        var declaringType = propertyInfo.DeclaringType;
        Guard.NotNull(declaringType);
        var proprities = declaringType.GetProperties()
            .Where(p => p.GetCustomAttribute<ForeignKeyAttribute>(false)?.Name == propertyInfo.Name)
            .ToArray();

        if (proprities.Length == 0)
        {
            throw new InvalidOperationException(
                $"The property '{propertyInfo.Name}' that is was a foreign key " +
                $"But the related property with {nameof(ForeignKeyAttribute)}(nameof({propertyInfo.Name})) one to one was not found ");
        }
        if (proprities.Length > 1)
        {
            throw new InvalidOperationException(
                $"The property '{propertyInfo.Name}' that is was a foreign key " +
                $"But more than one related property with {nameof(ForeignKeyAttribute)}(nameof({propertyInfo.Name})) one to one was found ");
        }
        return proprities[0];
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

}

