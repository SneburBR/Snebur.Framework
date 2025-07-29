using Snebur.Dominio.Atributos;

namespace Snebur.Comunicacao.Dominio.ServicoRegrasNegocio
{
    [IgnorarInterfaceTS]
    public interface IAplicacaoServicoRegraNegocio
    {
        IServicoRegrasNegocioCliente ServicoRegrasNegocio { get; }
    }
}
