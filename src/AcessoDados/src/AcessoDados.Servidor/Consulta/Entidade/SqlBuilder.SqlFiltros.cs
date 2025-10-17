using Snebur.Reflexao;
using System.Text;

namespace Snebur.AcessoDados.Mapeamento;

internal partial class BaseSqlBuilder
{
    internal const string NOME_PROPRIEDADE_ID = "Id";
    private int ContadorParametro = 0;

    private string RetornarSqlFiltros()
    {
        var sb = new StringBuilder();
        if (this.EstruturaConsulta.FiltroGrupoE.Filtros.Count > 0)
        {
            sb.AppendLine(this.RetornarSqlFiltroGrupo(this.EstruturaConsulta.FiltroGrupoE));
        }

        if (this.EstruturaConsulta.FiltroGrupoOU.Filtros.Count > 0)
        {
            if (this.EstruturaConsulta.FiltroGrupoE.Filtros.Count > 0)
            {
                sb.Append(" OR ");
            }
            sb.AppendLine(this.RetornarSqlFiltroGrupo(this.EstruturaConsulta.FiltroGrupoOU));
        }
        return sb.ToString();
    }

    private string RetornarSqlFiltroGrupo(BaseFiltroGrupo filtroGrupo)
    {
        var filtros = new List<string>();
        foreach (BaseFiltro filtro in filtroGrupo.Filtros)
        {

            switch (filtro)
            {
                case BaseFiltroGrupo baseFiltroGrupo:

                    filtros.Add(this.RetornarSqlFiltroGrupo(baseFiltroGrupo));
                    break;

                case FiltroPropriedade filtroPropriedade:

                    filtros.Add(this.RetornarFiltroPropriedade(filtroPropriedade));
                    break;

                case FiltroIds filtrosId:

                    //if (filtros.Count > 0)
                    //{
                    filtros.Add(this.RetornarFiltroIds(filtrosId));
                    //}
                    break;

                case FiltroPropriedadeIn filtroIn:

                    //if(filtros.Count > 0)
                    //{
                    filtros.Add(this.RetornarFiltroPropriedadeIn(filtroIn));
                    //}
                    break;

                default:
                    throw new ErroNaoSuportado(String.Format("O filtro não suportado {0} ", filtro.GetType().Name));

            }
        }

        if (filtros.Count > 0)
        {
            var juntar = String.Join(this.RetornarOperadorFiltroGrupo(filtroGrupo), filtros);
            var sqlFiltro = String.Format("( {0} )", juntar);
            if (filtroGrupo is FiltroGrupoNAO)
            {
                sqlFiltro = String.Format(" NOT ( {0} ) ", sqlFiltro);
            }
            return sqlFiltro;
        }
        return String.Empty;
    }

    private string RetornarFiltroIds(FiltroIds filtro)
    {
        var estruturaCampoApelidoPropriedade = this.RetornarEstruturaCampoApelido(this.EstruturaEntidade.EstruturaCampoChavePrimaria.Propriedade.Name);
        var caminhoCampoFiltro = estruturaCampoApelidoPropriedade.CaminhoBanco;

        if (filtro.Ids == null)
        {
            filtro.Ids = new List<long>();
        }
        if (filtro.Ids.Count == 0)
        {
            filtro.Ids.Add(0);
        }

        var idsString = String.Join(", ", filtro.Ids.OrderBy(x => x));
        var sql = String.Format(" ( {0} BETWEEN {1} AND {2} ) AND ", caminhoCampoFiltro, filtro.Ids.Min(), filtro.Ids.Max());
        return sql + String.Format(" {0} IN ( {1} ) ", caminhoCampoFiltro, idsString);
    }

    private string RetornarFiltroPropriedadeIn(FiltroPropriedadeIn filtro)
    {
        var estruturaCampoApelidoPropriedade = this.RetornarEstruturaCampoApelido(
            filtro.CaminhoPropriedade);
        var caminhoCampoFiltro = estruturaCampoApelidoPropriedade.CaminhoBanco;
        var tipoPrimarioEnum = estruturaCampoApelidoPropriedade.EstruturaCampo.TipoPrimarioEnum;

        if (filtro.Lista == null)
        {
            filtro.Lista = new List<string>();
        }

        if (filtro.Lista.Count == 0)
        {
            filtro.Lista.Add(this.RetornarValorPadraoListaVazia(tipoPrimarioEnum));
        }

        var listaString = String.Join(", ", this.RetornarValoresFiltroIn(tipoPrimarioEnum, filtro.Lista));
        return $" {caminhoCampoFiltro} IN ( {listaString} ) ";
    }

    private string RetornarValorPadraoListaVazia(EnumTipoPrimario tipoPrimarioEnum)
    {
        if (tipoPrimarioEnum == EnumTipoPrimario.String ||
          tipoPrimarioEnum == EnumTipoPrimario.Guid)
        {
            return Guid.NewGuid().ToString();
        }
        return "0";

    }

    private List<string> RetornarValoresFiltroIn(EnumTipoPrimario tipoPrimarioEnum, List<string> lista)
    {
        if (tipoPrimarioEnum == EnumTipoPrimario.String ||
            tipoPrimarioEnum == EnumTipoPrimario.Guid)
        {
            return lista.Select(x => $"\'{x}'").ToList();
        }
        return lista;
    }

    private string RetornarOperadorFiltroGrupo(BaseFiltroGrupo filtroGrupo)
    {
        if (filtroGrupo is FiltroGrupoE || filtroGrupo is FiltroGrupoNAO)
        {
            return " AND ";
        }
        if (filtroGrupo is FiltroGrupoOU)
        {
            return " OR ";
        }
        throw new Erro(String.Format("filtro grupo não suportado '{0}'", filtroGrupo.GetType().Name));
    }

    private string RetornarFiltroPropriedade(FiltroPropriedade filtroPropriedade)
    {
        Guard.NotNull(filtroPropriedade.CaminhoPropriedade);

        var estruturaCampoApelidoPropriedade = this.RetornarEstruturaCampoApelido(filtroPropriedade.CaminhoPropriedade);
        var estruturaCampo = estruturaCampoApelidoPropriedade.EstruturaCampo;
        var operadorFiltroPropriedade = this.RetornarOperadorFiltroPropriedade(filtroPropriedade);

        if (filtroPropriedade.Valor == null)
        {
            var valorPropriedade = this.RetornarValorPropriedadeFiltro(filtroPropriedade);
            return String.Format("(  {0} {1} {2} ) ", estruturaCampoApelidoPropriedade.CaminhoBanco,
                                                                         operadorFiltroPropriedade,
                                                                         valorPropriedade);

        }
        else
        {

            this.ContadorParametro += 1;
            var nomeParametro = $"{estruturaCampo.NomeParametro}{this.ContadorParametro}";
            var valorPropriedadeTipado = this.RetornarValorPropriedadeTipado(filtroPropriedade, estruturaCampo);

            if (estruturaCampo.IsPossuiIndiceTextoCompleto &&
                this.IsOperadorTextoPesquisa(filtroPropriedade.Operador))
            {
                var estruturasCampos = estruturaCampo.EstruturaEntidade.EstruturasCamposIndiceTextoCompleto;
                if (estruturasCampos.Count > 1)
                {
                    var estruturasCamposApelidos = estruturasCampos.Select(x => this.RetornarEstruturaCampoApelido(x.Propriedade.Name)).ToList();
                    return this.RetornarSqlTextoCompletoPesquisa(estruturasCamposApelidos,
                                                                 filtroPropriedade,
                                                                 nomeParametro);
                }
                return this.RetornarSqlTextoCompletoPesquisa(estruturaCampoApelidoPropriedade,
                                                             filtroPropriedade,
                                                             nomeParametro);
            }
            else
            {

                this.AdicionarParametroInfo(this.ConexaoDB.RetornarParametroInfo(estruturaCampoApelidoPropriedade.EstruturaCampo, nomeParametro, valorPropriedadeTipado));

                var condicaoNot = filtroPropriedade.TipoPrimarioEnum == EnumTipoPrimario.String &&
                                  filtroPropriedade.Operador == EnumOperadorFiltro.Diferente ?
                                  " not " : String.Empty;

                return $"  ( {condicaoNot} {estruturaCampoApelidoPropriedade.CaminhoBanco} {operadorFiltroPropriedade} {nomeParametro} ) ";
            }
        }
    }

    private void AdicionarParametroInfo(ParametroInfo parametroInfo)
    {
        this._mapeamentoConsulta.AdicionarParametroInfo(parametroInfo);
    }

    private bool IsOperadorTextoPesquisa(EnumOperadorFiltro operador)
    {
        switch (operador)
        {
            case EnumOperadorFiltro.Igual:
            case EnumOperadorFiltro.Diferente:
            case EnumOperadorFiltro.IniciaCom:
            case EnumOperadorFiltro.TerminaCom:
            case EnumOperadorFiltro.Possui:
                return true;
            default:
                return false;
        }
    }

    private string RetornarSqlTextoCompletoPesquisa(List<EstruturaCampoApelido> estruturasCampoApelidoPropriedade,
                                                    FiltroPropriedade filtroPropriedade,
                                                    string nomeParametro)
    {

        var valor = filtroPropriedade.Valor as string ?? String.Empty;
        var valorPesquisa = this.RetornarValorPesquisaTextoCompleto(valor, filtroPropriedade.Operador);
        this.AdicionarParametroInfo(this.ConexaoDB.RetornarParametroInfo(estruturasCampoApelidoPropriedade.First().EstruturaCampo, nomeParametro, valorPesquisa));
        var prefixoNot = filtroPropriedade.Operador == EnumOperadorFiltro.Diferente ? " not " : "";
        var colunas = String.Join(", ", estruturasCampoApelidoPropriedade.Select(x => x.CaminhoBanco));
        return $" {prefixoNot} CONTAINS( ({colunas}), {nomeParametro} )   ";
    }

    private string RetornarSqlTextoCompletoPesquisa(EstruturaCampoApelido estruturaCampoApelidoPropriedade,
                                                    FiltroPropriedade filtroPropriedade,
                                                    string nomeParametro)
    {
        var valor = filtroPropriedade.Valor as string ?? String.Empty;
        var valorPesquisa = this.RetornarValorPesquisaTextoCompleto(valor, filtroPropriedade.Operador);
        this.AdicionarParametroInfo(this.ConexaoDB.RetornarParametroInfo(estruturaCampoApelidoPropriedade.EstruturaCampo, nomeParametro, valorPesquisa));
        var prefixoNot = filtroPropriedade.Operador == EnumOperadorFiltro.Diferente ? " not " : "";
        return $" {prefixoNot} CONTAINS( ({estruturaCampoApelidoPropriedade.CaminhoBanco}), {nomeParametro} )   ";
    }

    private string RetornarValorPesquisaTextoCompleto(string valor, EnumOperadorFiltro operador)
    {
        switch (operador)
        {
            case EnumOperadorFiltro.Igual:
            case EnumOperadorFiltro.Diferente:

                return valor;

            case EnumOperadorFiltro.IniciaCom:

                return $"\"{valor}*\"";

            case EnumOperadorFiltro.TerminaCom:

                return $"\"*{valor}\"";

            case EnumOperadorFiltro.Possui:

                return $"\"*{valor}*\"";

            default:

                throw new Erro("Operador não suportado pela pesquisa de texto completo");
        }
    }

    private EstruturaCampoApelido RetornarEstruturaCampoApelido(string? caminhoPropriedade)
    {
        Guard.NotNullOrWhiteSpace(caminhoPropriedade);

        if (caminhoPropriedade == NOME_PROPRIEDADE_ID)
        {
            return this.EstruturaCampoApelidoChavePrimaria;
        }
        else
        {
            return this.TodasEstruturaCampoApelidoMapeado[caminhoPropriedade] ??
                throw new Exception(
                    $"A propriedade '{caminhoPropriedade}' não foi mapeada na entidade '{this.EstruturaEntidade.NomeTipoEntidade}' ");
        }
    }

    private string RetornarOperadorFiltroPropriedade(FiltroPropriedade filtroPropriedade)
    {
        if (filtroPropriedade.Valor == null)
        {
            switch (filtroPropriedade.Operador)
            {
                case EnumOperadorFiltro.Igual:
                case EnumOperadorFiltro.IgualAbsoluto:

                    return " is ";

                case EnumOperadorFiltro.Diferente:

                    return " is  not ";

                default:

                    throw new ErroNaoSuportado($"O operador  {EnumUtil.RetornarDescricao(filtroPropriedade.Operador)}  não é suportado com valor da propriedade nulo  'null'  ");

            }
        }
        switch (filtroPropriedade.TipoPrimarioEnum)
        {
            case (EnumTipoPrimario.String):

                return this.RetornarOperadorFiltroString(filtroPropriedade);

            default:

                return this.RetornarOperadorFiltroPropriedadeLogico(filtroPropriedade.Operador);
        }
    }

    private string RetornarOperadorFiltroPropriedadeLogico(EnumOperadorFiltro operador)
    {
        switch (operador)
        {
            case EnumOperadorFiltro.Igual:
            case EnumOperadorFiltro.IgualAbsoluto:

                return " = ";

            case EnumOperadorFiltro.Diferente:

                return " <> ";

            case EnumOperadorFiltro.Maior:

                return " > ";

            case EnumOperadorFiltro.MaiorIgual:

                return " >= ";

            case EnumOperadorFiltro.Menor:

                return " < ";

            case EnumOperadorFiltro.MenorIgual:

                return " <= ";

            default:
                throw new ErroNaoSuportado(String.Format("Operador não suportado {0} ", EnumUtil.RetornarDescricao(operador)));
        }
    }

    private object? RetornarValorPropriedadeTipado(FiltroPropriedade filtroPropriedade, Estrutura.EstruturaCampo estruturaCampo)
    {
        if (filtroPropriedade.TipoPrimarioEnum == EnumTipoPrimario.String && filtroPropriedade.Valor != null)
        {
            var valor = this.RetornarValorPropriedaeParametroString(filtroPropriedade);
            if (estruturaCampo.IsFormatarSomenteNumero)
            {
                return TextoUtil.RetornarSomenteNumeros(valor);
            }
            return valor;
        }
        else
        {
            return ConverterUtil.ConverterTipoPrimario(filtroPropriedade.Valor, filtroPropriedade.TipoPrimarioEnum);
        }
    }

    private string RetornarValorPropriedadeFiltro(FiltroPropriedade filtroPropriedade)
    {
        var valorPropriedade = filtroPropriedade.Valor;
        if (valorPropriedade == null)
        {
            return " null ";
        }
        else
        {
            switch (filtroPropriedade.TipoPrimarioEnum)
            {
                case (EnumTipoPrimario.String):

                    return this.RetornarValorPropriedaeFiltroString(filtroPropriedade);

                case (EnumTipoPrimario.Integer):
                case (EnumTipoPrimario.EnumValor):

                    return Convert.ToInt32(valorPropriedade).ToString();

                case (EnumTipoPrimario.Boolean):

                    return Convert.ToInt32(Convert.ToBoolean(valorPropriedade)).ToString();

                case (EnumTipoPrimario.Long):

                    return Convert.ToInt64(valorPropriedade).ToString();

                case (EnumTipoPrimario.Decimal):

                    return Convert.ToDecimal(valorPropriedade).ToString().Replace(",", ".");

                case (EnumTipoPrimario.Guid):

                    return String.Format(" '{0}' ", valorPropriedade.ToString());

                case (EnumTipoPrimario.DateTime):

                    return this.RetornarValorPropriedaeFiltroDataHora(filtroPropriedade);

                case (EnumTipoPrimario.TimeSpan):

                    return this.RetornarValorPropriedaeFiltroTimeSpan(filtroPropriedade);

                default:

                    throw new NotImplementedException();
            }
        }
    }

    #region String

    private string RetornarValorPropriedaeParametroString(FiltroPropriedade filtroPropriedade)
    {
        var valorString = filtroPropriedade.Valor?.ToString();
        switch (filtroPropriedade.Operador)
        {
            case (EnumOperadorFiltro.Igual):
            case (EnumOperadorFiltro.IgualAbsoluto):
            case (EnumOperadorFiltro.Diferente):

                return valorString ?? String.Empty;

            case (EnumOperadorFiltro.IniciaCom):

                return String.Format("{0}%", valorString);

            case (EnumOperadorFiltro.TerminaCom):

                return String.Format(" '%{0}", valorString);

            case (EnumOperadorFiltro.Possui):

                return String.Format("%{0}%", valorString);

            default:
                throw new ErroNaoSuportado("Operador para string não é suportado");
        }
    }

    private string RetornarValorPropriedaeFiltroString(FiltroPropriedade filtroPropriedade)
    {
        switch (filtroPropriedade.Operador)
        {
            case (EnumOperadorFiltro.Igual):
            case (EnumOperadorFiltro.IgualAbsoluto):
            case (EnumOperadorFiltro.Diferente):

                return String.Format(" '{0}' ", filtroPropriedade.Valor?.ToString()?.Replace("'", "''"));

            case (EnumOperadorFiltro.IniciaCom):

                return String.Format(" '{0}%' ", filtroPropriedade.Valor?.ToString());

            case (EnumOperadorFiltro.TerminaCom):

                return String.Format(" '%{0}' ", filtroPropriedade.Valor?.ToString());

            case (EnumOperadorFiltro.Possui):

                return String.Format(" '%{0}%' ", filtroPropriedade.Valor?.ToString());

            default:
                throw new ErroNaoSuportado("Operador para string não é suportado");
        }
    }

    private string RetornarOperadorFiltroString(FiltroPropriedade filtroPropriedade)
    {
        if (filtroPropriedade.Operador == EnumOperadorFiltro.IgualAbsoluto)
        {
            return " = ";
        }
        switch (ConfiguracaoAcessoDados.TipoBancoDadosEnum)
        {

            case (EnumTipoBancoDados.SQL_SERVER):

                return " like ";

            case (EnumTipoBancoDados.PostgreSQL):
            case (EnumTipoBancoDados.PostgreSQLImob):

                return " ilike ";

            default:

                throw new ErroNaoSuportado("O tipo do banco de dados não é suportado");
        }
    }
    #endregion

    #region DataHora

    private string RetornarValorPropriedaeFiltroDataHora(FiltroPropriedade filtroPropriedade)
    {
        return String.Format(" '{0}' ", filtroPropriedade.Valor?.ToString());
    }

    private string RetornarValorPropriedaeFiltroTimeSpan(FiltroPropriedade filtroPropriedade)
    {
        return String.Format(" '{0}' ", filtroPropriedade.Valor?.ToString());
    }
    #endregion
}