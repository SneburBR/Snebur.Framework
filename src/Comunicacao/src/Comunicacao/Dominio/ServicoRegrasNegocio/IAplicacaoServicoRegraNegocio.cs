using Snebur.Dominio.Atributos;

namespace Snebur.Comunicacao.Dominio;

[IgnorarInterfaceTS]
public interface IAplicacaoServicoRegraNegocio
{
    IServicoRegrasNegocioCliente ServicoRegrasNegocio { get; }
}
