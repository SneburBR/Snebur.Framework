namespace Snebur.AcessoDados.Seguranca
{
    internal class AutorizacaoEntidadeLeitura : AutorizacaoEntidade
    {
        internal AutorizacaoEntidadeLeitura(string nomeTipoEntidade, EnumOperacao operacao, EstruturaConsulta estruturaConsulta) :
                                            base(nomeTipoEntidade, operacao)
        {
        }
    }
}