namespace Snebur.Imagens;

public class ConfiguracaoTeste
{
    private static bool _salvarImagemTemporaria = false;

    public static bool SalvarImagemTemporaria
    {
        get
        {
#if DEBUG
            return ConfiguracaoTeste._salvarImagemTemporaria;
#else
            return false;
#endif

        }
    }
}
