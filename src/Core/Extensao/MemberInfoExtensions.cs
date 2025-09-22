using System.Reflection;

namespace Snebur;

public static class MemberInfoExtensions
{
    public static T GetRequiredCustomAttribute<T>(this MemberInfo element) where T : Attribute
    {
        var attribute = element.GetCustomAttribute<T>();
        if (attribute is null)
        {
            throw new InvalidOperationException(
                $"The attribute '{typeof(T).Name}' is not defined on '{element.Name}'.");
        }
        return attribute;
    }
}

