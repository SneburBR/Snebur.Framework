namespace Snebur;

public class GerenciadorErros
{
    private static GerenciadorErros? _instancia;

    public static GerenciadorErros Instancia
        => LazyUtil.RetornarValorLazyComBloqueio(ref _instancia, () => new GerenciadorErros());

    private GerenciadorErros()
    {

    }

    internal void AnalisarErroAsync(Erro erro)
    {
        //TODO: Implementar AnalisarErroAsync
    }
}