using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.AcessoDados.Estrutura
{
    internal partial class EstruturaBancoDados
    {
        private static EstruturaBancoDados _estruturaBancoDados;

        private static object Bloqueio = new object();

        public static EstruturaBancoDados Atual
        {
            get
            {
                return _estruturaBancoDados;
            }
        }

        internal static EstruturaBancoDados RetornarEstruturaBancoDados(Type tipoContexto, bool IsSemUsuario)
        {
            if (EstruturaBancoDados._estruturaBancoDados == null)
            {
                lock (EstruturaBancoDados.Bloqueio)
                {
                    if (EstruturaBancoDados._estruturaBancoDados == null)
                    {
                        EstruturaBancoDados._estruturaBancoDados = new EstruturaBancoDados(tipoContexto, IsSemUsuario);
                    }
                }
            }
            return _estruturaBancoDados;
        }
    }
}
