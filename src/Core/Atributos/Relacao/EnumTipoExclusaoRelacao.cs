namespace Snebur.Dominio.Atributos;

//[IgnorarEnumTS]
//[IgnorarTSReflexaoAttribute]
[IgnorarGlobalizacao]
public enum EnumTipoExclusaoRelacao
{
    [UndefinedEnumValue]
    Undefined = -1,
    NaoDeletar = 1,
    Cascata = 2,
    Zerar = 3
}