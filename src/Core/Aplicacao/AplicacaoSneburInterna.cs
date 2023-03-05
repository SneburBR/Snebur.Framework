using System;

namespace Snebur
{
#if NetCore == false

    internal class AplicacaoSneburInterna : AplicacaoSnebur
    {
        public string RetornarIpDaRequisicao()
        {
            return IpUtil.RetornarIPInformacao(String.Empty).IP;
        }
    }

#endif
}