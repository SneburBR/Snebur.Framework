using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados.Mapeamento;

internal class BaseMapeamentoConsulta : IDisposable
{
    #region Propriedades 
    internal int _contadorApelido;
    internal EstruturaBancoDados EstruturaBancoDados { get; }

    internal BaseConexao ConexaoDB { get; }

    internal Dictionary<string, BaseEstruturaApelido> EstruturasApelido { get; } = new();

    internal Type TipoEntidade { get; }

    internal EstruturaEntidade EstruturaEntidade { get; }

    internal EstruturaConsulta EstruturaConsulta { get; }

    internal BaseContextoDados Contexto { get; }

    #endregion

    #region Construtor

    internal BaseMapeamentoConsulta(EstruturaConsulta consulta,
                                    EstruturaBancoDados estruturaBancoDados,
                                    BaseConexao ConexaoDB,
                                    BaseContextoDados contexto)
    {
        var tipoEntidade = consulta.RetornarTipoEntidade();
        this.EstruturaConsulta = consulta;
        this.EstruturaBancoDados = estruturaBancoDados;
        this.ConexaoDB = ConexaoDB;
        this.Contexto = contexto;
        this.EstruturaEntidade = estruturaBancoDados.EstruturasEntidade[tipoEntidade.Name];
        this.TipoEntidade = tipoEntidade;
    }
    #endregion

    internal int RetornarContadorApelido()
    {
        this._contadorApelido += 1;
        return this._contadorApelido;
    }
    #region IDisposable

    public virtual void Dispose()
    {
    }
    #endregion
}