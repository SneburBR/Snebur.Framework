using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Snebur.AcessoDados.Migracao
{
    public class BaseContextoUtil
    {

        private static readonly object _bloqueio = new object();

        public static Type[] RetornarTiposBaseEntidade(BaseContextoEntity baseContexto)
        {
            lock (BaseContextoUtil._bloqueio)
            {
                var tipoContexto = baseContexto.GetType();
                var resultado = new List<Type>();
                var tipsoEntidades = tipoContexto.GetProperties().Where(x => x.PropertyType.IsGenericType && x.PropertyType.Name.Contains("DbSet")).Select(x => x.PropertyType.GetGenericArguments().Single()).ToList();

                if (tipsoEntidades.Count == 0)
                {
                    throw new Exception(string.Format("Nenhum DbSet definido no contexto '{0}'.", tipoContexto.Name));
                }
                foreach (var tipo in tipsoEntidades)
                {
                    resultado.Add(tipo);
                }
                return resultado.Distinct().OrderBy(x => x.Name).ToArray();
            }
        }
    }
}