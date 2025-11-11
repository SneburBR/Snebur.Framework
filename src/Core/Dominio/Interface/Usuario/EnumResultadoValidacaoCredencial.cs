namespace Snebur.Dominio;

public enum EnumResultadoValidacaoCredencial
{
    [UndefinedEnumValue] Undefined = -1,
    Sucesso = 1,
    UsuarioNaoExiste = 2,
    SenhaIncorreta = 3,
    MaximoTentativaAtingido = 4,
    EmailInvalido = 5,
    UsuarioOuSenhaInvalido = 5,
}