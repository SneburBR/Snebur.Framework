namespace Snebur.Dominio;

public enum EnumTipoValorPadrao
{
    [UndefinedEnumValue]
    Nenhum,
    IndentificadorProprietario,
    SessaoUsuario_Id,
    UsuarioLogado_Id,
    Comum,
    ValorPropriedadeNullOrDefault,
    ValorPropriedadeNullOrWhiteSpace
}