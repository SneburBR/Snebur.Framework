using System.IO;
using static Snebur.Depuracao.ConstantesDeburacao;

namespace Snebur.Depuracao;

public static class DepuracaoUtil
{
    public static string? RetornarUrlDepuracao(string diretorioAplicacao, string url)
    {
        var porta = DepuracaoUtil.RetornarPorta(diretorioAplicacao);
        if (porta > 0)
        {
            var prefixo = !url.Contains("?") ? "?" : "&";
            var caminhoDepurar = $"{prefixo}{PARAMETRO_VS_DEPURACAO_PORTA}={porta}";
            var posicao = url.IndexOf("#");
            if (posicao > 0)
            {
                return url.Substring(0, posicao) + caminhoDepurar + url.Substring(posicao);
            }
            return url + caminhoDepurar;
        }
        return null;
    }

    private static int RetornarPorta(string diretorio)
    {
        var caminhoArquivo = Path.Combine(diretorio, PARAMETRO_VS_DEPURACAO_PORTA + ".info");
        try
        {
            if (File.Exists(caminhoArquivo))
            {
                if (Int32.TryParse(File.ReadAllText(caminhoArquivo), out int porta))
                {
                    return porta;
                }
            }
        }
        catch (Exception)
        {

        }
        return 0;
    }
}
