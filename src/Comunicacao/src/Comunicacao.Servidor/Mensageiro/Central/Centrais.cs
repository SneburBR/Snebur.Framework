using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.Comunicacao.Mensageiro
{

    public static class Centrais
    {
        #region Estatico 

        private static object _bloqueio = new object();

        internal static Dictionary<Guid, ICentral> DicionarioCentrais = new Dictionary<Guid, ICentral>();

        public static TCentral RetornarCentral<TCentral>(Guid identificadorCentral) where TCentral : ICentral
        {
            if (!DicionarioCentrais.ContainsKey(identificadorCentral))
            {
                lock (_bloqueio)
                {
                    if (!DicionarioCentrais.ContainsKey(identificadorCentral))
                    {
                        var novoInstanciaCentral = RetornarNovaCentral<TCentral>(identificadorCentral);
                        DicionarioCentrais.Add(identificadorCentral, novoInstanciaCentral);
                    }
                }
            }
            return (TCentral)DicionarioCentrais[identificadorCentral];
        }

        private static TCentral RetornarNovaCentral<TCentral>(Guid identificador) where TCentral : ICentral
        {
            try
            {
                return (TCentral)Activator.CreateInstance(typeof(TCentral), new object[] { identificador }, null);
            }
            catch (Exception ex)
            {
                throw new Erro($"Não foi possivel criar central do mensageiro do tipo {typeof(TCentral).Name}", ex);
            }
        }

        #endregion
    }
}
