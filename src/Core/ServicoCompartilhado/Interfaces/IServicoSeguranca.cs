using System;

namespace Snebur.Servicos
{
    public interface IServicoLogSeguranca
    {
        Guid NotificarLogSeguranca(string mensagem,
                                   string stackTrace,
                                   EnumTipoLogSeguranca tipoLogSeguranca,
                                   BaseInformacaoAdicionalServicoCompartilhado informacaoAdicional);
    }
}
