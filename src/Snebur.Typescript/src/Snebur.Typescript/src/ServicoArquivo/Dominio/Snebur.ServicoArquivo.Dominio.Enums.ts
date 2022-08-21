/*eslint-disable*/
//Data : segunda-feira, 15 de agosto de 2022
//Hora : 06:15:27
//@Namespace: Snebur.ServicoArquivo
//@PrioridadeDominio: 2
//@Globalizar: False
//@Dominios dependentes: []

namespace Snebur.ServicoArquivo
{
    export enum EnumTipoErroServicoArquivo
    {
        ChecksumArquivoDiferente = 1,
        ChecksumPacoteDiferente = 2,
        TotalBytesDiferente = 3,
        ArquivoTempEmUso = 4,
        Desconhecido = 5,
        ArquivoNaoEncontrado = 6,
        IdArquivoNaoExiste = 7,
    }
    (EnumTipoErroServicoArquivo as any).Rotulos = {};
    (EnumTipoErroServicoArquivo as any).Rotulos["ChecksumArquivoDiferente"] = "ChecksumArquivoDiferente";
    (EnumTipoErroServicoArquivo as any).Rotulos["ChecksumPacoteDiferente"] = "ChecksumPacoteDiferente";
    (EnumTipoErroServicoArquivo as any).Rotulos["TotalBytesDiferente"] = "TotalBytesDiferente";
    (EnumTipoErroServicoArquivo as any).Rotulos["ArquivoTempEmUso"] = "ArquivoTempEmUso";
    (EnumTipoErroServicoArquivo as any).Rotulos["Desconhecido"] = "Desconhecido";
    (EnumTipoErroServicoArquivo as any).Rotulos["ArquivoNaoEncontrado"] = "ArquivoNaoEncontrado";
    (EnumTipoErroServicoArquivo as any).Rotulos["IdArquivoNaoExiste"] = "IdArquivoNaoExiste";

}