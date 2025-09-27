using Snebur.Dominio.Atributos;

namespace Snebur.Dominio;

public enum EnumStatusServicoArquivo
{
    [EnumValorNaoDefido]
    Undefined = -1,
    Aguardando = 0,
    EnvioIniciado = 1,
    EnviadoArquivos = 2,
    EnvioPendente = 3,
    EnvioConcluido = 4,

}
