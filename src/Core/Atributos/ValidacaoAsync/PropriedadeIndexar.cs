using System.Reflection;

namespace Snebur.Dominio.Atributos
{
    public class PropriedadeIndexar
    {
        public PropertyInfo Propriedade { get; }
        public bool IsPermitirDuplicarNulo { get; }
        public bool IsPermitirDuplicarZero { get; }
        public string NomeIndice { get; }

        public PropriedadeIndexar(PropertyInfo propriedade, string nomeIndice) : this(propriedade, false, false)
        {
            this.NomeIndice = nomeIndice;
        }

        public PropriedadeIndexar(PropertyInfo propriedade,
                                  bool isPermitirDuplicarNulo,
                                  bool isPermitirDuplicarZero)
        {
            this.Propriedade = propriedade;

            this.IsPermitirDuplicarNulo = isPermitirDuplicarNulo;
            this.IsPermitirDuplicarZero = isPermitirDuplicarZero;
        }

        public PropriedadeIndexar(PropertyInfo propriedade,
                                  ValidacaoUnicoAttribute validacaoUnicoAttribute)
        {

        }
    }
}