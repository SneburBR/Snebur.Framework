using System;

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

        internal static EstruturaBancoDados RetornarEstruturaBancoDados(Type tipoContexto, SqlSuporte sqlSuporte)
        {
            if (EstruturaBancoDados._estruturaBancoDados == null)
            {
                lock (EstruturaBancoDados.Bloqueio)
                {
                    if (EstruturaBancoDados._estruturaBancoDados == null)
                    {
                        EstruturaBancoDados._estruturaBancoDados = new EstruturaBancoDados(tipoContexto, sqlSuporte);
                    }
                }
            }
            return _estruturaBancoDados;
        }
    }
}
