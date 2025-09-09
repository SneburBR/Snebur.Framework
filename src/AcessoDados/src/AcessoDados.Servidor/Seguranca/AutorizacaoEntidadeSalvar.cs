namespace Snebur.AcessoDados.Seguranca;

internal class AutorizacaoEntidadeSalvar : AutorizacaoEntidade
{
    public List<Entidade> Entidades { get; }

    internal AutorizacaoEntidadeSalvar(
        string nomeTipoEntidade,
        EnumOperacao operacao,
        List<Entidade> entidades)
        : base(nomeTipoEntidade, operacao)
    {
        this.Entidades = entidades;
    }
}