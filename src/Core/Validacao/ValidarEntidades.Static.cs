namespace Snebur.Dominio;

public partial class ValidarEntidades
{
    public static List<ErroValidacaoInfo> Validar(object contextoDados, List<Entidade> entidades)
    {
        using (var validar = new ValidarEntidades(entidades))
        {
            return validar.RetornarErroValidacao(contextoDados);
        }
    }

    public static List<ErroValidacaoInfo> Validar(object contextoDados, Entidade entidade)
    {
        var entidades = new List<Entidade> { entidade };
        return Validar(contextoDados, entidades);
    }
}