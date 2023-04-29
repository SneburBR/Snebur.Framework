using Snebur.AcessoDados.Estrutura;
using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Snebur.AcessoDados.Mapeamento
{
    internal class MapeamentoEntidade : BaseMapeamentoEntidade
    {

        internal MapeamentoEntidade(MapeamentoConsulta mapeamentoConsulta,
                                    EstruturaEntidade estruturaEntidade,
                                    EstruturaBancoDados estruturaBancoDados,
                                    BaseConexao conexaoDB,
                                    BaseContextoDados contexto) :
                                    base(mapeamentoConsulta, estruturaEntidade, estruturaBancoDados, conexaoDB, contexto)
        {
            if ((this.EstruturaConsulta.Take == 0) ||
                (this.EstruturaConsulta.Take > estruturaEntidade.MaximoRegistroPorConsulta))
            {
                this.EstruturaConsulta.Take = estruturaEntidade.MaximoRegistroPorConsulta;
            }
        }

        internal protected ListaEntidades<Entidade> RetornarEntidades(BaseFiltroMapeamento filtro, int tentativa = 0)
        {
            var sql = this.RetornarSql(true, true, filtro);
            try
            {
                var dataTable = this.ConexaoDB.RetornarDataTable(sql, this.Parametros);
                var entidades = AjudanteDataSetMapeamento.MapearDataTable(this.EstruturaConsulta, dataTable, this);
                return entidades;
            }
            catch
            {
                if (DebugUtil.IsAttached && tentativa < 50)
                {
                    return this.RetornarEntidades(filtro, tentativa++);
                }
                throw;
            }

        }

        internal protected List<IdTipoEntidade> RetornarIdTipoEntidade(BaseFiltroMapeamento filtro)
        {

            var sql = this.RetornarSqlIdTipoEntidade(filtro);
            var dataTable = this.ConexaoDB.RetornarDataTable(sql, this.Parametros);
            var idsTipoEntidade = AjudanteDataSetMapeamento.MapearIdTipoEntidade(dataTable);
            return idsTipoEntidade;
        }

        internal protected int RetornarContagem(BaseFiltroMapeamento filtro)
        {
            var sql = this.RetornarSqlContagem(filtro);
            var contagem = Convert.ToInt32(this.ConexaoDB.RetornarValorScalar(sql, this.Parametros));
            return contagem;
        }

        internal protected override string RetornarSqlCampos()
        {
            var sepador = String.Format(" , \t {0}  ", System.Environment.NewLine);

            //var estruturasCamposEntidadeAtual = this.TodasEstruturaCampoMapeado.Values.Where(x => x.EstruturaEntidadeApelido == this.EstruturaEntidadeApelido);
            var estruturasCampoApelido = this.RetornarEstuturaCamposApelido();

            var camposCaminhoBanco = estruturasCampoApelido.Select(x => x.RetornarCaminhoBancoApelido());
            var sqlCampos = String.Join(sepador, camposCaminhoBanco);
            return AjudanteSql.RetornarSqlFormatado(sqlCampos);
        }

        internal List<EstruturaCampoApelido> RetornarEstuturaCamposApelido()
        {
            if (this.EstruturaConsulta.PropriedadesAbertas.Count == 0)
            {
                return this.TodasEstruturaCampoApelidoMapeado.Values.ToList();
            }
            var estruturasCampoApelido = new List<EstruturaCampoApelido>();
            estruturasCampoApelido.Add(this.EstruturaCampoApelidoChavePrimaria);

            foreach (var propriedadeAberta in this.EstruturaConsulta.PropriedadesAbertas)
            {
                var estruturaCampoApelido = this.TodasEstruturaCampoApelidoMapeado[propriedadeAberta];
                estruturasCampoApelido.Add(estruturaCampoApelido);
            }
            estruturasCampoApelido.AddRange(this.EstruturasCampoApelidoChaveEstrangeiraRelacoesAberta);

            var estruturasCampoApelidoChaveEstrageira = this.TodasEstruturaCampoApelidoMapeado.Values.Where(x => x.EstruturaCampo.IsRelacaoChaveEstrangeira).ToList();
            estruturasCampoApelido.AddRange(estruturasCampoApelidoChaveEstrageira);

            return estruturasCampoApelido;
        }

        internal string RetornarSqlIdTipoEntidade(BaseFiltroMapeamento filtroMapeamento)
        {
            var isRelacaoFilhos = this.MapeamentoConsulta is MapeamentoConsultaRelacaoAbertaFilhos;
            filtroMapeamento.IsIdTipoEntidade = true;
            var sqlCampos = this.RetornarSqlCamposIdTipoEntidade(filtroMapeamento);
            var sqlJoin = this.RetornarSqlConsulta(true, true, filtroMapeamento, isRelacaoFilhos);
            if (this.Contexto.SqlSuporte.IsOffsetFetch)
            {
                return $"SELECT {sqlCampos} FROM {sqlJoin}";
            }
            else
            {
                var take = this.MapeamentoConsulta.EstruturaEntidade.RetornarMaximoConsulta(this.EstruturaConsulta.Take);
                //sqlJoin = sqlJoin.Replace("ORDER BY [Id]", "WHERE not __NomeTipoEntidade is null ");
                return $"SELECT Top {take} {sqlCampos} FROM {sqlJoin} ";
            }
        }

        internal string RetornarSqlCamposIdTipoEntidade(BaseFiltroMapeamento filtro)
        {
            var campoChavePrimaria = AjudanteSql.RetornarSqlFormatado(String.Format("{0} As [Id] ", this.EstruturaEntidadeApelido.EstruturaCampoApelidoChavePrimaria.CaminhoBanco));
            var campoTipoTipoEntidade = AjudanteSql.RetornarSqlFormatado(this.RetornarCampoTipoEntidade());
            var campos = String.Format(" {0} , {1} ", campoChavePrimaria, campoTipoTipoEntidade);

            if (filtro.EstruturaCampoFiltro != null)
            {
                var tipoPropridadeSemNullable = ReflexaoUtil.RetornarTipoSemNullable(filtro.EstruturaCampoFiltro.Propriedade.PropertyType);

                if (!(tipoPropridadeSemNullable != typeof(long) || tipoPropridadeSemNullable != typeof(int)))
                {
                    throw new NotImplementedException();
                }
                var estruturaCampoFiltroApelido = this.TodasEstruturaCampoApelidoMapeado[filtro.EstruturaCampoFiltro.Propriedade.Name];
                campos += AjudanteSql.RetornarSqlFormatado(String.Format(", {0} As [CampoFiltro] ", estruturaCampoFiltroApelido.CaminhoBanco));
            }
            return campos;
        }

        internal string RetornarCampoTipoEntidade()
        {
            switch (ConfiguracaoAcessoDados.TipoBancoDadosEnum)
            {
                case (EnumTipoBancoDados.SQL_SERVER):

                    return this.EstruturaEntidadeApelido.CaminhoCampoNomeTipoEntidade();
                //return string.Format(" {0}.[NomeTipoEntidade] As [NomeTipoEntidade]  ", this.EstruturaEntidadeApelido.Apelido);

                case (EnumTipoBancoDados.PostgreSQL):

                    return this.EstruturaEntidadeApelido.CaminhoCampoNomeTipoEntidade();
                //return string.Format(" {0}.[NomeTipoEntidade] As [NomeTipoEntidade]  ", this.EstruturaEntidadeApelido.Apelido);

                case (EnumTipoBancoDados.PostgreSQLImob):

                    return " CAST( tableoid::regclass As text) As \"NomeTipoEntidade\" ";

                default:
                    throw new ErroNaoSuportado(String.Format("O tipo do banco de dados não é suportado"));
            }
        }
    }
}