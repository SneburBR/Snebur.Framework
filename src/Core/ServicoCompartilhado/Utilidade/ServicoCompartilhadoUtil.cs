using Snebur.Utilidade;

namespace Snebur.Servicos;

public class ServicoCompartilhadoUtil
{
    public static BaseInformacaoAdicionalServicoCompartilhado? RetornarInformacaoAdicionalServicoCompartilhado()
    {
        try
        {
            return AplicacaoSnebur.AtualRequired.FuncaoRetornarInformacaoAdicionalServicoCompartilhado?.Invoke();
        }
        catch (Exception)
        {
            if (DebugUtil.IsAttached)
            {
                throw;
            }
            return null;
        }
    }
}
