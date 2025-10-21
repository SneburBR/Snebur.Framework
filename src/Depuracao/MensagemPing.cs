#if NET48
using System;
#endif

namespace Snebur.Depuracao
{
    public class MensagemPing : Mensagem
    {
        public bool Ping { get; set; }

        public DateTime DataHora { get; set; }
    }
}