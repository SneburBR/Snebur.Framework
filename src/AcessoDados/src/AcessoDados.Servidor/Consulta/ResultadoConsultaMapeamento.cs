using Snebur.Dominio;

namespace Snebur.AcessoDados.Consulta
{
    internal class ResultadoConsultaMapeamento
    {
        internal int TotalRegistros { get; set; }
        internal ListaEntidades<IEntidade> Entidades { get; } = new ListaEntidades<IEntidade>();
    }
}
