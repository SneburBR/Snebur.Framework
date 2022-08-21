namespace Snebur.Dominio.Atributos
{
    //[IgnorarEnumTS]
    //[IgnorarTSReflexaoAttribute]
    [IgnorarGlobalizacao]
    public enum EnumTipoExclusaoRelacao
    {
        NaoExcluir = 1,

        Cascata = 2,

        Zerar = 3
    }
}