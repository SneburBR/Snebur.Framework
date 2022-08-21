using Snebur.Dominio;

namespace Snebur.AcessoDados
{
    internal interface IContextoDadosSemNotificar
    {
        ResultadoSalvar SalvarInternoSemNotificacao(IEntidade entidade);

        ResultadoSalvar SalvarInternoSemNotificacao(IEntidade entidade, bool ignorarValidacao);

        ResultadoSalvar SalvarInternoSemNotificacao(IEntidade[] entidades, bool ignorarValidacao);

    }
}
