namespace Snebur.Dominio;

[IgnorarClasseTS]
public partial class FiltroImagem : BaseTipoComplexo, IFiltroImagem
{
    public const double CONTRASTE_PADRAO = 100;
    public const double BRILHO_PADRAO = 100;
    public const double SATURACAO_PADRAO = 100;
    public const double SEPIA_PADRFAO = 0;
    public const double PRECO_BRANCO_PADRAO = 0;
    public const double INVERTER_PADRAO = 0;
    public const double MATRIZ_PADRAO = 0;
    public const double DESFOQUE_PADRAO = 0;

    public const double EXPOSICAO_PADRAO = 0;
    public const double CIANO_PADRAO = 0;
    public const double MAGENTA_PADRAO = 0;
    public const double AMARELO_PADRAO = 0;

    [ValidacaoIntervalo(-100, 100)]
    public double? Exposicao
    {
        get => field;
        set => this.SetProperty(field, field = (value == EXPOSICAO_PADRAO) ? null : value);
    }

    [ValidacaoIntervalo(-100, 100)]
    public double? Magenta
    {
        get => field;
        set => this.SetProperty(field, field = (value == MAGENTA_PADRAO) ? null : value);
    }

    [ValidacaoIntervalo(-100, 100)]
    public double? Ciano
    {
        get => field;
        set => this.SetProperty(field, field = (value == CIANO_PADRAO) ? null : value);
    } = null;

    [ValidacaoIntervalo(-100, 100)]
    public double? Amarelo
    {
        get => field;
        set => this.SetProperty(field, field = (value == AMARELO_PADRAO) ? null : value);
    }

    [ValidacaoIntervalo(0, 200)]
    public double? Contraste
    {
        get => field;
        set => this.SetProperty(field, field = (value == CONTRASTE_PADRAO) ? null : value);
    }

    [ValidacaoIntervalo(0, 200)]
    public double? Brilho
    {
        get => field;
        set => this.SetProperty(field, field = (value == BRILHO_PADRAO) ? null : value);
    }

    [ValidacaoIntervalo(0, 100)]
    public double? Sepia
    {
        get => field;
        set => this.SetProperty(field, field = (value == SEPIA_PADRFAO) ? null : value);
    }

    [ValidacaoIntervalo(0, 200)]
    public double? Saturacao
    {
        get => field;
        set => this.SetProperty(field, field = (value == SATURACAO_PADRAO) ? null : value);
    }

    [ValidacaoIntervalo(0, 100)]
    public double? PretoBranco
    {
        get => field;
        set => this.SetProperty(field, field = (value == PRECO_BRANCO_PADRAO) ? null : value);
    }

    [ValidacaoIntervalo(0, 100)]
    public double? Inverter
    {
        get => field;
        set => this.SetProperty(field, field = (value == INVERTER_PADRAO) ? null : value);
    }

    [ValidacaoIntervalo(0, 360)]
    public double? Matriz
    {
        get => field;
        set => this.SetProperty(field, field = (value == MATRIZ_PADRAO) ? null : value);
    }

    [ValidacaoIntervalo(0, 10)]
    public double? Desfoque { get => field; set => this.SetProperty(field, field = (value == DESFOQUE_PADRAO) ? null : value); } = null;

    public static FiltroImagem Empty
    {
        get
        {
            return new FiltroImagem();
        }
    }

    public FiltroImagem()
    {

    }

    protected internal override BaseTipoComplexo BaseClone()
    {
        return new FiltroImagem
        {
            Exposicao = this.Exposicao,
            Magenta = this.Magenta,
            Ciano = this.Ciano,
            Amarelo = this.Amarelo,
            Contraste = this.Contraste,
            Brilho = this.Brilho,
            Sepia = this.Sepia,
            Saturacao = this.Saturacao,
            PretoBranco = this.PretoBranco,
            Inverter = this.Inverter,
            Matriz = this.Matriz,
            Desfoque = this.Desfoque
        };
    }
}

public interface IFiltroImagem
{
    double? Exposicao { get; set; }
    double? Magenta { get; set; }
    double? Ciano { get; set; }
    double? Amarelo { get; set; }
    double? Contraste { get; set; }
    double? Brilho { get; set; }
    double? Sepia { get; set; }
    double? Saturacao { get; set; }
    double? PretoBranco { get; set; }
    double? Inverter { get; set; }
    double? Matriz { get; set; }
    double? Desfoque { get; set; }
}

public static class FiltroImagemExtensions
{
    public static bool IsEmpty(this IFiltroImagem filtroImagem)
    {
        return filtroImagem.Exposicao is null
            && filtroImagem.Magenta is null
            && filtroImagem.Ciano is null
            && filtroImagem.Amarelo is null
            && filtroImagem.Contraste is null
            && filtroImagem.Brilho is null
            && filtroImagem.Sepia is null
            && filtroImagem.Saturacao is null
            && filtroImagem.PretoBranco is null
            && filtroImagem.Inverter is null
            && filtroImagem.Matriz is null
            && filtroImagem.Desfoque is null;
    }
}