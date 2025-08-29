namespace Snebur.Dominio.Atributos;

public class RelacaoPaiExternaAttribute : NaoMapearAttribute, IIgnorarAlerta
{
    public bool IgnorarAlerta => true;
    public string NomePropriedadeChaveEstrangeira { get; set; }
    public string NomeContextoDadosExterno { get; set; }

    public RelacaoPaiExternaAttribute(string nomeContextoDadosExtexto,
                             string nomePropriedadeChaveEstrangeira)
    {
        this.NomePropriedadeChaveEstrangeira = nomePropriedadeChaveEstrangeira;
        this.NomeContextoDadosExterno = nomeContextoDadosExtexto;
    }
}