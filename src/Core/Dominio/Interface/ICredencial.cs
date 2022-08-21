using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public interface ICredencial
    {
        [ValidacaoRequerido]
        [ValidacaoUnico]
        [ValidacaoTextoTamanho(100)]
        string IdentificadorUsuario { get; set; }

        //[IgnorarPropriedadeTS]
        //[IgnorarPropriedadeTSReflexao]
        [ValidacaoSenha]
        [ValidacaoTextoTamanho(36)]
        string Senha { get; set; }

        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        bool IsAnonimo { get; }
    }


}