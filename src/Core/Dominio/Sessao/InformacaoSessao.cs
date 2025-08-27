namespace Snebur.Dominio;

public class InformacaoSessao : BaseDominio, IInformacaoSessao
{
    //public string IdentificadorProprietario { get; set; }

    public Guid IdentificadorAplicacaoGuid { get; set; }

    public required string IdentificadorAplicacao { get; set; }

    //public DadosIPInformacao IPInformacao { get; set; }

    //public string Local { get; set; }

    public required string? Cultura { get; set; }

    public required string Idioma { get; set; }

    public EnumPlataforma Plataforma { get; set; }

    public EnumTipoAplicacao TipoAplicacao { get; set; }

    public Dimensao Resolucao { get; set; } = new Dimensao();

    public string? UserAgent { get; set; }

    public Navegador Navegador { get; set; } = new Navegador();

    public required SistemaOperacional SistemaOperacional { get; set; }

    public required string VersaoAplicacao { get; set; }

    public required string? NomeComputador { get; set; }

    //public string IP { get; set; }

    //IIPInformacao IInformacaoSessao.IPInformacao
    //{
    //    get
    //    {
    //        return (IIPInformacao)this.IPInformacao;
    //    }
    //    set
    //    {
    //        this.IPInformacao = (DadosIPInformacao)value;
    //    }
    //}
}

//public class InformacaoSessaoUsuario: InformacaoSessao 
//{
//    public Guid IdentificadorSessaoUsuario { get; set; }
//    public InformacaoSessaoUsuario()
//    {
//    }
//}