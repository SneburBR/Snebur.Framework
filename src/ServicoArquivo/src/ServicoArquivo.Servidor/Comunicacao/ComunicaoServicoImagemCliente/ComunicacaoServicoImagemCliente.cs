using Snebur.Dominio;
using Snebur.Seguranca;
using System;
using System.Reflection;

namespace Snebur.ServicoArquivo
{

    public class ComunicacaoServicoImagemCliente : ComunicacaoServicoArquivoCliente, IComunicacaoServicoImagem
    {

        public ComunicacaoServicoImagemCliente(string urlServico, 
                                               CredencialUsuario credencialRequisicao, 
                                               Guid identificadorSessaoUsuario,
                                               FuncaoNormalizadorOrigem funcaoNormalizadorOrigem) :
                                               base(urlServico, 
                                                    credencialRequisicao, 
                                                    identificadorSessaoUsuario, 
                                                    funcaoNormalizadorOrigem)
        {
        }

        public bool ExisteImagem(long idImagem, EnumTamanhoImagem tamanhoImagem)
        {
            object[] parametros = { idImagem, tamanhoImagem };
            return this.ChamarServico<bool>(MethodBase.GetCurrentMethod(), parametros);
        }

        public bool NotificarFimEnvioImagem(long idImagem, long totalBytes, EnumTamanhoImagem tamanhoImagem, string checksum)
        {
            object[] parametros = { idImagem, totalBytes, tamanhoImagem, checksum };
            return this.ChamarServico<bool>(MethodBase.GetCurrentMethod(), parametros);
        }
    }
}