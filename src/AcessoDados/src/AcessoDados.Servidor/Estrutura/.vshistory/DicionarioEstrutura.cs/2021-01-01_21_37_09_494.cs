using System;
using System.Collections.Generic;

namespace Snebur.AcessoDados.Estrutura
{
    public class DicionarioEstrutura<T> : Dictionary<string, T>
    {

        public override string ToString()
        {
            return String.Format("Dicionario {0} - {1}", typeof(T),  base.ToString());
        }
    }
}
