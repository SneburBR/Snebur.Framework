using System;
using System.Collections.Generic;

namespace Snebur.AcessoDados.Estrutura
{
    internal partial class EstruturaBancoDados
    {
        private static readonly object _bloqueioInstancias = new object();
        private static readonly Dictionary<Type, EstruturaBancoDados> Instancias = new Dictionary<Type, EstruturaBancoDados>();

        internal static EstruturaBancoDados RetornarEstruturaBancoDados(BaseContextoDados baseContexto,
                                                                        BancoDadosSuporta sqlSuporte)
        {
            var tipoContexto = baseContexto.GetType();
            if (!Instancias.ContainsKey(tipoContexto))
            {
                lock (_bloqueioInstancias)
                {
                    if (!Instancias.ContainsKey(tipoContexto))
                    {
                        var estruturaBanco = new EstruturaBancoDados(baseContexto, sqlSuporte);
                        Instancias.Add(tipoContexto, estruturaBanco);
                    }
                }
            }
            return Instancias[tipoContexto];
        }
    }
}
