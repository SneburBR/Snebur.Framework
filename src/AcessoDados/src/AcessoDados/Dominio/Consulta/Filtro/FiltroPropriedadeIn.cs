namespace Snebur.AcessoDados;

public class FiltroPropriedadeIn : BaseFiltro
{

    #region Campos Privados

    private string? _caminhoPropriedade;

    #endregion

    public string? CaminhoPropriedade { get => this.GetPropertyValue(this._caminhoPropriedade); set => this.SetProperty(this._caminhoPropriedade, this._caminhoPropriedade = value); }

    public List<string> Lista { get; set; } = new List<string>();

    public FiltroPropriedadeIn(string caminhoPropriedade, List<string> lista)
    {
        this.CaminhoPropriedade = caminhoPropriedade;
        this.Lista = lista;
    }
}