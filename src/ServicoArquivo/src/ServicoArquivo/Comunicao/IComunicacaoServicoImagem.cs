namespace Snebur.ServicoArquivo.Comunicao;

[IgnorarInterfaceTS]
public interface IComunicacaoServicoImagem : IComunicacaoServicoArquivo
{
    bool ExisteImagem(long idImagem, EnumTamanhoImagem tamanhoImagem);

    bool NotificarFimEnvioImagem(long idImagem, long totalBytes, EnumTamanhoImagem tamanhoImagem, string checksum);
}
