using Snebur.AcessoDados.Estrutura;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Snebur.AcessoDados.Mapeamento
{
    internal abstract partial class BaseMapeamentoEntidade : IDisposable
    {
        #region Propriedades

        internal List<DbParameter> Parametros { get; } = new List<DbParameter>();

        internal BaseMapeamentoConsulta MapeamentoConsulta { get; set; }

        internal EstruturaConsulta EstruturaConsulta { get; }

        internal EstruturaEntidade EstruturaEntidade { get; }

        internal EstruturaBancoDados EstruturaBancoDados { get; }

        internal BaseConexao ConexaoDB { get; }

        internal BaseContextoDados Contexto { get; }

        internal Type TipoEntidade { get; }

        internal Dictionary<string, string> RelacoesAberta { get; private set; }

        internal EstruturaCampoApelido EstruturaCampoApelidoChavePrimaria { get; private set; }

        internal DicionarioEstrutura<EstruturaCampoApelido> TodasEstruturaCampoApelidoMapeado { get; private set; }

        internal HashSet<EstruturaCampoApelido> EstruturasCampoApelidoChaveEstrangeiraRelacoesAberta { get; private set; } = new HashSet<EstruturaCampoApelido>();

        public EstruturaEntidadeApelido EstruturaEntidadeApelido { get; set; }

        //public Dictionary<string, EstruturaEntidadeMapeadaRelacao> EstruturasEntidadeMapeadaRelacao { get; set; }

        #endregion

        internal BaseMapeamentoEntidade(BaseMapeamentoConsulta mapeamentoConsulta,
                                        EstruturaEntidade estruturaEntidade,
                                        EstruturaBancoDados estruturaBancoDados,
                                        BaseConexao conexaoDB,
                                        BaseContextoDados contexto)
        {
            this.MapeamentoConsulta = mapeamentoConsulta;
            this.EstruturaConsulta = this.MapeamentoConsulta.EstruturaConsulta;
            this.EstruturaEntidade = estruturaEntidade;
            this.EstruturaBancoDados = estruturaBancoDados;
            this.ConexaoDB = conexaoDB;
            this.Contexto = contexto;

            this.TipoEntidade = estruturaEntidade.TipoEntidade;

            this.RelacoesAberta = new Dictionary<string, string>();
            this.TodasEstruturaCampoApelidoMapeado = new DicionarioEstrutura<EstruturaCampoApelido>();
            //this.EstruturasEntidadeMapeadaRelacao = new Dictionary<string, EstruturaEntidadeMapeadaRelacao>();

            this.EstruturaEntidadeApelido = this.RetornarEstruturaEntidadeApelido(this.EstruturaEntidade, this.TipoEntidade.Name, String.Empty);
            this.MontarEstruturasRelacoesAbertasFiltro();
        }

        internal string RetornarSql(bool ordenacao, bool isLimitarPaginacao, BaseFiltroMapeamento filtroMapeamento)
        {
            var isRelacaoFilhos = this.MapeamentoConsulta is MapeamentoConsultaRelacaoAbertaFilhos;
            var sqlCampos = this.RetornarSqlCampos();
            var sqlJoin = this.RetornarSqlConsulta(ordenacao, isLimitarPaginacao, filtroMapeamento, isRelacaoFilhos);
            //if ((!isLimitarPaginacao && this.EstruturaConsulta.Take > 0) || !this.Contexto.SqlSuporte.IsOffsetFetch)
            //{
            var take = this.EstruturaEntidade.RetornarMaximoConsulta(this.EstruturaConsulta.Take);
            return $"SELECT Top {take} {sqlCampos} FROM {sqlJoin}";
            //}
            //else
            //{
            //    return $"SELECT  {sqlCampos} FROM {sqlJoin}";
            //}
        }

        internal string RetornarSqlContagem(BaseFiltroMapeamento filtro)
        {
            var isRelacaoFilhos = this.MapeamentoConsulta is MapeamentoConsultaRelacaoAbertaFilhos;
            var sqlJoin = this.RetornarSqlConsulta(false, false, filtro, isRelacaoFilhos);
            return String.Format("SELECT COUNT(*) FROM {0}", sqlJoin);
        }

        #region Mapeamentos das Estruturas

        private void MontarEstruturasRelacoesAbertasFiltro()
        {
            var estruturaEntidade = this.EstruturaEntidade;
            var apelidoEntidade = this.TipoEntidade.Name;

            var relacoesAbertaEntidade = this.EstruturaConsulta.RelacoesAbertaFiltro.Select(x => x.Value).ToList();

            EstruturaEntidadeApelido mapeamentoEntidadeAtual = this.EstruturaEntidadeApelido;

            foreach (var relacaoAbertaEntidade in relacoesAbertaEntidade)
            {
                var estruturaEntidadeAtual = estruturaEntidade;
                var apelidorRelacaoEntidadeAtual = apelidoEntidade;
                var nomesProprieadades = relacaoAbertaEntidade.CaminhoPropriedade.Split('.').ToList();
                var nomesPropriedadesAcessado = new List<string>();

                foreach (var nomePropriedade in nomesProprieadades)
                {
                    var estruturaRelacao = estruturaEntidadeAtual.RetornarEstruturaRelacao(nomePropriedade);

                    // Será vazio caso ja primeiro nivel da Relacao Cliente.Endereco;
                    // Caso Cliente.Endereco.Cidade - o caminho Relacao Endereco
                    var caminhoPropriedadeEntidadePai = String.Join(".", nomesPropriedadesAcessado);

                    nomesPropriedadesAcessado.Add(nomePropriedade);

                    var caminhoRelacaoAberta = String.Join(".", nomesPropriedadesAcessado);

                    switch (estruturaRelacao)
                    {
                        case EstruturaRelacaoChaveEstrangeira estruturaRelacaoChaveEstrangeira:

                            {
                                var estruturaEntidadeRelacaoChaveEstrangeira = estruturaRelacaoChaveEstrangeira.EstruturaEntidadeChaveEstrangeiraDeclarada;
                                var pontoSeparador = String.IsNullOrWhiteSpace(caminhoPropriedadeEntidadePai) ? String.Empty : ".";
                                var caminhoCampoChaveEstrangeira = String.Format("{0}{1}{2}", caminhoPropriedadeEntidadePai, pontoSeparador, estruturaRelacaoChaveEstrangeira.EstruturaCampoChaveEstrangeira.Propriedade.Name);
                                var estruturaCampoChaveEstrangeiraAberta = this.TodasEstruturaCampoApelidoMapeado[caminhoCampoChaveEstrangeira];

                                this.EstruturasCampoApelidoChaveEstrangeiraRelacoesAberta.Add(estruturaCampoChaveEstrangeiraAberta);

                                var apelidoRelacao = String.Format("{0}_{1}", apelidoEntidade, String.Join("_", nomesPropriedadesAcessado));

                                if (!this.RelacoesAberta.ContainsKey(caminhoRelacaoAberta))
                                {
                                    this.RelacoesAberta.Add(caminhoRelacaoAberta, null);
                                    var mapeamentoEntidade = this.RetornarEstruturaEntidadeApelido(estruturaEntidadeRelacaoChaveEstrangeira, apelidoRelacao, caminhoRelacaoAberta, null, estruturaRelacaoChaveEstrangeira, estruturaCampoChaveEstrangeiraAberta);

                                    //if (mapeamentoEntidadeAtual == null)
                                    //{
                                    //    this.EstruturasEntidadeMapeadaRelacao.Add(mapeamentoEntidade.Apelido, (EstruturaEntidadeMapeadaRelacao)mapeamentoEntidade);
                                    //}
                                    //else
                                    //{
                                    //    mapeamentoEntidadeAtual.EstruturasEntidadeRelacaoMapeada.Add(mapeamentoEntidade);
                                    //}

                                    mapeamentoEntidadeAtual.EstruturasEntidadeRelacaoMapeadaInterna.Add(mapeamentoEntidade);

                                    mapeamentoEntidadeAtual = mapeamentoEntidade;
                                }
                                estruturaEntidadeAtual = estruturaEntidadeRelacaoChaveEstrangeira;

                                break;
                            }
                        case EstruturaRelacaoUmUmReversa estruturaRelacaoUmUmReversa:

                            throw new Exception("Não implementado");

                        default:

                            throw new ErroNaoSuportado($"Estrutura relacao não suportada {estruturaRelacao}");
                    }
                }
            }
        }
        #endregion

        #region Métodos privados

        internal protected abstract string RetornarSqlCampos();

        #endregion

        #region IDisposable

        public void Dispose()
        {
            this.TodasEstruturaCampoApelidoMapeado?.Clear();
            this.EstruturasCampoApelidoChaveEstrangeiraRelacoesAberta?.Clear();
            this.RelacoesAberta?.Clear();
            // this.EstruturasEntidadeMapeadaRelacao?.Clear();

            this.TodasEstruturaCampoApelidoMapeado = null;
            this.RelacoesAberta = null;
            this.EstruturasCampoApelidoChaveEstrangeiraRelacoesAberta = null;
            //  this.EstruturasEntidadeMapeadaRelacao = null;
        }
        #endregion
    }
}