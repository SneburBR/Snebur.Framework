using Snebur.Comunicacao;

namespace Snebur.AcessoDados;

//[IgnorarServicoTS]
public interface IServicoDados : IBaseServico
{
    object? RetornarValorScalar(EstruturaConsulta estruturaConsulta);

    ResultadoConsulta RetornarResultadoConsulta(EstruturaConsulta estruturaConsulta);

    ResultadoSalvar Salvar(IEnumerable<IEntidade> entidades);

    ResultadoDeletar Deletar(IEnumerable<IEntidade> entidades, string relacoesEmCascata);

    DateTime RetornarDataHora();

    DateTime RetornarDataHoraUTC();
}