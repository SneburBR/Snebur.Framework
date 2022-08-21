using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    [IgnorarTSReflexao]
    [IgnorarClasseTSAttribute]
    public class EntidadeViewModel : BaseViewModel
    {
        public Entidade Entidade { get; set; }
    }
}
