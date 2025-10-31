namespace Snebur.Dominio;

public enum EnumResultadoAutenticacao
{
    [UndefinedEnumValue] Undefined = -1,
    Sucesso = 1,
    UsuarioNaoExiste = 2,
    SenhaInvalida = 3,
    MaximoTentativa = 4,
    UsuarioBloqueado = 5,
    ContaAtualNaoDefinida = 9
}