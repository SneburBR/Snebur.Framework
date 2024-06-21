using Snebur.AcessoDados.Estrutura;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Snebur.AcessoDados.Mapeamento
{
    internal abstract partial class BaseSqlBuilder
    {
        private BaseMapeamentoEntidade mapeamentoConsulta;
        protected EstruturaEntidadeApelido EstruturaEntidadeApelido => this.mapeamentoConsulta.EstruturaEntidadeApelido;
        private BaseContextoDados Contexto => this.mapeamentoConsulta.Contexto;
        private EstruturaEntidade EstruturaEntidade => this.mapeamentoConsulta.EstruturaEntidade;
        private BaseConexao ConexaoDB => this.mapeamentoConsulta.ConexaoDB;
        private EstruturaConsulta EstruturaConsulta => this.mapeamentoConsulta.EstruturaConsulta;
        private EstruturaCampoApelido EstruturaCampoApelidoChavePrimaria => this.mapeamentoConsulta.EstruturaCampoApelidoChavePrimaria;
        private List<ParametroInfo> ParametrosInfo => this.mapeamentoConsulta.ParametrosInfo;
        private DicionarioEstrutura<EstruturaCampoApelido> TodasEstruturaCampoApelidoMapeado => this.mapeamentoConsulta.TodasEstruturaCampoApelidoMapeado;

        protected int Skip { get; private set; }
        protected int Take { get; private set; }

        protected BaseSqlBuilder(BaseMapeamentoEntidade mapeamentoConsulta)
        {
            this.mapeamentoConsulta = mapeamentoConsulta;
            this.Skip = this.RetornarSkip();
            this.Take = this.RetornarTake();
        }

        internal virtual string MontarSql(BaseFiltroMapeamento filtroMapeamento,
                                          string sqlCampos,
                                          bool isIncluirOrdenacaoPaginacao,
                                          bool isRelacaoFilhos)
        {
            var sqlConsulta = this.RetornarSqlConsulta(filtroMapeamento,
                                                      isIncluirOrdenacaoPaginacao,
                                                      isRelacaoFilhos);
            return $"SELECT {sqlCampos} FROM {sqlConsulta}";
        }

        private int RetornarSkip()
        {
            if (this.EstruturaConsulta.Skip > 0)
            {
                return this.EstruturaConsulta.Skip;
            }
            else if (this.EstruturaConsulta.PaginaAtual > 0)
            {
                return (this.EstruturaConsulta.PaginaAtual - 1) * this.EstruturaConsulta.Take;
            }
            else
            {
                return 0;
            }
        }

        private int RetornarTake()
        {
            if(this.EstruturaEntidade == null)
            {
                throw new Exception("EstruturaEntidade não foi definida");
            }
            return this.EstruturaEntidade.RetornarMaximoConsulta(this.EstruturaConsulta.Take);
        }
    }

    internal class SqlBuilder : BaseSqlBuilder
    {
        internal SqlBuilder(BaseMapeamentoEntidade mapeamentoConsulta) : base(mapeamentoConsulta) { }
    }
}
