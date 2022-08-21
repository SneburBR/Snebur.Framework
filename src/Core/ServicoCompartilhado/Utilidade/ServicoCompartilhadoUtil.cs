using System;

namespace Snebur.Servicos
{
    public class ServicoCompartilhadoUtil
    {
        public static BaseInformacaoAdicionalServicoCompartilhado RetornarInformacaoAdicionalServicoCompartilhado()
        {
            try
            {
                return AplicacaoSnebur.Atual.FuncaoRetornarInformacaoAdicionalServicoCompartilhado?.Invoke();
            }
            catch (Exception)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    throw;
                }
                return null;
            }
        }
    }
}
