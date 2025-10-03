using System.Reflection;

namespace Snebur.Reflexao;

public class CustomParameterInfo : ParameterInfo
{
    private readonly MemberInfo _member;
    private readonly string _name;
    private readonly Type _parameterType;
    private readonly int _position;
    private readonly ParameterAttributes _attributes;
    private readonly bool _hasDefaultValue;
    private readonly object? _defaultValue;
    private readonly IReadOnlyList<Attribute> _attributesList;

    public CustomParameterInfo(
        MemberInfo member,
        string name,
        Type parameterType,
        int position,
        ParameterAttributes attributes = ParameterAttributes.None,
        bool hasDefaultValue = false,
        object? defaultValue = null,
        IEnumerable<Attribute>? customAttributes = null)
    {
        _member = member ?? throw new ArgumentNullException(nameof(member));
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _parameterType = parameterType ?? throw new ArgumentNullException(nameof(parameterType));
        _position = position;
        _attributes = attributes;
        _hasDefaultValue = hasDefaultValue;
        _defaultValue = defaultValue;

        _attributesList = (customAttributes as IReadOnlyList<Attribute>)
                          ?? customAttributes?.ToArray()
                          ?? Array.Empty<Attribute>();
    }

    public override MemberInfo Member
        => _member;

    public override string Name
        => _name;

    public override Type ParameterType
        => _parameterType;

    public override int Position
        => _position;

    public override ParameterAttributes Attributes
        => _attributes;

    public override bool HasDefaultValue
        => _hasDefaultValue;

    public override object? DefaultValue
    {
        get
        {
            if (!_hasDefaultValue)
                throw new FormatException("Parameter has no default value.");
            return _defaultValue;
        }
    }

    public override object? RawDefaultValue
        => _hasDefaultValue ? _defaultValue : DBNull.Value;

    public override IEnumerable<CustomAttributeData> CustomAttributes
        => Enumerable.Empty<CustomAttributeData>();

    public override object[] GetCustomAttributes(bool inherit)
    {
        if (_attributesList.Count == 0)
            return Array.Empty<Attribute>();

        var result = new Attribute[_attributesList.Count];
        for (int i = 0; i < _attributesList.Count; i++)
        {
            result[i] = _attributesList[i];
        }
        return result;
    }

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
        if (attributeType == null)
            throw new ArgumentNullException(nameof(attributeType));

        if (_attributesList.Count == 0)
            return Array.Empty<Attribute>();

        return _attributesList
            .Where(a => attributeType.IsInstanceOfType(a)).Cast<Attribute>()
            .ToArray();
    }

    public override bool IsDefined(Type attributeType, bool inherit)
    {
        if (attributeType == null)
            throw new ArgumentNullException(nameof(attributeType));
        return _attributesList.Any(a => attributeType.IsInstanceOfType(a));
    }

    public override IList<CustomAttributeData> GetCustomAttributesData()
    {
        return Enumerable.Empty<CustomAttributeData>().ToList();
    }

    // Convenience helpers
    public static CustomParameterInfo In(
        MemberInfo member,
        string name,
        Type type,
        int position,
        bool hasDefaultValue = false,
        object? defaultValue = null,
        IEnumerable<Attribute>? customAttributes = null)
        => new CustomParameterInfo(
            member,
            name,
            type,
            position,
            ParameterAttributes.In,
            hasDefaultValue,
            defaultValue,
            customAttributes);

    public static CustomParameterInfo Out(
        MemberInfo member,
        string name,
        Type type,
        int position,
        IEnumerable<Attribute>? customAttributes = null)
        => new CustomParameterInfo(
            member,
            name,
            type.MakeByRefType(),
            position,
            ParameterAttributes.Out,
            hasDefaultValue: false,
            defaultValue: null,
            customAttributes);

    public static CustomParameterInfo Optional(
        MemberInfo member,
        string name,
        Type type,
        int position,
        object? defaultValue = null,
        IEnumerable<Attribute>? customAttributes = null)
        => new CustomParameterInfo(
            member,
            name,
            type,
            position,
            ParameterAttributes.Optional,
            hasDefaultValue: true,
            defaultValue,
            customAttributes);
}

