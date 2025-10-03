namespace Snebur.Dominio;

public interface IDimensao
{
    double Largura { get; set; }

    double Altura { get; set; }

    //[IgnorarPropriedadeTS]
    //int LarguraVisualizacao { get; }

    //[IgnorarPropriedadeTS]
    //int AlturaVisualizacao { get; }
}

[IgnorarEnumTS, IgnorarTSReflexao]
public enum ColorSpaceData
{
    [EnumTSString("RGB")]
    RGB = 1,
    [EnumTSString("CMYK")]
    CMYK = 2,
    [EnumTSString("GrayScale")]
    GrayScale = 3,
    [EnumTSString("Desconhecido")]
    Desconhecido = 4
}