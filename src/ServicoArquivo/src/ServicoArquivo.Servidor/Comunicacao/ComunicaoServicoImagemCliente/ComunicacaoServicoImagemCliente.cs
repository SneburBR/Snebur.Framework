namespace Snebur.ServicoArquivo;


public class ComunicacaoServicoImagemCliente : ComunicacaoServicoArquivoCliente, IComunicacaoServicoImagem
{

    public ComunicacaoServicoImagemCliente(
        string urlServico,
        CredencialUsuario credencialRequisicao,
        Guid? identificadorSessaoUsuario,
        string? identificadorProprietario,
        FuncaoNormalizadorOrigem funcaoNormalizadorOrigem) :
        base(urlServico,
            credencialRequisicao,
            identificadorSessaoUsuario,
            identificadorProprietario,
            funcaoNormalizadorOrigem)
    {
    }

    public bool ExisteImagem(long idImagem, EnumTamanhoImagem tamanhoImagem)
    {
        object[] parametros = { idImagem, tamanhoImagem };
        return this.ChamarServico<bool>(MethodBase.GetCurrentMethod(), parametros);
    }

    public bool NotificarFimEnvioImagem(long idImagem, long totalBytes, EnumTamanhoImagem tamanhoImagem, string? checksum)
    {
        object?[] parametros = { idImagem, totalBytes, tamanhoImagem, checksum };
        return this.ChamarServico<bool>(MethodBase.GetCurrentMethod(), parametros);
    }
}