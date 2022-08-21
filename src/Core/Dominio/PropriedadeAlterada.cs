
using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    [IgnorarClasseTS]
    public class PropriedadeAlterada : BaseDominio, IPropriedadeAlterada
    {
        public string NomePropriedade { get; set; }

        public object AntigoValor { get; set; }

        public object NovoValor { get; set; }

        public PropriedadeAlterada(string nomePropriedade, object antigoValor, object novoValor)
        {
            this.NomePropriedade = nomePropriedade;
            this.AntigoValor = antigoValor;
            this.NovoValor = novoValor;
        }
    }
}