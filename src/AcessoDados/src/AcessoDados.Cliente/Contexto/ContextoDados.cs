using Snebur.AcessoDados.Cliente;
using Snebur.Comunicacao;
using Snebur.Dominio;
using Snebur.Utilidade;
using System.Collections;

namespace Snebur.AcessoDados;

public abstract class BaseContextoDados : __BaseContextoDados, IBaseServico, IServicoDados
{
    protected abstract BaseServicoDadosCliente RetornarServicoDadosCliente();

    private BaseServicoDadosCliente ServicoDados { get; }

    public BaseContextoDados() : base()
    {
        this.ServicoDados = this.RetornarServicoDadosCliente();
    }

    #region IServicoDados

    public override object RetornarValorScalar(EstruturaConsulta estruturaConsulta)
    {
        return this.ServicoDados.RetornarValorScalar(estruturaConsulta);
    }

    public override ResultadoConsulta RetornarResultadoConsulta(EstruturaConsulta estruturaConsulta)
    {
        var resultado = this.ServicoDados.RetornarResultadoConsulta(estruturaConsulta);
        var entidades = resultado.Entidades.Cast<Entidade>().ToList();
        this.NormalizarEntidadesConsulta(entidades);
        return resultado;
    }

    public ResultadoSalvar Salvar(params Entidade[] entidades)
    {
        var lista = new List<Entidade>();
        lista.AddRange(entidades);
        return this.Salvar(lista);
    }

    public ResultadoSalvar Salvar(ListaEntidades<Entidade> entidades)
    {
        var lista = new List<Entidade>();
        lista.AddRange(entidades);
        return this.Salvar(lista);
    }

    public ResultadoDeletar Deletar(params Entidade[] entidades)
    {
        var lista = new List<Entidade>();
        lista.AddRange(entidades);
        return this.Deletar(lista, String.Empty);
    }

    public ResultadoDeletar Deletar(ListaEntidades<Entidade> entidades)
    {
        var lista = new List<Entidade>();
        lista.AddRange(entidades);
        return this.Deletar(lista, String.Empty);
    }

    public override ResultadoSalvar Salvar(IEntidade entidade)
    {
        return this.Salvar(new List<IEntidade> { entidade });
    }

    public override ResultadoSalvar Salvar(IEnumerable<IEntidade> entidades)
    {
        var resultadoSalvar = this.ServicoDados.Salvar(entidades);
        if (!resultadoSalvar.IsSucesso)
        {
            var descricaoEntidades = String.Join(",", entidades.Select(x => $"{x.GetType().Name}({x.Id})"));
            throw new Erro($"Não foi possível salvar as entidade {descricaoEntidades}\r\n{resultadoSalvar.MensagemErro}");
        }
        this.NormailizarEntidadeSalvar(resultadoSalvar, entidades);
        return resultadoSalvar;
    }

    public override ResultadoDeletar Deletar(IEntidade entidade)
    {
        return this.Deletar(new List<IEntidade> { entidade });
    }

    public override ResultadoDeletar Deletar(IEnumerable<IEntidade> entidades)
    {
        return this.Deletar(entidades, String.Empty);
    }

    public override ResultadoDeletar Deletar(IEntidade entidade, string relacoesEmCascata)
    {
        return this.Deletar(new List<IEntidade> { entidade }, String.Empty);
    }

    public override ResultadoDeletar Deletar(IEnumerable<IEntidade> entidades, string relacoesEmCascata)
    {
        return this.ServicoDados.Deletar(entidades, relacoesEmCascata);
    }

    #endregion

    #region  Normalizar entidades Salvar

    private void NormailizarEntidadeSalvar(ResultadoSalvar resultadoSalvar, IEnumerable<IEntidade> entidades)
    {
        var todasEntidades = this.RetornarTodasEntidadesSalvas(entidades);
        foreach (var entidadeSalva in resultadoSalvar.EntidadesSalvas)
        {
            if (todasEntidades.ContainsKey(entidadeSalva.IdentificadorUnicoEntidade))
            {
                var entidade = todasEntidades[entidadeSalva.IdentificadorUnicoEntidade];
                if (entidade.Id == 0)
                {
                    entidade.Id = entidadeSalva.Id;
                    foreach (var propriedadeComputada in entidadeSalva.PropriedadesComputada)
                    {
                        var propriedade = entidade.GetType().GetProperty(propriedadeComputada.NomePropriedade);
                        Guard.NotNull(propriedade);
                        var valorConvertido = ConverterUtil.Para(
                            propriedadeComputada.Valor,
                            propriedade.PropertyType);

                        propriedade.SetValue(entidade, valorConvertido);
                    }
                    entidade.AtivarControladorPropriedadeAlterada();
                }
            }
        }
    }

    private void NormalizarEntidadesSalvar(
        IEntidade entidade,
        Dictionary<Guid, IEntidade> entidadesEncontradas)
    {
        if (entidade == null) return;
        if (entidadesEncontradas.ContainsKey(entidade.RetornarIdentificadorReferencia()))
        {
            return;
        }
        else
        {
            entidadesEncontradas.Add(entidade.RetornarIdentificadorReferencia(), entidade);
        }
        var propriedades = ReflexaoUtil.RetornarPropriedades(entidade.GetType());
        foreach (var propriedade in propriedades)
        {
            if (ReflexaoUtil.IsTipoRetornaColecaoEntidade(propriedade.PropertyType))
            {
                var colecao = propriedade.GetValue(entidade) as IEnumerable;
                if (colecao is not null)
                {
                    foreach (var itemEntidade in colecao)
                    {
                        if (!(itemEntidade is Entidade typedItem))
                        {
                            throw new Erro("Tipo de entidade inválido na coleção.");
                        }
                        this.NormalizarEntidadesSalvar(typedItem, entidadesEncontradas);
                    }
                }
            }
            if (ReflexaoUtil.IsTipoEntidade(propriedade.PropertyType))
            {
                var entidadeRelacao = propriedade.GetValue(entidade) as Entidade;
                Guard.NotNull(entidadeRelacao);
                this.NormalizarEntidadesSalvar(entidadeRelacao, entidadesEncontradas);
            }
        }
    }

    private Dictionary<Guid, IEntidade> RetornarTodasEntidadesSalvas(IEnumerable<IEntidade> entidades)
    {
        var todasEntidade = new Dictionary<Guid, IEntidade>();
        foreach (var entidade in entidades)
        {
            this.NormalizarEntidadesSalvar(entidade, todasEntidade);
        }
        return todasEntidade;
    }

    #endregion

    #region Normalizar consulta

    private void NormalizarEntidadesConsulta(List<Entidade> entidades)
    {
        var todasEntidades = this.RetornarTodasEntidadesConsulta(entidades);
        foreach (var entidade in todasEntidades.Values)
        {
            entidade.AtivarControladorPropriedadeAlterada();
        }
    }

    private Dictionary<Guid, Entidade> RetornarTodasEntidadesConsulta(List<Entidade> entidades)
    {
        var todasEntidade = new Dictionary<Guid, Entidade>();
        foreach (var entidade in entidades)
        {
            this.NormalizarEntidadesConsulta(entidade, todasEntidade);
        }
        return todasEntidade;
    }

    private void NormalizarEntidadesConsulta(Entidade entidade, Dictionary<Guid, Entidade> entidadesEncontradas)
    {
        if (entidade is null)
            return;
        if (entidadesEncontradas.ContainsKey(entidade.RetornarIdentificadorReferencia()))
        {
            return;
        }
        else
        {
            entidadesEncontradas.Add(entidade.RetornarIdentificadorReferencia(), entidade);
        }
        var propriedades = ReflexaoUtil.RetornarPropriedades(entidade.GetType());
        foreach (var propriedade in propriedades)
        {
            if (ReflexaoUtil.IsTipoRetornaColecaoEntidade(propriedade.PropertyType))
            {
                var colecao = propriedade.GetValue(entidade) as IEnumerable;
                if (colecao is not null)
                {
                    foreach (var itemEntidade in colecao)
                    {
                        if(!(itemEntidade is Entidade typedItem))
                        {
                            throw new Erro("Tipo de entidade inválido na coleção.");
                        }

                        this.NormalizarEntidadesConsulta(typedItem, entidadesEncontradas);
                    }
                }
            }
            if (ReflexaoUtil.IsTipoEntidade(propriedade.PropertyType))
            {
                var entidadeRelacao = propriedade.GetValue(entidade) as Entidade;
                Guard.NotNull(entidadeRelacao); 
                this.NormalizarEntidadesConsulta(entidadeRelacao, entidadesEncontradas);
            }
        }
    }

    #endregion

    #region IBaseServico

    public override bool Ping()
    {
        return this.ServicoDados.Ping();
    }

    public override DateTime RetornarDataHora()
    {
        return this.ServicoDados.RetornarDataHora();
    }

    public override DateTime RetornarDataHoraUTC()
    {
        return this.ServicoDados.RetornarDataHoraUTC();
    }
    #endregion

}

public abstract class BaseContextoDados<T> : BaseContextoDados where T : __BaseContextoDados, new()
{
    public static T Instancia => LazyUtil.RetornarValorLazyComBloqueio(ref field, () => new T());
}