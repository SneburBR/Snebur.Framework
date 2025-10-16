namespace Snebur.Dominio;

public enum EnumTipoValorPadrao
{
    [UndefinedEnumValue] Undefined = -1,
    Nenhum,
    IndentificadorProprietario,
    SessaoUsuario_Id,
    UsuarioLogado_Id,
    Comum,
    ValorPropriedadeNullOrDefault,
    ValorPropriedadeNullOrWhiteSpace
}