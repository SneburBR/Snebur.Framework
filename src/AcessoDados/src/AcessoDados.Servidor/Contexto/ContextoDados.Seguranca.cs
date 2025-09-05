using Snebur.AcessoDados.Servidor.Salvar;
using Snebur.Linq;

namespace Snebur.AcessoDados
{
    public abstract partial class BaseContextoDados : IContextoDadosSeguranca
    {
        #region IContextoDadosSeguranca 

        TiposSeguranca IContextoDadosSeguranca.TiposSeguranca { get => this.TiposSeguranca; }

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
                return resultado as ResultadoSalvar;
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
                return resultado as ResultadoDeletar;
            }
        }

        IUsuario IContextoDadosSeguranca.RetornarUsuarioAnonimo()
        {
            return this.CacheSessaoUsuario.UsuarioAnonimo;
        }

        private void ValidarEntidadesSeguranca(List<IEntidade> entidades)
        {
            var nomesTipoEntidades = entidades.Select(x => x.GetType().Name).Distinct().ToList();
            var estruturasEntdiade = nomesTipoEntidades.Select(x => this.EstruturaBancoDados.EstruturasEntidade[x]).ToList();
            var estruturasSeguranca = estruturasEntdiade.Where(x => x.Schema != TabelaSegurancaAttribute.SCHEMA_SEGURANCA).Distinct().ToList();
            estruturasSeguranca = estruturasSeguranca.Where(x => !x.TipoEntidade.IsSubclassOf(this.EstruturaBancoDados.TiposSeguranca.TipoUsuario)).ToList();

            if (estruturasSeguranca.Count > 0)
            {
                var mensagem = String.Format("O tipos {0} não pertencem ao Schema {1} ", String.Join(", ", estruturasSeguranca.Select(x => x.TipoEntidade.Name)), TabelaSegurancaAttribute.SCHEMA_SEGURANCA);
                throw new Erro(mensagem);
            }
        }

        bool IContextoDadosSeguranca.IsContextoInicializado { get => this.IsContextoInicializado; }

        #endregion
    }
}