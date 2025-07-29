namespace Snebur.ServicoArquivo.Comunicao;

[IgnorarInterfaceTS]
public interface IComunicacaoServicoArquivo
{
    bool ExisteIdArquivo(long idArquivo);

    bool ExisteArquivo(long idArquivo);

    bool NotificarInicioEnvioArquivo(long idArquivo);

    bool NotificarFimEnvioArquivo(long idArquivo, long totalBytes, string checksum);

    bool NotificarArquivoDeletado(long idArquivo);

    bool NotificarProgresso(long idArquivo, double progresso);
}
