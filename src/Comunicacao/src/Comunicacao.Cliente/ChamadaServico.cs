using System;
using System.Collections.Generic;

namespace Snebur.Comunicacao
{

    public class ChamadaServico : BaseChamadaServico
    {

        public ChamadaServico(string nomeManipulador,
                              ContratoChamada contratoChamada,
                              string urlServico,
                              Type tipoRetorno,
                              Dictionary<string, string> parametrosCabeacalhoAdicionais) :
                              base(nomeManipulador, contratoChamada, urlServico, tipoRetorno, parametrosCabeacalhoAdicionais)
        {
        }

        public object ExecutarChamada()
        {
            return this.RetornarValorChamada();
        }

    }

}
