using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados
{
    internal class SqlValidacaoUnico : BaseSqlIndice
    {
        //internal SqlValidacaoUnico(EstruturaEntidade estruturaEntidade, PropriedadeIndexar propriedade) :
        //    this(estruturaEntidade,
        //        new List<PropriedadeIndexar>() { propriedade },
        //        new List<FiltroPropriedadeIndexar>())
        //{
        //}

        internal SqlValidacaoUnico(EstruturaEntidade estruturaEntidade,
                                   List<PropriedadeIndexar> propriedades,
                                   List<FiltroPropriedadeIndexar> filtros) :
            base(estruturaEntidade, propriedades, filtros, true)
        {
        }

        protected override string RetornarSql_SqlServer()
        {
            return base.RetornarSql_SqlServer();
        }
    }
}