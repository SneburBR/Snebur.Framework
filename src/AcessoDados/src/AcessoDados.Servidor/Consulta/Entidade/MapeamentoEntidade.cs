using Snebur.AcessoDados.Estrutura;
using Snebur.Linq;
using System.Diagnostics;
using System.Threading;

namespace Snebur.AcessoDados.Mapeamento;

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

    internal protected List<Entidade> RetornarEntidades(
        BaseFiltroMapeamento filtro,
        int tentativa = 0)
    {
        var sql = this.RetornarSql(filtro, isIncluirOrdenacaoPaginacao: true);
        try
        {

            var dataTable = this.ConexaoDB.RetornarDataTable(sql, this.ParametrosInfo);
            var entidades = AjudanteDataSetMapeamento.MapearDataTable(this.EstruturaConsulta, dataTable, this);

            if (this.EstruturaEntidade.IsInterceptar && entidades.Count > 0)
            {
                var interceptor = this.EstruturaEntidade.Interceptador;
                if (interceptor is not null &&
                    this.Contexto.IsInterceptar &&
                    !this.Contexto.InterceptoresAtivos.Contains(interceptor))
                {
                    this.Contexto.InterceptoresAtivos.Add(interceptor);
                    try
                    {
                        entidades = interceptor.__Interceptar(this.Contexto, entidades) as List<Entidade>;
                    }
                    finally
                    {
                        this.Contexto.InterceptoresAtivos.Remove(interceptor);
                    }
                }
                return entidades;
            }
            return entidades;
        }
        catch(Exception ex)
        { 
            throw new ErroExecutarSql(sql, this.ParametrosInfo, filtro.ToString(), ex);
        }
    }

    internal protected List<IdTipoEntidade> RetornarIdTipoEntidade(BaseFiltroMapeamento filtro)
    {
        var sql = this.RetornarSqlIdTipoEntidade(filtro);
        var dataTable = this.ConexaoDB.RetornarDataTable(sql, this.ParametrosInfo);
        var idsTipoEntidade = AjudanteDataSetMapeamento.MapearIdTipoEntidade(dataTable);
        return idsTipoEntidade;
    }

    internal protected int RetornarContagem(BaseFiltroMapeamento filtro)
    {
        var sql = this.RetornarSqlContagem(filtro);
        var contagem = Convert.ToInt32(this.ConexaoDB.RetornarValorScalar(sql, this.ParametrosInfo));
        return contagem;
    }

    private string RetornarSqlContagem(BaseFiltroMapeamento filtro)
    {
        return this.MontarSql(filtro, sqlCampos: "COUNT(*)", isIncluirOrdenacaoPaginacao: false);
    }

    internal protected override string RetornarSqlCampos()
    {
        var sepador = String.Format(" , \t {0}  ", Environment.NewLine);

        //var estruturasCamposEntidadeAtual = this.TodasEstruturaCampoMapeado.Values.Where(x => x.EstruturaEntidadeApelido == this.EstruturaEntidadeApelido);
        var estruturasCampoApelido = this.RetornarEstuturaCamposApelido();
        var camposCaminhoBanco = estruturasCampoApelido.Select(x => x.RetornarCaminhoBancoApelido());
        var sqlCampos = String.Join(sepador, camposCaminhoBanco);
        return AjudanteSql.RetornarSqlFormatado(sqlCampos);
    }

    internal List<EstruturaCampoApelido> RetornarEstuturaCamposApelido()
    {
        var propriedadesAbertas = this.EstruturaConsulta.PropriedadesAbertas;
        if (propriedadesAbertas.Count == 0)
        {
            return this.TodasEstruturaCampoApelidoMapeado.Values.ToList();
        }
        var estruturasCampoApelido = new List<EstruturaCampoApelido>
        {
            this.EstruturaCampoApelidoChavePrimaria
        };

        if (this.EstruturaEntidade.IsImplementaInterfaceIDeletado)
        {
            propriedadesAbertas.AddIfNotExits(nameof(IDeletado.IsDeletado));
        }

        if (this.EstruturaEntidade.IsImplementaInterfaceIOrdenacao)
        {
            propriedadesAbertas.AddIfNotExits(nameof(IOrdenacao.Ordenacao));
        }

        foreach (var propriedadeAberta in propriedadesAbertas)
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
        filtroMapeamento.IsIdTipoEntidade = true;
        var sqlCampos = this.RetornarSqlCamposIdTipoEntidade(filtroMapeamento);
        return this.MontarSql(filtroMapeamento,
                              sqlCampos,
                              isIncluirOrdenacaoPaginacao: true);
    }

    internal string RetornarSqlCamposIdTipoEntidade(BaseFiltroMapeamento filtro)
    {
        var estruturaChavePrimariaApelido = this.EstruturaEntidadeApelido.EstruturaCampoApelidoChavePrimaria ??
            throw new Erro($"A estrutura entidade {this.EstruturaEntidadeApelido.EstruturaEntidade.NomeTipoEntidade} não possui chave primária");

        var campoChavePrimaria = AjudanteSql.RetornarSqlFormatado(String.Format("{0} As [Id] ", estruturaChavePrimariaApelido.CaminhoBanco));
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