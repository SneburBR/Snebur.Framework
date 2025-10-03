namespace Snebur;

[IgnorarInterfaceTS]
public interface IInformacaoImagem : IDimensao
{
    bool IsImagem { get; set; }
    string NomeArquivo { get; set; }

    // number in TS, bytes make more sense as long in C#
    long TotalBytes { get; set; }

    // optional in TS, nullable in C#
    string? Type { get; set; }

    // angles and orientations are integers in most pipelines
    int? Rotacao { get; set; }
    string? Tamanho { get; set; }
    int? Orientacao { get; set; }

    bool? IsHeic { get; set; }
    bool? IsIcone { get; set; }

    string? Url { get; set; }
    string? PerfilCor { get; set; }
    bool? IsAlertaPerfilCor { get; set; }
    ColorSpaceData? ColorSpace { get; set; }
}
