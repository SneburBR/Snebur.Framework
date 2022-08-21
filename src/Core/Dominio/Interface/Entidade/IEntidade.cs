﻿using Snebur.Dominio.Atributos;
using System.Collections.Generic;
using System.ComponentModel;

namespace Snebur.Dominio
{

    public interface IEntidade /*: IBaseDominio*/
    {
        long Id { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        bool __IsExisteAlteracao { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        string __NomeTipoEntidade { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        string __IdentificadorEntidade { get; }

        Dictionary<string, PropriedadeAlterada> __PropriedadesAlteradas { get; }
        //IEntidade CloneSomenteId();

        //TEntidade CloneSomenteId<TEntidade>() where TEntidade : IEntidade;

        //TEntidade CloneSomenteId<TEntidade>() where TEntidade : Entidade;
    }
}
