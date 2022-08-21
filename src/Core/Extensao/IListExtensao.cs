using System.Collections.Generic;

namespace System
{
    public static class IListExtensao
    {
        public static List<(T, int)> ToTupleItemIndex<T>(this IList<T> lista)
        {
            var retorno = new List<(T, int)>();
            var len = lista.Count;
            for (var i = 0; i < len; i++)
            {
                var item = lista[i];
                retorno.Add((item, i));
            }
            return retorno;
        }

        public static List<(T, double)> ToTupleItemProgresso<T>(this IList<T> lista)
        {
            var retorno = new List<(T, double)>();
            double len = lista.Count;
            for (var i = 0; i < len; i++)
            {
                var p = ((i + 1) / len) * 100;
                var item = lista[i];
                retorno.Add((item, p));
            }
            return retorno;
        }
    }
}
