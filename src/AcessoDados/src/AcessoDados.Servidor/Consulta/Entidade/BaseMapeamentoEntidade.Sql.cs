using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snebur.AcessoDados.Mapeamento
{
    internal abstract partial class BaseMapeamentoEntidade
    {
        internal string MontarSql(BaseFiltroMapeamento filtroMapeamento,
                                  string sqlCampos,
                                  bool isIncluirOrdenacaoPaginacao)
        {
            var isRelacaoFilhos = this.MapeamentoConsulta is MapeamentoConsultaRelacaoAbertaFilhos;
            var sqlJoin = this.RetornarSqlConsulta(filtroMapeamento, isRelacaoFilhos, isIncluirOrdenacaoPaginacao);
            if (this.Contexto.SqlSuporte.IsOffsetFetch || !isIncluirOrdenacaoPaginacao)
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
         
        


        //é precisa refazer, 
        protected string RetornarSqlConsulta(BaseFiltroMapeamento filtroMapeamento,
                                             bool isRelacaoFilhos,
                                             bool isIncluirOrdenacaoPaginacao)
        {
            ErroUtil.ValidarReferenciaNula(filtroMapeamento, nameof(filtroMapeamento));

            var sb = new StringBuilderSql();
            var isOperadorWhereAdicionado = false;

            //Entidade

            if (this.EstruturaEntidadeApelido.EstruturasEntidadeMapeadaBase.Count > 0)
            {
                sb.AppendLine(" ( ");
            }
            sb.AppendLineFormat("  {0} As {1}  ", this.EstruturaEntidadeApelido.CaminhoBanco, this.EstruturaEntidadeApelido.Apelido);

            if (this.EstruturaEntidadeApelido.EstruturasEntidadeMapeadaBase.Count > 0)
            {
                sb.AppendFormat(" {0} \t  INNER JOIN  ", System.Environment.NewLine);

                sb.AppendLineFormat(" \t {0} ", this.EstruturaEntidadeApelido.RetornarSqlInnerJoinTabelasEntidadeBase());
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
                if (this.EstruturaConsulta.Ordenacoes.Count > 0)
                {
                    sb.AppendLine(this.RetornarSqlOrdenacao());
                }
                else
                {
                    if (!this.EstruturaConsulta.IsDesativarOrdenacao &&
                        this.EstruturaEntidade.EstruturaCampoOrdenacao != null)
                    {
                        //if (!filtroMapeamento.IsIdTipoEntidade)
                        //{

                        //}
                        var campoApelido = this.TodasEstruturaCampoApelidoMapeado[this.EstruturaEntidade.EstruturaCampoOrdenacao.Propriedade.Name];
                        sb.AppendLineFormat(" ORDER BY {0}", campoApelido.CaminhoBanco);
                        //throw new NotImplementedException();
                    }
                }
            }
            if (isIncluirOrdenacaoPaginacao)
            {
                if (this.EstruturaEntidade.EstruturaCampoOrdenacao == null && (this.EstruturaConsulta.Ordenacoes.Count == 0))
                {
                    sb.AppendLine(this.RetornarSqlOrdenacaoChavePrimaria(filtroMapeamento));
                }
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

            var ligacaoJoin = (estruturaEntidadeMapeadaRelacao.EstruturaRelacao.IsRequerido) ? " INNER JOIN " : " LEFT OUTER JOIN ";

            sb.AppendFormat(" {0} \t  {1}  ", System.Environment.NewLine, ligacaoJoin);

            if (estruturaEntidadeMapeadaRelacao.EstruturasEntidadeMapeadaBase.Count > 0)
            {
                sb.AppendLine(" ( ");
            }
            sb.AppendLineFormat("  {0} As {1}  ", estruturaEntidadeMapeadaRelacao.CaminhoBanco, estruturaEntidadeMapeadaRelacao.Apelido);

            if (estruturaEntidadeMapeadaRelacao.EstruturasEntidadeMapeadaBase.Count > 0)
            {
                sb.AppendFormat(" {0} \t  INNER JOIN  ", System.Environment.NewLine);

                sb.AppendLineFormat(estruturaEntidadeMapeadaRelacao.RetornarSqlInnerJoinTabelasEntidadeBase());
            }

            if (estruturaEntidadeMapeadaRelacao.EstruturasEntidadeMapeadaBase.Count > 0)
            {
                sb.AppendLine(" ) ");
            }

            sb.AppendLineFormat(" ON {0} = {1} ", estruturaEntidadeMapeadaRelacao.EstruturaCampoApelidoChavePrimaria.CaminhoBanco, estruturaEntidadeMapeadaRelacao.EstruturaCampoChaveEstrangeiraMapeado.CaminhoBanco);

            if (estruturaEntidadeMapeadaRelacao.EstruturasEntidadeRelacaoMapeadaInterna.Count > 0)
            {
                sb.AppendLine("");

                sb.AppendLine(this.RetornarSqlsJoinRelacao(estruturaEntidadeMapeadaRelacao));

                sb.AppendLine("");
            }
            return sb.ToString();
        }

        private string RetornarSqlOrdenacao()
        {
            var ordenacoes = new List<string>();
            foreach (var ordenacao in this.EstruturaConsulta.Ordenacoes.Values)
            {
                var estruturaCampoApelido = this.TodasEstruturaCampoApelidoMapeado[ordenacao.CaminhoPropriedade];
                var sqlOrdenacao = estruturaCampoApelido.CaminhoBanco;

                if (ordenacao.SentidoOrdenacaoEnum == EnumSentidoOrdenacao.Decrescente)
                {
                    sqlOrdenacao += " DESC ";
                }
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
                var sqlOrdenacao = estruturaCampoApelidoChavePrimaria.Apelido;
                return " ORDER BY " + estruturaCampoApelidoChavePrimaria.Apelido;
            }
        }

        private string RetornarSqlLimite()
        {
            var skip = this.RetornarSkip();
            if (this.Contexto.SqlSuporte.IsOffsetFetch)
            {
                string sqlLimite = "";
                sqlLimite += " OFFSET  " + skip;

                if (ConfiguracaoAcessoDados.TipoBancoDadosEnum == EnumTipoBancoDados.SQL_SERVER)
                {
                    sqlLimite += " ROWS ";
                }
                if (this.EstruturaConsulta.Take > 0)
                {
                    sqlLimite += String.Format("  FETCH NEXT {0} ROWS ONLY  ", this.EstruturaConsulta.Take);
                }
                return sqlLimite;
            }

            if (skip > 0)
            {
                throw new Exception("O banco da deados não tem suporte ao OFFSET FETCH");
            }
            return String.Empty;

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
    }
}