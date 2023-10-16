using Snebur.Dominio;
using Snebur.Utilidade;
using System.Linq;
using System.Reflection;

namespace Snebur.AcessoDados.Migracao
{
    public partial class ReflectionEntityUtil
    {

        public static bool PropriedadeRetornaBaseEntidade(PropertyInfo pi)
        {
            if (pi.PropertyType.IsSubclassOf(typeof(Entidade)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool PropriedadeRetornaColecaoBaseEntidade(PropertyInfo pi)
        {
            if (ReflexaoUtil.IsPropriedadeRetornaColecao(pi) && pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericArguments().Count() == 1 && pi.PropertyType.GetGenericArguments().Single().IsSubclassOf(typeof(Entidade)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}