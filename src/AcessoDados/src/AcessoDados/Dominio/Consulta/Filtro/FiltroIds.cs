﻿namespace Snebur.AcessoDados;

public class FiltroIds : BaseFiltro
{

    #region Campos Privados

    #endregion

    public List<long> Ids { get; set; } = new List<long>();

    public FiltroIds(List<long> ids)
    {
        this.Ids = ids;
    }
}