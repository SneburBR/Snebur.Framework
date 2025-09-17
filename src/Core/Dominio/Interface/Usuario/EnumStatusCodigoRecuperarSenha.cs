namespace Snebur.Dominio;

public enum EnumStatusCodigoRecuperarSenha
{
    Desconhecido = -1,
    Nenhum = 0,
    Novo = 1,
    TentativaInvalida = 2,
    Sucesso = 3,
    Expirado = 4,
    MaximoTentativaAtingido = 5,
    SenhaRecuperada = 6,
    TempoMaximoTentativaExpirado = 7,
    UsuarioNaoEncontrado = 8
}
