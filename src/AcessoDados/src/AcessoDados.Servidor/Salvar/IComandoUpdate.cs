using Snebur.Dominio;
using System.Collections.Generic;

namespace Snebur.AcessoDados.Servidor.Salvar
{
    public interface IComandoUpdate
    {
        Dictionary<string, PropriedadeAlterada> PropriedadesAlterada { get;   }
    }
}