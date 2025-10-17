using Snebur.AcessoDados.Estrutura;
using System.Collections.Generic;

namespace Snebur.AcessoDados.Mapeamento;

internal abstract partial class BaseMapeamentoEntidade : IDisposable
{
    #region Propriedades

    private readonly HashSet<ParametroInfo> _parametroInfos = new();

    internal IReadOnlyCollection<ParametroInfo> ParametrosInfo
        => this._parametroInfos;

    internal BaseMapeamentoConsulta MapeamentoConsulta { get; }

    internal EstruturaConsulta EstruturaConsulta { get; }

    internal EstruturaEntidade EstruturaEntidade { get; }

    internal EstruturaBancoDados EstruturaBancoDados { get; }

    internal BaseConexao ConexaoDB { get; }

    internal BaseContextoDados Contexto { get; }

    internal Type TipoEntidade { get; }

    internal Dictionary<string, string?> RelacoesAberta { get; } = new Dictionary<string, string?>();
    internal DicionarioEstrutura<EstruturaCampoApelido> TodasEstruturaCampoApelidoMapeado { get; } = new DicionarioEstrutura<EstruturaCampoApelido>();
    internal HashSet<EstruturaCampoApelido> EstruturasCampoApelidoChaveEstrangeiraRelacoesAberta { get; } = new HashSet<EstruturaCampoApelido>();
    internal EstruturaCampoApelido EstruturaCampoApelidoChavePrimaria { get; private set; }
    public EstruturaEntidadeApelido EstruturaEntidadeApelido { get; }

    //public Dictionary<string, EstruturaEntidadeMapeadaRelacao> EstruturasEntidadeMapeadaRelacao { get; set; }

    #endregion

    internal BaseMapeamentoEntidade(
        BaseMapeamentoConsulta mapeamentoConsulta,
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

        //this.EstruturasEntidadeMapeadaRelacao = new Dictionary<string, EstruturaEntidadeMapeadaRelacao>();
        //var (estruturaEntidadeApelido, estruturaCampoApelidoChavePrimaria) = ;
        //this.EstruturaEntidadeApelido = this.RetornarEstruturaEntidadeApelido(this.EstruturaEntidade, this.TipoEntidade.Name, String.Empty);

        this.EstruturaEntidadeApelido = this.RetornarEstruturaEntidadeApelido(
            estruturaEntidade,
            this.TipoEntidade.Name,
            String.Empty);

        Guard.NotNull(this.EstruturaCampoApelidoChavePrimaria);
        //this.EstruturaCampoApelidoChavePrimaria = estruturaCampoApelidoChavePrimaria;

        this.MontarEstruturasRelacoesAbertasFiltro();
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
            Guard.NotNullOrWhiteSpace(relacaoAbertaEntidade.CaminhoPropriedade);

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

    #region Métodos internos

    internal string RetornarSql(BaseFiltroMapeamento filtroMapeamento,
                                 bool isIncluirOrdenacaoPaginacao)
    {
        var sqlCampos = this.RetornarSqlCampos();
        return this.MontarSql(filtroMapeamento,
                              sqlCampos,
                              isIncluirOrdenacaoPaginacao);
    }

    internal protected abstract string RetornarSqlCampos();

    internal void AdicionarParametroInfo(ParametroInfo parametroInfo)
    {
        if (this._parametroInfos.TryGetValue(parametroInfo, out var current))
        {
            if(current.EqualsStrict(parametroInfo))
                return;

            throw new ErroAcessoDadosGenerico(
                $"Parâmetro {parametroInfo.ParameterName} duplicado com valores diferentes");
        }
        this._parametroInfos.Add(parametroInfo);
         
    }

    #endregion
 
    #region IDisposable

    public void Dispose()
    {
        this.TodasEstruturaCampoApelidoMapeado?.Clear();
        this.EstruturasCampoApelidoChaveEstrangeiraRelacoesAberta?.Clear();
        this.RelacoesAberta?.Clear();

    }
    #endregion
}