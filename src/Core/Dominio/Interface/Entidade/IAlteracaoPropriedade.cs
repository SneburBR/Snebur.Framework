using Snebur.Dominio.Atributos;
using System;

namespace Snebur.Dominio
{
    public interface IAlteracaoPropriedade : IAtividadeUsuario
    {

        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        object ValorPropriedadeRelacao { get; set; }

        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        object ValorPropriedadeAntigo { get; set; }

        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        object ValorPropriedadeAlterada { get; set; }

        //[IgnorarPropriedadeTS]
        //[IgnorarPropriedadeTSReflexao]
        DateTime? DataHoraFimAlteracao { get; set; }
    }


}