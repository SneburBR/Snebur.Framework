namespace Snebur.AcessoDados;

public class IntercepetadorModel
{
    public Type TipoEntidade { get; }
    public Type[] TiposInterceptador { get; }
    public IInterceptador Instancia { get; }

    public IntercepetadorModel(
        Type tipoEntidade,
        Type[] types)
    {
        this.TipoEntidade = tipoEntidade;
        this.TiposInterceptador = types;
        this.Instancia = Activator.CreateInstance(this.TiposInterceptador[0]) as IInterceptador ??
                            throw new InvalidOperationException("Não foi possível criar a instância do interceptador.");

        for (var i = 1; i < this.TiposInterceptador.Length; i++)
        {
            var intaciaBase = Activator.CreateInstance(this.TiposInterceptador[i]) as IInterceptador ??
                                throw new InvalidOperationException("Não foi possível criar a instância do interceptador.");

            this.Instancia.SetInterceptadorBase(intaciaBase);
        }
    }
}
