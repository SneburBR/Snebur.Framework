using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Snebur.Serializacao
{
    internal class FieldValueProvider : IValueProvider
    {
        private FieldInfo _field;

        public FieldValueProvider(FieldInfo field)
        {
            this._field = field;
        }

        public void SetValue(object target, object? value)
        {
            this._field.SetValue(target, value);
        }

        public object? GetValue(object target)
        {
            return this._field?.GetValue(target);
        }
    }
}
