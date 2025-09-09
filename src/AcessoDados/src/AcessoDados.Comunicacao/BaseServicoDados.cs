namespace Snebur.AcessoDados.Comunicacao;

public abstract class BaseServicoContextoDados<TContextoDados> : BaseServicoComunicacaoDados<TContextoDados>, IServicoDados where TContextoDados : BaseContextoDados
{

    public BaseServicoContextoDados()
    {

    }

    #region IServicoDados

    public ResultadoConsulta RetornarResultadoConsulta(EstruturaConsulta estruturaConsulta)
    {
        Guard.NotNull(this.ContextoDados);
        return this.ContextoDados.RetornarResultadoConsulta(estruturaConsulta);

    }

    public object? RetornarValorScalar(EstruturaConsulta estruturaConsulta)
    {
        Guard.NotNull(this.ContextoDados);
        return this.ContextoDados.RetornarValorScalar(estruturaConsulta);
    }

    public ResultadoSalvar Salvar(IEnumerable<IEntidade> entidades)
    {
        Guard.NotNull(this.ContextoDados);
        return this.ContextoDados.Salvar(entidades, true);
    }

    public ResultadoDeletar Deletar(IEnumerable<IEntidade> entidades, string relacoesEmCascata)
    {
        Guard.NotNull(this.ContextoDados);
        return this.ContextoDados.Deletar(entidades, relacoesEmCascata);
    }

    #endregion

}