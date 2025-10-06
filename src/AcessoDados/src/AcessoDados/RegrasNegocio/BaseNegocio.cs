using Snebur.AcessoDados;
using Snebur.Comunicacao;

namespace Snebur.RegrasNegocio;

public abstract class BaseNegocio : IBaseNegocio
{

}

public abstract class BaseNegocio<TContextoDados> : BaseNegocio
    where TContextoDados : __BaseContextoDados
{
    public TContextoDados ContextoDados { get; }

    public BaseNegocio(TContextoDados contexto)
    {
        Guard.NotNull(contexto);
        this.ContextoDados = contexto;
    }
}
