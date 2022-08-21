using Snebur.Utilidade;
using System;

namespace Snebur.Servicos
{
    public class ServicoLogSegurancaLocal : BaseServicoLocal, IServicoLogSeguranca
    {
        public Guid NotificarLogSeguranca(string mensagem,
                              string stackTrace,
                              EnumTipoLogSeguranca tipoLogSeguranca,
                              BaseInformacaoAdicionalServicoCompartilhado informacaoAdicional)
        {
            var descricaoTipo = EnumUtil.RetornarDescricao(tipoLogSeguranca);
            mensagem = String.Format("Tipo : {0} \n{1}", descricaoTipo, mensagem);

            this.SalvarLog(mensagem);
            this.IsDebugAttachDispararErro = true;
            return Guid.NewGuid();
        }
    }
}
