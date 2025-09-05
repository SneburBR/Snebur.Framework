using System.Text;

namespace Snebur.AcessoDados.Mapeamento
{
    internal partial class BaseSqlBuilder
    {
        private string RetornarSqlFiltroMapeamento(BaseFiltroMapeamento filtro)
        {
            var sqlFiltroBase = "";
            if (filtro.FiltroMapeamentoBase != null &&
                (!(filtro.FiltroMapeamentoBase is FiltroMapeamentoVazio)))
            {
                //Aqui bicho pega
                sqlFiltroBase = this.RetornarSqlFiltroMapeamento(filtro.FiltroMapeamentoBase);
                sqlFiltroBase += " AND ";
            }
            switch (filtro)
            {
                case FiltroMapeamentoEntre filtroMapeamentoEntre:

                    return sqlFiltroBase + this.RetornarSqlFiltroMapeamentoEntre(filtroMapeamentoEntre);

                case FiltroMapeamentoIds filtroMapeamentoIds:

                    return sqlFiltroBase + this.RetornarSqlFiltroMapeamentoIds(filtroMapeamentoIds);

                case FiltroMapeamentoReverso filtroMapeamentoReverso:

                    return sqlFiltroBase + this.RetornarSqlFiltroMapeamentoReverso(filtroMapeamentoReverso);

                default:

                    throw new ErroNaoSuportado($"O filtro não suportado {filtro.GetType().Name}");
            }
        }

        private string RetornarSqlFiltroMapeamentoEntre(FiltroMapeamentoEntre filtro)
        {
            var caminhoCampoFiltro = this.RetornarCaminhoCampoFiltro(filtro);

            var sb = new StringBuilder();
            if (filtro.NomeTipoEntidade != null && ConfiguracaoAcessoDados.TipoBancoDadosEnum == EnumTipoBancoDados.PostgreSQLImob)
            {
                sb.Append($" CAST( tableoid::regclass As text) = '{filtro.NomeTipoEntidade}' AND  ");
            }
            sb.Append($" ( {caminhoCampoFiltro} BETWEEN {filtro.MenorId} AND  {filtro.MaiorId} ) ");
            return sb.ToString();
        }

        private string RetornarSqlFiltroMapeamentoIds(FiltroMapeamentoIds filtro)
        {
            var caminhoCampoFiltro = this.RetornarCaminhoCampoFiltro(filtro);
            var ids = String.Join(",", filtro.Ids);
            if (filtro.Ids.Count > 0)
            {
                var sql = String.Format(" ( {0} BETWEEN {1} AND {2} ) AND ", caminhoCampoFiltro, filtro.Ids.Min(), filtro.Ids.Max());
                return sql + String.Format(" {0} IN ( {1} ) ", caminhoCampoFiltro, ids);
            }
            else
            {
                return string.Empty;
            }
        }

        private string RetornarSqlFiltroMapeamentoReverso(FiltroMapeamentoReverso filtro)
        {
            var caminhoCampoFiltro = this.RetornarCaminhoCampoFiltro(filtro);
            var ids = String.Join(",", filtro.Ids);
            if (filtro.Ids.Count > 0)
            {
                var sql = String.Format(" ( {0} BETWEEN {1} AND {2} ) AND ", caminhoCampoFiltro, filtro.Ids.Min(), filtro.Ids.Max());
                return sql + String.Format(" {0} IN ( {1} ) ", caminhoCampoFiltro, ids);
            }
            else
            {
                return string.Empty;
            }
        }

        private string RetornarCaminhoCampoFiltro(BaseFiltroMapeamento filtro)
        {
            if (filtro.EstruturaCampoFiltro == null)
            {
                return this.EstruturaEntidadeApelido.EstruturaCampoApelidoChavePrimaria.CaminhoBanco;
            }
            else
            {
                var estruturaCampoApelido = this.TodasEstruturaCampoApelidoMapeado[filtro.EstruturaCampoFiltro.Propriedade.Name];
                return estruturaCampoApelido.CaminhoBanco;
            }
        }
    }
}