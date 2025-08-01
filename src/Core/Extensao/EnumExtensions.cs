﻿using System.ComponentModel;
using System.Reflection;

namespace Snebur;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        if (field == null) 
            return value.ToString();
        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute != null ? attribute.Description : value.ToString();
    }
}
