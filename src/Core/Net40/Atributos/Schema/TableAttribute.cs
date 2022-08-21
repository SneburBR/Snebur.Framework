using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations.Schema
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {
        public string Schema { get; set; }
        public string Name { get; set; }

        public TableAttribute(string name)
        {
            this.Name = name;
        }


    }
}