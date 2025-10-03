using System.Globalization;
using System.Reflection;

namespace Snebur.Reflexao;

public class EmptyConstructorInfo : ConstructorInfo
{
    private readonly Type _declaringType;
    private readonly ConstructorInfo? _realCtor;

    public EmptyConstructorInfo(Type declaringType)
    {
        _declaringType = declaringType
            ?? throw new ArgumentNullException(nameof(declaringType));

        _realCtor = declaringType.GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            types: Type.EmptyTypes,
            modifiers: null);
    }

    public override Type? DeclaringType
        => _declaringType;

    public override Type? ReflectedType
        => _declaringType;

    public override string Name
        => _realCtor?.Name ?? ".ctor";

    public override MethodAttributes Attributes
        => _realCtor?.Attributes
        ?? (MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.HideBySig);

    public override RuntimeMethodHandle MethodHandle
        => _realCtor?.MethodHandle ?? default;

    public override MethodImplAttributes GetMethodImplementationFlags()
        => _realCtor?.GetMethodImplementationFlags() ?? MethodImplAttributes.IL;

    public override ParameterInfo[] GetParameters()
        => Array.Empty<ParameterInfo>();

    public override object[] GetCustomAttributes(bool inherit)
        => Array.Empty<Attribute>();

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        => Array.Empty<Attribute>();

    public override bool IsDefined(Type attributeType, bool inherit)
        => false;

    public override object Invoke(BindingFlags invokeAttr, Binder? binder, object?[]? parameters, CultureInfo? culture)
    {
        EnsureNoParameters(parameters);

        if (_realCtor is not null)
        {
            // Target is a constructor, so obj is ignored, we just invoke it
            return _realCtor.Invoke(null);
        }

        // No real parameterless ctor, try to create anyway
        // Uses nonPublic true to allow private parameterless constructors if present
        // This throws MissingMethodException if the runtime cannot create the instance
        return Activator.CreateInstance(_declaringType, nonPublic: true)
               ?? throw new TargetInvocationException("Activator.CreateInstance returned null", null);
    }

    public override object? Invoke(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? parameters, CultureInfo? culture)
    {
        // The instance parameter is ignored for constructors, match ConstructorInfo behavior
        return Invoke(invokeAttr, binder, parameters, culture);
    }

    private static void EnsureNoParameters(object?[]? parameters)
    {
        if (parameters is { Length: > 0 })
        {
            throw new TargetParameterCountException("This constructor does not take parameters.");
        }
    }

    public override IList<CustomAttributeData> GetCustomAttributesData()
    {
        return Enumerable.Empty<CustomAttributeData>().ToList();
    }
}

