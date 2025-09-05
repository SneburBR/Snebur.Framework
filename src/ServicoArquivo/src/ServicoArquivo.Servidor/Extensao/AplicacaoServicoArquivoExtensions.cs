namespace Snebur.ServicoArquivo.Servidor;

public static class AplicacaoServicoArquivoExtensions
{

    public static BaseAplicacaoServicoArquivo AplicacaoServicoArquivo(this AplicacaoSnebur aplicacao)
    {
        var aplicacaoArquivo = aplicacao as BaseAplicacaoServicoArquivo;
        if (aplicacaoArquivo is null)
        {
            throw new InvalidOperationException(
                $"Aplicação '{AplicacaoSnebur.AtualRequired?.GetType().Name ?? "null"}' deve herdar de '{nameof(BaseAplicacaoServicoArquivo)}'");
        }
        return aplicacaoArquivo;
    }

}