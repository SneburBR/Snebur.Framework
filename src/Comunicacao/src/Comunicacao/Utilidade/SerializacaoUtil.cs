//using Snebur.Utilidade;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using Snebur.Dominio.Atributos;

//namespace Snebur.Comunicacao
//{
//    public class SerializacaoUtil
//    {
//        public static List<PropertyInfo> RetornarPropriedades(Type tipo)
//        {
//            var propriedades = ReflexaoUtil.RetornarPropriedades(tipo, false);
//            propriedades = propriedades.Where(x => x.CanRead && x.CanWrite &&
//                                                   x.GetGetMethod() != null && x.GetGetMethod().IsPublic &&
//                                                   x.GetSetMethod() != null && x.GetSetMethod().IsPublic).
//                                        Where(x =>
//            {
//                var atributoPropriedadeInterface = x.GetCustomAttribute(typeof(PropriedadeInterfaceAttribute));
//                return atributoPropriedadeInterface == null;

//            }).ToList();
//            return propriedades;
//        }
//    }
//}