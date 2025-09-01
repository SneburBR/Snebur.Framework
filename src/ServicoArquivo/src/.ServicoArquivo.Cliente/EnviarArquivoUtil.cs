
namespace Snebur.ServicoArquivo.Cliente;

public class EnviarArquivoUtil
{
    public static Task<ResultadoServicoArquivo> EnviarArquvoAsync(IArquivo arquivo,
                                                     FileInfo fi)
    {
        return Task.Factory.StartNew(() => EnviarArquvo(arquivo, fi.FullName));
    }

    public static Task<ResultadoServicoArquivo> EnviarArquvoAsync(IArquivo arquivo,
                                                                 string caminhoArquivo)
    {
        return Task.Factory.StartNew(() => EnviarArquvo(arquivo, caminhoArquivo));

    }
    public static Task<ResultadoServicoArquivo> EnviarArquvoAsync(IArquivo arquivo,
                                                                 Stream stream)
    {
        return Task.Factory.StartNew(() => EnviarArquvo(arquivo, stream));
    }

    public static ResultadoServicoArquivo EnviarArquvo(IArquivo arquivo,
                                                      FileInfo fi)
    {
        return EnviarArquvo(arquivo, fi.FullName);
    }

    public static ResultadoServicoArquivo EnviarArquvo(IArquivo arquivo,
                                                      string caminhoArquivo)
    {
        using (var fs = StreamUtil.OpenRead(caminhoArquivo))
        {
            return EnviarArquvo(arquivo, fs);
        }
    }
    public static ResultadoServicoArquivo EnviarArquvo(IArquivo arquivo,
                                                       Stream stream)
    {
        var aplicacao = AplicacaoSnebur.AtualRequired;
        return EnviarArquivo(aplicacao.UrlServicoArquivo, arquivo, stream,
                             aplicacao.CredencialUsuario,
                             aplicacao.IdentificadorSessaoUsuario,
                             aplicacao.IdentificadorProprietario);
    }

    private static ResultadoServicoArquivo EnviarArquivo(string urlServicoArquivo,
                                                         IArquivo arquivo,
                                                         Stream stream,
                                                         CredencialUsuario credencialUsuario,
                                                         Guid identificadorSessaoUsuario,
                                                         string identificadorProprietario)
    {
        using (var enviadorImagemn = new EnviadorArquivo(urlServicoArquivo,
                                                        arquivo,
                                                        stream,
                                                        credencialUsuario,
                                                        identificadorSessaoUsuario,
                                                        identificadorProprietario))
        {
            enviadorImagemn.Enviar();
            Guard.NotNull(enviadorImagemn.Resultado);
            return enviadorImagemn.Resultado;
        }
    }
}
