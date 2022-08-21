using System.Collections.Generic;

namespace Snebur.AcessoDados
{
    public class FiltroIds : BaseFiltro
    {

        #region Campos Privados


        #endregion

        public List<long> Ids { get; set; }

        public FiltroIds(List<long> ids)
        {
            this.Ids = ids;
        }
    }
}