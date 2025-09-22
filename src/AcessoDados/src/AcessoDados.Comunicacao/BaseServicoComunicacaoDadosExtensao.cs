//using Snebur.AcessoDados.Comunicacao;

//namespace Snebur.AcessoDados;

//public static class BaseServicoComunicacaoDadosExtensao
//{
//    public static TContextoDados GetRequiredContextoDados<TContextoDados>(
//        this BaseServicoComunicacaoDados<TContextoDados> servicoComunicacaoDados)
//        where TContextoDados : BaseContextoDados
//    {
//        return servicoComunicacaoDados.ContextoDados
//            ?? throw new ErroOperacaoInvalida("ContextoDados não inicializado");
//    }
//}