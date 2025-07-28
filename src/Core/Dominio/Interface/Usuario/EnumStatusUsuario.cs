using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public enum EnumStatusUsuario
    {
        [EnumValorNaoDefido]
        Desconhecido =0,

        Novo = 1,

        Ativo = 3,

        Inativo = 4,

        Bloqueado = 5
    }
}
