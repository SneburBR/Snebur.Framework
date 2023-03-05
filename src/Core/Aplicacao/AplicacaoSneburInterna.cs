using System;

namespace Snebur
{
#if NetCore == false

    internal class AplicacaoSneburInterna : AplicacaoSnebur
    {
        public override string RetornarIpDaRequisicao()
        {
            return IpUtil.RetornarIPInformacao(String.Empty).IP;
        }
    }

#endif
}