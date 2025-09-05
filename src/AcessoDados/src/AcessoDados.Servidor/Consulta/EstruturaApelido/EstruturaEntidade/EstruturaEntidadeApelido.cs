using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento
{
    internal class EstruturaEntidadeApelido : BaseEstruturaEntidadeApelido
    {

        internal EstruturaEntidadeApelido(BaseMapeamentoConsulta mapeamentoConsulta,
                                          string apelidoEntidadeMapeada,
                                          EstruturaEntidade estruturaEntidade
                                          ) :
                                          base(mapeamentoConsulta,
                                               apelidoEntidadeMapeada,
                                               estruturaEntidade)
        {
        }

        internal List<EstruturaCampoApelido> RetornarEstruturasCampoMapeado()
        {
            var estruturas = new List<EstruturaCampoApelido>();
            estruturas.AddRange(this.EstruturasCampoApelido);

            if (this.EstruturasEntidadeMapeadaBase.Count > 0)
            {
                var estruturasCampoBase = this.EstruturasEntidadeMapeadaBase.SelectMany(x => x.EstruturasCampoApelido).ToList();
                estruturas.AddRange(estruturasCampoBase);
            }
            return estruturas;
        }

        internal string RetornarSqlInnerJoinTabelasEntidadeBase()
        {
            var sqlsJoinTabela = this.EstruturasEntidadeMapeadaBase.Select(x => x.RetornarSqlUniaoEntidadeBaseBanco()).ToList();
            var sepador = String.Format(" {0} \t  INNER JOIN ", System.Environment.NewLine);
            var sql = String.Join(sepador, sqlsJoinTabela);
            return sql;
        }
    }
}