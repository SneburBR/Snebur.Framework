using System.Text;

namespace Snebur.Comunicacao;

public class InfoRequisicao : BaseDominio
{
    public string? UserAgent { get; set; }
    public string? IpRequisicao { get; set; }
    public string? CredencialUsuario { get; set; }
}

public class ComunicaoRequisicaoInfo
{
    public required string? UserAgent { get; init; }
    public required string Json { get; init; }
    public required string Url { get; init; }
    public required string Identificador { get; init; }
    public required DateTime DataHoraInicio { get; init; }
    public required string NomeManipulador { get; init; }
    public required string IdentificadorProprietario { get; init; }
    public required string? Operacao { get; init; }
    public required string? IpRequisicao { get; init; }
    public required string? CredencialUsuario { get; init; }

    public string BuildInfo(bool newLineSeparator = true)
    {
       
        var sb = new StringBuilder();
        void separator()
        {
            if (newLineSeparator)
                sb.AppendLine();
            else
                sb.Append(" - ");
        }

        sb.Append($"Identificador: {this.Identificador}");

        separator();

        sb.AppendLine($"Inicio: {this.DataHoraInicio:O}");

        separator();
        sb.AppendLine($"Url: {this.Url}");
        separator();
        sb.AppendLine($"Manipulador: {this.NomeManipulador}");
        separator();
        sb.AppendLine($"Operacao: {this.Operacao}");
        separator();
        return sb.ToString();
    }
}
