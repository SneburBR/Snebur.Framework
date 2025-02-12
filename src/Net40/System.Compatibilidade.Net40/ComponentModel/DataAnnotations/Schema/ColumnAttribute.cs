namespace System.ComponentModel.DataAnnotations.Schema
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }

        public ColumnAttribute()
        {

        }
        public ColumnAttribute(string name)
        {
            this.Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DatabaseGeneratedAttribute: Attribute
    {
       public DatabaseGeneratedOption DatabaseGeneratedOption { get; set; }

        public DatabaseGeneratedAttribute()
        {
        }

    }
    //
    // Summary:
    //     Represents the pattern used to generate values for a property in the database.
    public enum DatabaseGeneratedOption
    {
        //
        // Summary:
        //     The database does not generate values.
        None = 0,
        //
        // Summary:
        //     The database generates a value when a row is inserted.
        Identity = 1,
        //
        // Summary:
        //     The database generates a value when a row is inserted or updated.
        Computed = 2
    }
}
