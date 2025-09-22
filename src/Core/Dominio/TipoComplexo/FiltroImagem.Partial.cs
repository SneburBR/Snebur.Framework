using Snebur.Dominio.Atributos;

namespace Snebur.Dominio;

public partial class FiltroImagem : BaseTipoComplexo
{

    [NaoMapear, JsonIgnore]
    [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
    public bool IsExistExposicao
        => !this.IsNullOuPadrao(this.Exposicao, EXPOSICAO_PADRAO);

    [NaoMapear, JsonIgnore]
    [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
    public bool IsExisteBrilho
        => !this.IsNullOuPadrao(this.Brilho, BRILHO_PADRAO);

    [NaoMapear, JsonIgnore]
    [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
    public bool IsExisteContraste
        => !this.IsNullOuPadrao(this.Contraste, CONTRASTE_PADRAO);

    [NaoMapear, JsonIgnore]
    [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
    public bool IsExisteSaturacao
        => !this.IsNullOuPadrao(this.Saturacao, SATURACAO_PADRAO);

    [NaoMapear, JsonIgnore]
    [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
    public bool IsExistePetroBranco
        => !this.IsNullOuPadrao(this.PretoBranco, PRECO_BRANCO_PADRAO);

    [NaoMapear, JsonIgnore]
    [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
    public bool IsExisteSepia
        => !this.IsNullOuPadrao(this.Sepia, SEPIA_PADRFAO);

    [NaoMapear, JsonIgnore]
    [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
    public bool IsExisteMatrix
        => !this.IsNullOuPadrao(this.Matriz, MATRIZ_PADRAO);

    [NaoMapear, JsonIgnore]
    [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
    public bool IsExisteInverter
        => !this.IsNullOuPadrao(this.Inverter, INVERTER_PADRAO);

    [NaoMapear, JsonIgnore]
    [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
    public bool IsExisteDesfoque
        => !this.IsNullOuPadrao(this.Desfoque, DESFOQUE_PADRAO);

    [NaoMapear, JsonIgnore]
    [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
    public bool IsExisteCiano
        => !this.IsNullOuPadrao(this.Ciano, CIANO_PADRAO);

    [NaoMapear, JsonIgnore]
    [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
    public bool IsExisteMagenta
        => !this.IsNullOuPadrao(this.Magenta, MAGENTA_PADRAO);

    [NaoMapear, JsonIgnore]
    [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
    public bool IsExisteAmarelo
        => !this.IsNullOuPadrao(this.Amarelo, AMARELO_PADRAO);

    [NaoMapear, JsonIgnore]
    [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
    public bool IsExisteExposicao
        => !this.IsNullOuPadrao(this.Exposicao, EXPOSICAO_PADRAO);

    [NaoMapear, JsonIgnore]
    [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
    public bool IsExisteExposicaoExtra
        => this.IsExisteExposicao && Math.Abs(this.Exposicao.GetValueOrDefault()) > 50;

    [NaoMapear, JsonIgnore]
    [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
    public bool IsExisteAjuste
        => this.IsExisteAjusteCor || this.IsExisteAjusteComum;

    [NaoMapear, JsonIgnore]
    [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
    public bool IsExisteAjusteCor
        => this.IsExisteCiano ||
           this.IsExisteMagenta ||
           this.IsExisteAmarelo ||
           this.IsExistExposicao;

    [NaoMapear, JsonIgnore]
    [IgnorarPropriedade, IgnorarPropriedadeTSReflexao]
    public bool IsExisteAjusteComum
        => this.IsExisteBrilho ||
           this.IsExisteContraste ||
           this.IsExisteSaturacao ||
           this.IsExistePetroBranco ||
           this.IsExisteSepia ||
           this.IsExisteMatrix ||
           this.IsExisteInverter ||
           this.IsExisteDesfoque;

    private bool IsNullOuPadrao<T>(T? valor, T padrao) where T : struct
    {
        return valor == null || valor.Value.Equals(padrao);
    }
}
