namespace Snebur.Dominio;

//[IgnorarClasseTS]
public partial class Borda : BaseMedidaTipoComplexo, IBorda
{
    private string _corRgba = "rgba(0,0,0,0)";
    private bool _isInterna = true;
    private double _afastamento;
    private double _espessura;
    private int _arredondamento;

    //[NaoMapear]
    //[IgnorarPropriedadeTS]
    //[IgnorarPropriedadeTSReflexao]    
    //public Cor Cor { get; }

    [ValidacaoTextoTamanho(32)]
    public string CorRgba { get => this._corRgba; set => this.SetProperty(this._corRgba, this._corRgba = value); }

    public bool IsInterna { get => this._isInterna; set => this.SetProperty(this._isInterna, this._isInterna = value); }

    public double Afastamento { get => this._afastamento; set => this.SetProperty(this._afastamento, this._afastamento = value); }

    public double Espessura { get => this._espessura; set => this.SetProperty(this._espessura, this._espessura = value); }

    public int Arredondamento { get => this._arredondamento; set => this.SetProperty(this._arredondamento, this._arredondamento = value); }

    public Borda()
    {
    }

    [IgnorarConstrutorTS]
    public Borda(Cor cor, bool isInterna, double afastamento, double espessura, int arredondamento)
    {
        this.Cor = cor;
        this.IsInterna = isInterna;
        this.Afastamento = afastamento;
        this.Espessura = espessura;
        this.Arredondamento = arredondamento;
    }

    [IgnorarConstrutorTS]
    public Borda(string corRgba, bool isInterna, double afastamento, double espessura, int arredondamento)
    {
        this.CorRgba = corRgba;
        this.IsInterna = isInterna;
        this.Afastamento = afastamento;
        this.Espessura = espessura;
        this.Arredondamento = arredondamento;
    }

    protected internal override BaseTipoComplexo BaseClone()
    {
        return new Borda(this.CorRgba, this.IsInterna, this.Afastamento, this.Espessura, this.Arredondamento);
    }
}