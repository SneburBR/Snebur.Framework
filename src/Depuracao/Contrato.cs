using Snebur.Dominio;

namespace Snebur.Depuracao
{
    public class Contrato : BaseDominio
    {

#if NET6_0_OR_GREATER
        public Mensagem? Mensagem { get; set; }
#endif

#if NET48
        public Mensagem Mensagem { get; set; }
#endif

    }
}