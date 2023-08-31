using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snebur.AcessoDados.Mapeamento
{
    internal partial class BaseSqlBuilder
    {

        protected string RetornarSqlConsulta(BaseFiltroMapeamento filtroMapeamento,
                                            bool isIncluirOrdenacaoPaginacao,
                                            bool isRelacaoFilhos)
        {
            ErroUtil.ValidarReferenciaNula(filtroMapeamento, nameof(filtroMapeamento));

            var sb = new StringBuilderSql();
            var isOperadorWhereAdicionado = false;

            //Entidade

            if (this.EstruturaEntidadeApelido.EstruturasEntidadeMapeadaBase.Count > 0)
            {
                sb.AppendLine(" ( ");
            }

            sb.AppendLine($"  {this.EstruturaEntidadeApelido.CaminhoBanco} As {this.EstruturaEntidadeApelido.Apelido}  ");

            if (this.EstruturaEntidadeApelido.EstruturasEntidadeMapeadaBase.Count > 0)
            {
                sb.Append($" {Environment.NewLine} \t  INNER JOIN  ");
                sb.AppendLine($" \t {this.EstruturaEntidadeApelido.RetornarSqlInnerJoinTabelasEntidadeBase()} ");
            }

            if (this.EstruturaEntidadeApelido.EstruturasEntidadeMapeadaBase.Count > 0)
            {
                sb.AppendLine(" ) ");
            }

            sb.AppendLine(" ");
            sb.AppendLine(this.RetornarSqlsJoinRelacao(this.EstruturaEntidadeApelido));

            //Outras relações  implementar aqui - RelacaoFilho

            sb.AppendLine("  ");

            if (this.Contexto.IsFiltrarIdentificadorProprietario && this.Contexto.IdentificadorProprietario != null &&
                    !this.Contexto.IsIdentificadorProprietarioGlobal)
            {
                var estrutaCampoIdentificadorProprietario = this.EstruturaEntidade.EstruturaCampoIdentificadorProprietario;
                if (estrutaCampoIdentificadorProprietario != null)
                {
                    var identificadorProprietario = this.Contexto.IdentificadorProprietario;

                    var estruturaApelido = this.TodasEstruturaCampoApelidoMapeado[estrutaCampoIdentificadorProprietario.Propriedade.Name];
                    sb.AppendLine(" WHERE ( ");
                    sb.AppendLine($"{estruturaApelido.CaminhoBanco} = {identificadorProprietario.ToString()} ");

                    sb.AppendLine(" ) ");

                    isOperadorWhereAdicionado = true;
                }
            }


            if (!(filtroMapeamento is FiltroMapeamentoIds) || isRelacaoFilhos)
            {
                if (!this.EstruturaConsulta.IsIncluirDeletados &&
                     this.EstruturaEntidade.EstruturaCampoDelatado != null)
                {
                    if (!isOperadorWhereAdicionado)
                    {
                        sb.AppendLine(" WHERE ");
                    }
                    else
                    {
                        sb.AppendLine(" AND ");
                    }
                    var estruturaApelido = this.TodasEstruturaCampoApelidoMapeado[this.EstruturaEntidade.EstruturaCampoDelatado.Propriedade.Name];

                    sb.AppendLine($" {estruturaApelido.CaminhoBanco} = 0 ");
                    isOperadorWhereAdicionado = true;
                }
            }

            if (!(filtroMapeamento is FiltroMapeamentoVazio))
            {
                var sqlFiltros = this.RetornarSqlFiltroMapeamento(filtroMapeamento);
                if (!String.IsNullOrWhiteSpace(sqlFiltros))
                {
                    var operadorFiltro = !isOperadorWhereAdicionado ? " WHERE " :
                                                                      " AND ";

                    sb.AppendLine($" {operadorFiltro} ( {sqlFiltros} ) ");
                }
                isOperadorWhereAdicionado = true;
            }

            if (this.EstruturaConsulta.FiltroGrupoE?.Filtros.Count > 0 ||
                this.EstruturaConsulta.FiltroGrupoOU?.Filtros.Count > 0)
            {
                var sqlFiltros = this.RetornarSqlFiltros();
                if (!String.IsNullOrWhiteSpace(sqlFiltros))
                {
                    var operadorFiltro = !isOperadorWhereAdicionado ? " WHERE " :
                                                                      " AND ";

                    sb.AppendLine($" {operadorFiltro} ( {sqlFiltros} ) ");
                }

            }

            if (isIncluirOrdenacaoPaginacao)
            {
                sb.AppendLine(this.RetornarSqlOrdenacao(filtroMapeamento));
            }

            if (isIncluirOrdenacaoPaginacao)
            {
                if (filtroMapeamento is FiltroMapeamentoVazio)
                {
                    sb.AppendLine(this.RetornarSqlLimite());
                }
            }
            return sb.ToString();
        }

        private string RetornarSqlsJoinRelacao(EstruturaEntidadeApelido estruturaEntidadeMapeada)
        {
            var sb = new StringBuilderSql();

            var estrurasEntidadeMapeadaRelacaoPai = estruturaEntidadeMapeada.EstruturasEntidadeRelacaoMapeadaInterna.OfType<EstruturaEntidadeApelidoRelacaoPai>().ToList();

            foreach (var estruturaEntidadeMapeadaRelacao in estrurasEntidadeMapeadaRelacaoPai)
            {
                sb.AppendLine("");
                sb.AppendLine(this.RetornarSqlJoinRelacaoPai(estruturaEntidadeMapeadaRelacao));
                sb.AppendLine("");
            }
            return sb.ToString();
        }

        private string RetornarSqlJoinRelacaoPai(EstruturaEntidadeApelidoRelacaoPai estruturaEntidadeMapeadaRelacao)
        {
            var sb = new StringBuilderSql();

            var ligacaoJoin = estruturaEntidadeMapeadaRelacao.EstruturaRelacao.IsRequerido
                                ? " INNER JOIN "
                                : " LEFT OUTER JOIN ";

            sb.Append($" \r\n \t  {ligacaoJoin}  ");

            if (estruturaEntidadeMapeadaRelacao.EstruturasEntidadeMapeadaBase.Count > 0)
            {
                sb.AppendLine(" ( ");
            }
            sb.AppendLine($"  {estruturaEntidadeMapeadaRelacao.CaminhoBanco} As {estruturaEntidadeMapeadaRelacao.Apelido}  ");

            if (estruturaEntidadeMapeadaRelacao.EstruturasEntidadeMapeadaBase.Count > 0)
            {
                sb.Append($"\r\n \t  INNER JOIN  ");

                sb.AppendLine(estruturaEntidadeMapeadaRelacao.RetornarSqlInnerJoinTabelasEntidadeBase());
            }

            if (estruturaEntidadeMapeadaRelacao.EstruturasEntidadeMapeadaBase.Count > 0)
            {
                sb.AppendLine(" ) ");
            }

            sb.AppendLine($" ON {estruturaEntidadeMapeadaRelacao.EstruturaCampoApelidoChavePrimaria.CaminhoBanco} = {estruturaEntidadeMapeadaRelacao.EstruturaCampoChaveEstrangeiraMapeado.CaminhoBanco} ");

            if (estruturaEntidadeMapeadaRelacao.EstruturasEntidadeRelacaoMapeadaInterna.Count > 0)
            {
                sb.AppendLine("");

                sb.AppendLine(this.RetornarSqlsJoinRelacao(estruturaEntidadeMapeadaRelacao));

                sb.AppendLine("");
            }
            return sb.ToString();
        }

        protected string RetornarSqlOrdenacao(BaseFiltroMapeamento filtroMapeamento)
        {
            var sb = new StringBuilderSql();
            if (this.EstruturaConsulta.Ordenacoes.Count > 0)
            {
                sb.AppendLine(this.RetornarSqlOrdenacaoInterno());
            }
            else
            {
                if (!this.EstruturaConsulta.IsDesativarOrdenacao &&
                    this.EstruturaEntidade.EstruturaCampoOrdenacao != null)
                {
                    var campoApelido = this.TodasEstruturaCampoApelidoMapeado[this.EstruturaEntidade.EstruturaCampoOrdenacao.Propriedade.Name];
                    sb.AppendLine($" \r\n \t ORDER BY {campoApelido.CaminhoBanco} ");
                }
                else
                {
                    sb.AppendLine(this.RetornarSqlOrdenacaoChavePrimaria(filtroMapeamento));
                }

            }
            return sb.ToString();
        }

        private string RetornarSqlOrdenacaoInterno()
        {
            var ordenacoes = new List<string>();
            foreach (var ordenacao in this.EstruturaConsulta.Ordenacoes.Values)
            {
                var estruturaCampoApelido = this.TodasEstruturaCampoApelidoMapeado[ordenacao.CaminhoPropriedade];
                var sqlOrdenacao = (ordenacao.SentidoOrdenacaoEnum == EnumSentidoOrdenacao.Decrescente)
                                    ? $" {estruturaCampoApelido.CaminhoBanco} DESC "
                                    : estruturaCampoApelido.CaminhoBanco;

                ordenacoes.Add(sqlOrdenacao);
            }
            return " ORDER BY " + String.Join(", ", ordenacoes);
        }

        private string RetornarSqlOrdenacaoChavePrimaria(BaseFiltroMapeamento filtro)
        {
            if (filtro.IsIdTipoEntidade)
            {
                return " ORDER BY [Id]";
            }
            else
            {
                var estruturaCampoApelidoChavePrimaria = this.EstruturaEntidadeApelido.EstruturaCampoApelidoChavePrimaria;
                return " ORDER BY " + estruturaCampoApelidoChavePrimaria.CaminhoBanco;
            }
        }

        private string RetornarSqlLimite()
        {
            var skip = this.Skip;
            if (this.Contexto.SqlSuporte.IsOffsetFetch)
            {
                string sqlLimite = "";
                sqlLimite += " OFFSET  " + skip;

                if (ConfiguracaoAcessoDados.TipoBancoDadosEnum == EnumTipoBancoDados.SQL_SERVER)
                {
                    sqlLimite += " ROWS ";
                }
                if (this.Take > 0)
                {
                    sqlLimite += String.Format("  FETCH NEXT {0} ROWS ONLY  ", this.Take);
                }
                return sqlLimite;
            }

            if (skip > 0)
            {
                throw new Exception("O banco da deados não tem suporte ao OFFSET FETCH");
            }
            return String.Empty;

        }


    }
}
