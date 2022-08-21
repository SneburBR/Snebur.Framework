using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Snebur.Utilidade;

namespace Snebur.AcessoDados
{
    public class ContextoDadosUtil
    {
        public  static List<PropertyInfo> RetornarPropriedadesIConsultaEntidade(Type tipoContexto)
        {
            return tipoContexto.GetProperties(ReflexaoUtil.BindingFlags).
                                  Where(x => x.PropertyType.IsInterface &&
                                             ReflexaoUtil.TipoImplementaInterface(x.PropertyType, typeof(IConsultaEntidade), false)).ToList();
        }
    }
}