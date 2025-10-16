namespace Snebur.Dominio;

public enum EnumStatusArquivo
{
    [UndefinedEnumValue] Undefined = -1,
    Novo = 1,
    Enviando = 2,
    EnvioConcluido = 3,
    ArquivoDeletado = 4,
    Pendente = 5,
    ChecksumInvalido = 6,
}