using System.ComponentModel;

namespace Snebur.ServicoArquivo.Dominio;

public enum EnumTipoErroServicoArquivo
{
    [Description("Checksum do arquivo diferentes")]
    ChecksumArquivoDiferente = 1,

    [Description("Checksum do pacote diferentes")]
    ChecksumPacoteDiferente = 2,

    [Description("Total de bytes diferente")]
    TotalBytesDiferente = 3,

    [Description("Arquivo temp em uso")]
    ArquivoTempEmUso = 4,

    [Description("Desconhecido")]
    Desconhecido = 5,

    ArquivoNaoEncontrado = 6,
    IdArquivoNaoExiste = 7
}