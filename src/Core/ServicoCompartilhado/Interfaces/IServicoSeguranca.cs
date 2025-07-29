using Snebur.Comunicacao;

namespace Snebur.Servicos;

public interface IServicoLogSeguranca
{
    Guid NotificarLogSeguranca(string mensagem,
                               string? stackTrace,
                               InfoRequisicao? infoRequisicao,
                               EnumTipoLogSeguranca tipoLogSeguranca,
                               BaseInformacaoAdicionalServicoCompartilhado? informacaoAdicional);
}
