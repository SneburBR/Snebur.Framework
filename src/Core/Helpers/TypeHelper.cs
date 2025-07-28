using System;

namespace Snebur.Helpers
{
    public static class TypeHelper
    {
        public static T GetUnderlyingDefaultValue<T>()
        {
            var type = typeof(T);
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType is not null)
            {
                return (T)Activator.CreateInstance(underlyingType)!;
            }
            return default!;
        }

        public static object? GetDefaultValue(Type tipo)
        {
            if (!tipo.IsValueType)
            {
                return null;
            }

            if (tipo.IsGenericType &&
                tipo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return null;
            }
            return Activator.CreateInstance(tipo);
        }

       
    }
}
