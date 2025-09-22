namespace Snebur.ServicoArquivo;

public class InformacaoRepositorioArquivo : IInformacaoRepositorioArquivo
{
    public long IdArquivo { get; }

    public string NomeTipoArquivo { get; }

    public InformacaoRepositorioArquivo(long idArquivo, string nomeTipoArquivo)
    {
        this.IdArquivo = idArquivo;
        this.NomeTipoArquivo = nomeTipoArquivo;
    }
}

public class InformacaoRepositorioImagem : InformacaoRepositorioArquivo, IInformacaoRepositorioImagem
{
    public EnumTamanhoImagem TamanhoImagem { get; }

    public InformacaoRepositorioImagem(long idArquivo, string nomeTipoArquivo, EnumTamanhoImagem tamanhoImagem) : base(idArquivo, nomeTipoArquivo)
    {
        this.TamanhoImagem = tamanhoImagem;
    }
}

public interface IInformacaoRepositorioArquivo
{
    long IdArquivo { get; }

    string NomeTipoArquivo { get; }
}

public interface IInformacaoRepositorioImagem : IInformacaoRepositorioArquivo
{
    EnumTamanhoImagem TamanhoImagem { get; }
}
