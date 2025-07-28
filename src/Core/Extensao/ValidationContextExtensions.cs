using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Snebur
{
    public static class ValidationContextExtensions
    {
        public static PropertyInfo GetRequiredProperty(
            this ValidationContext context )
        {
            Guard.NotNull(context);
            Guard.NotNullOrWhiteSpace(context.MemberName);

            var propertyName = context.MemberName;
            var property = context.ObjectType.GetProperty(propertyName);
            if (property == null)
            {
                throw new ArgumentException($"Property '{propertyName}' not found on type '{context.ObjectType.FullName}'.", nameof(propertyName));
            }
            return property;
        }
    }
}
