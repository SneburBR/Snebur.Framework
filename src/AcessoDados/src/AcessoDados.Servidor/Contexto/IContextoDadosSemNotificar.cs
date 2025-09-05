namespace Snebur.AcessoDados
{
    internal interface IContextoDadosSemNotificar
    {
        void NotificarSessaoUsuarioAtiva(IUsuario usuario, ISessaoUsuario sessaoUsuario);
        ResultadoSalvar SalvarInternoSemNotificacao(IEntidade entidade);

        ResultadoSalvar SalvarInternoSemNotificacao(IEntidade entidade, bool ignorarValidacao);

        ResultadoSalvar SalvarInternoSemNotificacao(IEntidade[] entidades, bool ignorarValidacao);

    }
}
