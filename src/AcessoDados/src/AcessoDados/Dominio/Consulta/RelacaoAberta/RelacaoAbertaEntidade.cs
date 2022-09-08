using Snebur.Dominio.Atributos;
using System.Reflection;

namespace Snebur.AcessoDados
{
    public class RelacaoAbertaEntidade : BaseRelacaoAberta
    {
        #region Campos Privados

        #endregion

        public RelacaoAbertaEntidade() : base()
        {
        }
        [IgnorarConstrutorTS]
        public RelacaoAbertaEntidade(PropertyInfo propriedade) : base(propriedade)
        {
        }
    }
}