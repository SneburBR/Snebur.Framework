using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snebur.Comunicacao
{

    public class ChamadaServicoAsync : BaseChamadaServico
    {

        public Action<ArgsResultadoChamadaServico> Callback { get; set; }
        public object UserState { get; set; }

        public ChamadaServicoAsync(string nomeManipulador,
                                   ContratoChamada informacaoChamada,
                                   string urlServico,
                                   Type tipoRetorno,
                                   Dictionary<string, string> parametrosCabeacalhoAdicionais) :
                                   base(nomeManipulador, informacaoChamada, urlServico, tipoRetorno, parametrosCabeacalhoAdicionais)
        {
        }

        public void ExecutarChamaraAsync(Action<ArgsResultadoChamadaServico> callback, object userState = null)
        {
            this.Callback = callback;
            this.UserState = userState;
            Task.Factory.StartNew(this.ExecutarChamada);
        }


        private void ExecutarChamada()
        {
            object resultado = null;
            Exception erro = null;
            try
            {
                resultado = this.RetornarValorChamada();
            }
            catch (Exception ex)
            {
                erro = ex;
            }
            finally
            {
                if (this.Callback != null)
                {
                    ArgsResultadoChamadaServico args = new ArgsResultadoChamadaServico(erro, resultado, this.UserState);
                    this.Callback.Invoke(args);
                    this.Callback = null;
                }
            }
        }


    }


}
