using Snebur.Dominio.Atributos;
using System.Reflection;

namespace Snebur.AcessoDados
{
    public class RelacaoAbertaColecao : BaseRelacaoAberta
    {
        #region Campos Privados

        #endregion

        [IgnorarPropriedadeTS, IgnorarPropriedadeTSReflexao]
        public bool IsIncluirDeletados => this.EstruturaConsulta.IsIncluirDeletados;
        public EstruturaConsulta EstruturaConsulta { get; set; } = new EstruturaConsulta();

        public RelacaoAbertaColecao() : base()
        {
        }

        [IgnorarConstrutorTS]
        public RelacaoAbertaColecao(PropertyInfo propriedade) : base(propriedade)
        {
        }
    }
}