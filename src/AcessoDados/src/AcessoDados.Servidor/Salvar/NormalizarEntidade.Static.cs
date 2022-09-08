using Snebur.Dominio;
using System.Collections.Generic;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    internal partial class NormalizarEntidade
    {
        internal static HashSet<Entidade> RetornarEntidadesNormalizada(BaseContextoDados contexto, HashSet<Entidade> entidades)
        {
            using (var normalizar = new NormalizarEntidade(contexto, entidades))
            {
                return normalizar.RetornarEntidadesNormalizada();
            }
        }
    }
}