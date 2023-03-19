
using Snebur.Dominio.Atributos;

namespace Snebur.Dominio
{
    [IgnorarClasseTS]
    public class PropriedadeAlterada : BaseDominio, IPropriedadeAlterada
    {
        public string NomePropriedade { get; set; }
        public string NomePropriedadeTipoComplexo { get; set; }

        public object AntigoValor { get; set; }

        public object NovoValor { get; set; }

        public PropriedadeAlterada(
            string nomePropriedade, 
            object antigoValor, 
            object novoValor,
            string nomePropriedadeTipoComplexo)
        {
            this.NomePropriedade = nomePropriedade;
            this.NomePropriedadeTipoComplexo = nomePropriedadeTipoComplexo;
            this.AntigoValor = antigoValor;
            this.NovoValor = novoValor;
        }
    }
}