using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.AcessoDados.Mapeamento
{
    internal abstract partial class BaseMapeamentoEntidade : IDisposable
    {

        protected string MontarSql(BaseFiltroMapeamento filtroMapeamento,
                                   string sqlCampos,
                                   bool isIncluirOrdenacaoPaginacao)
        {
            var isRelacaoFilhos = this.MapeamentoConsulta is MapeamentoConsultaRelacaoAbertaFilhos;
            var builder = this.RetornarSqlBuilder();
            return builder.MontarSql(filtroMapeamento, 
                                     sqlCampos,
                                     isIncluirOrdenacaoPaginacao,
                                     isRelacaoFilhos);
        }

        private BaseSqlBuilder RetornarSqlBuilder()
        {
            if (this.Contexto.SqlSuporte.IsOffsetFetch)
            {
                return new SqlBuilder(this);
            }
            return new SqlBuilder2008(this);
        }
    }
}
