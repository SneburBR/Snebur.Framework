using System;

namespace Snebur.Depuracao
{
    public class MensagemIrParaCodigo : Mensagem
    {
        public string NomeControle { get; set; } = "";
        public string[] SearchElementPatterns { get; set; } = Array.Empty<string>();
        public string TagElemento { get; set; } = "";
        public string Namespace { get; set; } = "";
    }
}
