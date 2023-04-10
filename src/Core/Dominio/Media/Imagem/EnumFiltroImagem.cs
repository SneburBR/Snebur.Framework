

using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    public enum EnumFiltroImagem
    {
        [Rotulo("Exposição")]
        Exposicao,

        [Rotulo("Ciano")]
        Ciano,

        [Rotulo("Magenta")]
        Magenta,

        [Rotulo("Amarelo")]
        Amarelo,

        [Rotulo("Contraste")]
        Contraste,

        [Rotulo("Brilho")]
        Brilho,

        [Rotulo("Sépia")]
        Sepia,

        [Rotulo("Saturação")]
        Saturacao,

        [Rotulo("Preto e branco")]
        PretoBranco,

        [Rotulo("Inverter")]
        Inverter,

        [Rotulo("Matriz")]
        Matriz,

        [Rotulo("Desfoque")]
        Desfoque
    }
}
