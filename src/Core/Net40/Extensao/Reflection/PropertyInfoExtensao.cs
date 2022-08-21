using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    public static class PropertyInfoExtensao
    {
        public static void SetValue(this PropertyInfo propriedade, object obj, object value)
        {
            propriedade.SetValue(obj, value, null);
        }

        public static object GetValue(this PropertyInfo propriedade, object obj)
        {
            return propriedade.GetValue(obj, null);
        }

        public static IEnumerable<Attribute> GetCustomAttributes(this PropertyInfo propriedade)
        {
            return propriedade.GetCustomAttributes(true).OfType<Attribute>();
        }

        public static T GetCustomAttribute<T>(this PropertyInfo origem) where T : Attribute
        {
            return origem.GetCustomAttribute<T>(true);
        }
        public static T GetCustomAttribute<T>(this ParameterInfo propriedade, bool inherit) where T : Attribute
        {
            return propriedade.GetCustomAttributes(typeof(T), inherit).Single() as T;
        }

        public static Attribute GetCustomAttribute(this Assembly origem, Type attributeType)
        {
            return origem.GetCustomAttributes(attributeType).SingleOrDefault();
        }

        public static Attribute GetCustomAttribute(this Module origem, Type attributeType)
        {
            return origem.GetCustomAttributes(attributeType).SingleOrDefault();
        }

        public static Attribute GetCustomAttribute(this MemberInfo origem, Type attributeType)
        {
            return origem.GetCustomAttributes(attributeType).SingleOrDefault();
        }

        public static Attribute GetCustomAttribute(this MemberInfo origem, Type attributeType, bool inherit)
        {
            return origem.GetCustomAttributes(attributeType, inherit).SingleOrDefault() as Attribute;
        }

        public static Attribute GetCustomAttribute(this ParameterInfo origem, Type attributeType, bool inherit)
        {
            return origem.GetCustomAttributes(attributeType, inherit).SingleOrDefault() as Attribute;
        }

        public static T GetCustomAttribute<T>(this MemberInfo origem) where T : Attribute
        {
            return origem.GetCustomAttributes<T>(true).SingleOrDefault();
        }

        public static T GetCustomAttribute<T>(this Assembly origem) where T : Attribute
        {
            return origem.GetCustomAttributes(typeof(T), true).SingleOrDefault() as T;
        }

        public static T GetCustomAttribute<T>(this Module element) where T : Attribute
        {
            throw new NotImplementedException();
        }

        public static T GetCustomAttribute<T>(this MemberInfo element, bool inherit) where T : Attribute
        {
            throw new NotImplementedException();
        }


        public static IEnumerable<Attribute> GetCustomAttributes(this Assembly element)
        {
            throw new NotImplementedException();
        }
        public static IEnumerable<Attribute> GetCustomAttributes(this Module element)
        {
            throw new NotImplementedException();
        }
        public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element)
        {
            throw new NotImplementedException();
        }
        public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element)
        {
            throw new NotImplementedException();
        }
        public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo origem, Type attributeType)
        {
            return origem.GetCustomAttributes(attributeType, true).OfType<Attribute>();
        }
        public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element, Type attributeType)
        {
            throw new NotImplementedException();
        }
        public static IEnumerable<Attribute> GetCustomAttributes(this Module element, Type attributeType)
        {
            throw new NotImplementedException();
        }
        public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element, bool inherit)
        {
            throw new NotImplementedException();
        }
        public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element, bool inherit)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<Attribute> GetCustomAttributes(this Assembly element, Type attributeType)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<Attribute> GetCustomAttributes(this Assembly origem, Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element, Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this Assembly element) where T : Attribute
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this Module element) where T : Attribute
        {
            throw new NotImplementedException();
        }
        public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo origem) where T : Attribute
        {
            var atributos = origem.GetCustomAttributes(typeof(T), true);
            return atributos.OfType<T>();
        }
        public static IEnumerable<T> GetCustomAttributes<T>(this ParameterInfo element) where T : Attribute
        {
            throw new NotImplementedException();
        }
        public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo element, bool inherit) where T : Attribute
        {
            throw new NotImplementedException();
        }
        public static IEnumerable<T> GetCustomAttributes<T>(this ParameterInfo element, bool inherit) where T : Attribute
        {
            throw new NotImplementedException();
        }

    }
}
