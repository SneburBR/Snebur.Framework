using Snebur.AcessoDados.Servidor.Salvar;
using Snebur.Linq;

namespace Snebur.AcessoDados;

public abstract partial class BaseContextoDados : IContextoDadosSeguranca
{
    #region IContextoDadosSeguranca 

    TiposSeguranca IContextoDadosSeguranca.TiposSeguranca
        => this.TiposSeguranca ??
        throw new InvalidOperationException($"O {nameof(TiposSeguranca)} não foi inicializado");

    ResultadoSalvar IContextoDadosSeguranca.SalvarSeguranca(IEntidade entidade)
    {
        return (this as IContextoDadosSeguranca).SalvarSeguranca(new List<IEntidade>() { entidade });
    }
    ResultadoSalvar IContextoDadosSeguranca.SalvarSeguranca(List<IEntidade> entidades)
    {
        this.ValidarEntidadesSeguranca(entidades);

        var entidadesTipada = entidades.Cast<Entidade>().ToHashSet();
        using (var salvarEntidades = new SalvarEntidades(this, entidadesTipada, EnumOpcaoSalvar.Salvar, false))
        {
            var resultado = salvarEntidades.Salvar(false);
            if (resultado.Erro != null)
            {
                throw resultado.Erro;
            }
            return resultado as ResultadoSalvar
                ?? throw new InvalidCastException($"O tipo de resultado {resultado.GetType().Name} não foi possível converter para {nameof(ResultadoSalvar)}");
        }
    }
    ResultadoDeletar IContextoDadosSeguranca.DeletarSeguranca(IEntidade entidade)
    {
        return (this as IContextoDadosSeguranca).DeletarSeguranca(new List<IEntidade>() { entidade });
    }
    ResultadoDeletar IContextoDadosSeguranca.DeletarSeguranca(List<IEntidade> entidades)
    {
        this.ValidarEntidadesSeguranca(entidades);

        var entidadesTipada = entidades.Cast<Entidade>().ToHashSet();
        using (var salvar = new SalvarEntidades(this, entidadesTipada, EnumOpcaoSalvar.Deletar, false))
        {
            var resultado = salvar.Salvar();
            if (resultado.Erro != null)
            {
                throw resultado.Erro;
            }
            return resultado as ResultadoDeletar ??
                throw new InvalidCastException($"O tipo de resultado {resultado.GetType().Name} não foi possível converter para {nameof(ResultadoDeletar)}");
        }
    }

    IUsuario? IContextoDadosSeguranca.RetornarUsuarioAnonimo()
    {
        Guard.NotNull(this.CacheSessaoUsuario);
        return this.CacheSessaoUsuario.UsuarioAnonimo;
    }

    private void ValidarEntidadesSeguranca(List<IEntidade> entidades)
    {
        var nomesTipoEntidades = entidades.Select(x => x.GetType().Name).Distinct().ToList();
        var estruturasEntdiade = nomesTipoEntidades.Select(x => this.EstruturaBancoDados.EstruturasEntidade[x]).ToList();

        var estruturasSeguranca = estruturasEntdiade.Where(x => x.Schema != TabelaSegurancaAttribute.SCHEMA_SEGURANCA).Distinct().ToList();
        var tipoUsuario = this.EstruturaBancoDados.TiposSeguranca.TipoUsuario;

        Guard.NotNull(tipoUsuario);

        estruturasSeguranca = estruturasSeguranca
            .Where(x => !x.TipoEntidade.IsSubclassOf(tipoUsuario))
            .ToList();

        if (estruturasSeguranca.Count > 0)
        {
            var mensagem = String.Format("O tipos {0} não pertencem ao Schema {1} ", String.Join(", ", estruturasSeguranca.Select(x => x.TipoEntidade.Name)), TabelaSegurancaAttribute.SCHEMA_SEGURANCA);
            throw new Erro(mensagem);
        }
    }

    bool IContextoDadosSeguranca.IsContextoInicializado
        => this.IsContextoInicializado;

    #endregion
}