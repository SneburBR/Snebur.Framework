using Snebur.AcessoDados;
using Snebur.Dominio.Atributos;

namespace Snebur.Comunicacao;

[IgnorarInterfaceTS]
public interface IAplicacaoServicoRegraNegocio
{
    IServicoRegrasNegocioCliente ServicoRegrasNegocio { get; }
}
