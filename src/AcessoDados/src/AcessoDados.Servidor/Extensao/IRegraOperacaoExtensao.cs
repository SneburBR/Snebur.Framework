namespace Snebur.AcessoDados.Seguranca;

public static class RegraOperacaoExtensao
{
    public static EnumPermissao RetornarPermisao(this IRegraOperacao operacao)
    {
        if (operacao.Autorizado)
        {
            if (operacao.AvalistaRequerido)
            {
                return EnumPermissao.AvalistaRequerido;
            }
            return EnumPermissao.Autorizado;
        }
        return EnumPermissao.Negado;
    }
}