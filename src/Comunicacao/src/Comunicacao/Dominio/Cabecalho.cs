using Snebur.Seguranca;

namespace Snebur.Comunicacao.Dominio;

public class Cabecalho : BaseComunicao /*, IIdentificadorProprietario, IIdentificadorAplicacao */
{

    public string? IdentificadorProprietario { get; set; }
    public CredencialServico? CredencialServico { get; set; }

    public CredencialUsuario? CredencialUsuario { get; set; }

    public CredencialUsuario? CredencialAvalista { get; set; }

    public string? UrlOrigem { get; set; }
}