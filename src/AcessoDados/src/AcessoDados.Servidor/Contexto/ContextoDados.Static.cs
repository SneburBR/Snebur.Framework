using System.Collections.Generic;

namespace Snebur.AcessoDados
{
    public abstract partial class BaseContextoDados
    {
        //public static void Inicializar<TContexto>() where TContexto : ContextoDados
        //{
        //    EstruturaBancoDados.Inicializar(typeof(TContexto));
        //}
        internal HashSet<IInterceptador> InterceptoresAtivos { get; }= new HashSet<IInterceptador>();
    }
}