namespace System.ComponentModel.DataAnnotations.Schema
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ForeignKeyAttribute : Attribute
    {
        public string Name { get; }
        public ForeignKeyAttribute(string name)
        {
            this.Name = name;
        }
    }
}