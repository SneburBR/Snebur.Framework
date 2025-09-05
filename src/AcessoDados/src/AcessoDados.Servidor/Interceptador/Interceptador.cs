namespace Snebur.AcessoDados
{
    public interface IInterceptador
    {
        List<Entidade> __Interceptar(BaseContextoDados contexto, List<Entidade> entidades);

        void SetInterceptadorBase(IInterceptador interceptador);
    }

    public interface IInterceptador<TContexto, TEntidade> : IInterceptador
                                                            where TContexto : BaseContextoDados
                                                            where TEntidade : Entidade
    {
        IEnumerable<TEntidade> Interceptar(TContexto contexto, IEnumerable<TEntidade> entidades);
    }

    public abstract class Interceptador<TContexto, TEntidade> : IInterceptador,
                                                                IInterceptador<TContexto, TEntidade>
                                                                where TContexto : BaseContextoDados
                                                                where TEntidade : Entidade
    {
        internal IInterceptador InterceptadorBase { get; private set; }

        public List<Entidade> __Interceptar(BaseContextoDados contexto,
                                            List<Entidade> entidades)
        {
            var entidadesInterceptadas = this.Interceptar((TContexto)contexto,
                                                          entidades.Cast<TEntidade>());
            if (this.InterceptadorBase != null)
            {
                return this.InterceptadorBase.__Interceptar(contexto, entidades);
            }
            return entidadesInterceptadas.ToList<Entidade>();
        }

        public abstract IEnumerable<TEntidade> Interceptar(TContexto contexto,
                                                           IEnumerable<TEntidade> entidades);

        public void SetInterceptadorBase(IInterceptador interceptador)
        {
            if (this.InterceptadorBase == null)
            {
                this.InterceptadorBase = interceptador;
                return;
            }
            this.InterceptadorBase.SetInterceptadorBase(interceptador);
        }
    }
}
