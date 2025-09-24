namespace Snebur.Dominio;

public class ErroValidacaoInfo : BaseDominio
{
    public required string NomeTipoEntidade { get; set; }

    public required string NomePropriedade { get; set; }

    public required string NomeTipoValidacao { get; set; }

    public required string Mensagem { get; set; }
}
