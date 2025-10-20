using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace Snebur.Helpers;

public static class NullabilityHelpers
{
    private static readonly NullabilityInfoContext _context = new();
    private static readonly ConcurrentDictionary<object, MemberNullabilityInfoBase> _cache = new();

    public static bool IsNullable(this PropertyInfo property)
    {
        Guard.NotNull(property);

        if (property.PropertyType.IsNullableType())
            return true;

        if (property.PropertyType.IsValueType)
            return false;

        return GetNullabilityInfo(property).IsMemberNullable;
    }

    public static bool IsNullable(this ParameterInfo parameter)
    {
        Guard.NotNull(parameter);

        if (parameter.ParameterType.IsNullableType())
            return true;

        if (parameter.ParameterType.IsValueType)
            return false;

        return GetNullabilityInfo(parameter).IsMemberNullable;
    }

    public static bool IsPrimaryKey(this PropertyInfo property)
    {
        Guard.NotNull(property);
        return property.GetCustomAttribute<KeyAttribute>(false) is not null;
    }
    public static bool IsReturnTypeNullable(this MethodInfo method)
    {
        Guard.NotNull(method);

        if (method.ReturnType.IsNullableType())
            return true;

        if (method.ReturnType.IsValueType)
            return false;

        return GetNullabilityInfo(method).IsMemberNullable;
    }

    public static bool IsNullable(FieldInfo field)
    {
        Guard.NotNull(field);

        if (field.FieldType.IsNullableType())
            return true;

        if (field.FieldType.IsValueType)
            return false;

        return GetNullabilityInfo(field).IsMemberNullable;
    }

    public static bool IsNullable(this EventInfo eventInfo)
    {
        Guard.NotNull(eventInfo);

        var handlerType = eventInfo.EventHandlerType;
        if (handlerType is null)
            return true;
        if (handlerType.IsNullableType())
            return true;

        if (handlerType.IsValueType)
            return false;

        return GetNullabilityInfo(eventInfo).IsMemberNullable;
    }

    public static MemberNullabilityInfoBase GetNullabilityInfo(ICustomAttributeProvider memberInfo)
    {
        if (Debugger.IsAttached && false)
        {
            Debugger.Break();
            return CreateNullabilityInfo(memberInfo);
        }
        return _cache.GetOrAdd(memberInfo, CreateNullabilityInfo);
    }

    private static MemberNullabilityInfoBase CreateNullabilityInfo(object member)
    {
        return member switch
        {
            PropertyInfo p => CreateNullabilityInfo(p),
            ParameterInfo p => CreateNullabilityInfo(p),
            FieldInfo f => CreateNullabilityInfo(f),
            MethodInfo m => CreateNullabilityInfo(m),
            EventInfo e => CreateNullabilityInfo(e),
            _ => throw new NotSupportedException(
                $"Unsupported member type: {member.GetType().Name}")
        };

    }
    private static MemberNullabilityInfoBase CreateNullabilityInfo(FieldInfo fieldInfo)
    {
        var info = _context.Create(fieldInfo);
        return CreateMemberInfoNullabilityInternal(info, fieldInfo.FieldType);
    }

    private static MemberNullabilityInfoBase CreateNullabilityInfo(PropertyInfo prop)
    {
        var info = _context.Create(prop);
        return CreateMemberInfoNullabilityInternal(info, prop.PropertyType);
    }

    private static MemberNullabilityInfoBase CreateNullabilityInfo(ParameterInfo parameter)
    {

        try
        {
            var info = _context.Create(parameter);
            return CreateMemberInfoNullabilityInternal(info, parameter.ParameterType);
        }
        catch (Exception)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
                return CreateNullabilityInfo(parameter);
            }
            throw;
        }
    }

    private static MemberNullabilityInfoBase CreateNullabilityInfo(MethodInfo method)
    {
        var info = _context.Create(method.ReturnParameter);
        return CreateMemberInfoNullabilityInternal(info, method.ReturnType);
    }

    private static MemberNullabilityInfoBase CreateNullabilityInfo(EventInfo eventInfo)
    {
        var info = _context.Create(eventInfo);
        return CreateMemberInfoNullabilityInternal(info, eventInfo.EventHandlerType!);
    }

    private static MemberNullabilityInfoBase CreateMemberInfoNullabilityInternal(
        NullabilityInfo memberInfo,
        Type memberType)
    {
        if (memberType.IsDictionaryType() == true)
        {
            return CreateMemberInfoNullabilityFromDictionary(memberInfo, memberType);
        }

        if (memberType.IsCollectionType() == true)
        {
            return CreateMemberInfoNullabilityFromCollection(memberInfo, memberType);
        }

        return new MemberNullabilityInfo
        {
            MemberInfo = memberInfo,
            MemberType = memberType,
            MemberState = memberInfo.ReadState,
        };
    }

    private static CollectionMemberNullabilityInfo CreateMemberInfoNullabilityFromCollection(
        NullabilityInfo memberInfo,
        Type memberType)
    {
        var itemType = memberType.GetCollectionItemType();
        Guard.NotNull(itemType);

        if (memberType.IsArray)
        {
            // Arrays use ElementType
            var itemInfo = memberInfo.ElementType;
            if (itemInfo is null)
            {
                throw new Exception("Element type is null for array type.");
            }

            if (itemInfo.Type != itemType)
            {
                throw new Exception("Element type mismatch in array type.");
            }

            return new CollectionMemberNullabilityInfo
            {
                MemberType = memberType,
                MemberInfo = memberInfo,
                MemberState = memberInfo.ReadState,
                ItemState = itemInfo.ReadState,
                ItemInfo = itemInfo,
                ItemType = itemType,
            };

        }

        if (memberType.IsGenericType)
        {
            var itemInfo = memberInfo.GenericTypeArguments.FirstOrDefault();
            if (itemInfo is null)
            {
                throw new Exception("Generic type argument is null for generic collection type.");
            }

            return new CollectionMemberNullabilityInfo
            {
                MemberType = memberType,
                MemberState = memberInfo.ReadState,
                MemberInfo = memberInfo,
                ItemState = itemInfo.ReadState,
                ItemType = itemType,
                ItemInfo = itemInfo,
            };
        }

        throw new NotSupportedException(
            $" The collection type '{memberType.FullName}' is not supported.");
    }

    private static DictionaryNullabilityInfo CreateMemberInfoNullabilityFromDictionary(
        NullabilityInfo memberInfo,
        Type memberType)
    {
        var args = memberInfo.GenericTypeArguments;
        if (memberType.IsGenericType)
        {
            var genericArgs = memberInfo.GenericTypeArguments;
            if (genericArgs.Length != 2)
            {
                throw new Exception("Generic type arguments are invalid for dictionary type.");
            }

            var valueType = memberType.GetDictionaryValueType();
            var keyType = genericArgs[0].Type;

            Guard.NotNull(valueType);
            Guard.NotNull(keyType);

            var keyInfo = args[0];
            var valueInfo = args[1];

            if (keyInfo.Type != keyType)
            {
                throw new Exception("Key type mismatch in dictionary type.");
            }

            if (valueInfo.Type != valueType)
            {
                throw new Exception("Value type mismatch in dictionary type.");
            }

            return new DictionaryNullabilityInfo
            {
                MemberType = memberType,
                MemberState = memberInfo.ReadState,
                MemberInfo = memberInfo,
                KeyInfo = keyInfo,
                ValueInfo = valueInfo,
                KeyState = keyInfo.ReadState,
                KeyType = genericArgs[0].Type,
                ValueState = valueInfo.ReadState,
                ValueType = valueType,
            };
        }
        throw new NotSupportedException(
            $" The dictionary type '{memberType.FullName}' is not supported.");

    }
}

public abstract record MemberNullabilityInfoBase
{
    public required NullabilityInfo MemberInfo { get; init; }
    public required Type MemberType { get; init; }
    public required NullabilityState MemberState { get; init; }

    public bool IsMemberNullable
       => this.MemberType?.IsNullableType() == true ||
          this.MemberState == NullabilityState.Nullable;
}

public sealed record MemberNullabilityInfo : MemberNullabilityInfoBase
{

}

public sealed record CollectionMemberNullabilityInfo : MemberNullabilityInfoBase
{
    public required NullabilityInfo ItemInfo { get; init; }
    public required Type ItemType { get; init; }
    public required NullabilityState ItemState { get; init; }

    public bool ItemIsNullable
        => this.ItemType.IsNullableType() == true ||
           this.ItemState == NullabilityState.Nullable;
}

public sealed record DictionaryNullabilityInfo : MemberNullabilityInfoBase
{
    public required Type KeyType { get; init; }
    public required Type ValueType { get; init; }
    public required NullabilityInfo KeyInfo { get; init; }
    public required NullabilityInfo ValueInfo { get; init; }
    public required NullabilityState KeyState { get; init; }
    public required NullabilityState ValueState { get; init; }

    public bool KeyIsNullable
        => this.KeyType.IsNullableType() == true ||
           this.KeyState == NullabilityState.Nullable;
    public bool ValueIsNullable
        => this.ValueType.IsNullableType() == true ||
           this.ValueState == NullabilityState.Nullable;
}

