using Snebur.AcessoDados.Estrutura;

namespace Snebur.AcessoDados
{
    public class TiposSeguranca
    {

        public Type? TipoIdentificacao { get; }

        public Type? TipoUsuario { get; }

        public Type? TipoSessaoUsuario { get; }

        public Type? TipoIpInformacao { get; }

        public Type? TipoGrupoUsuario { get; }

        public Type? TipoPermissaoEntidade { get; }

        public Type? TipoPermissaoCampo { get; }

        public Type? TipoRegraOperacao { get; }

        public Type? TipoUsuarioAdicionarGrupo { get; }

        public Type? TipoLogAlteracao { get; }

        public Type? TipoRelacaoIdentificacaoGrupoUsuario { get; set; }

        public Type? TipoRelacaoTipoUsuarioAdicionarGrupoUsuarioGrupoUsuario { get; }

        public Type? TipoRestricaoEntidade { get; }

        public bool AtivarSeguranca { get; }

        internal TiposSeguranca(EstruturaBancoDados estrutura)
        {
            this.TipoIdentificacao = estrutura.RetornarTipoConsultaImplementaInterface<IIdentificacao>(true);
            this.TipoUsuario = estrutura.RetornarTipoConsultaImplementaInterface<IUsuario>(true);
            this.TipoSessaoUsuario = estrutura.RetornarTipoConsultaImplementaInterface<ISessaoUsuario>(true);
            this.TipoIpInformacao = estrutura.RetornarTipoConsultaImplementaInterface<IIPInformacaoEntidade>(true);
            this.TipoGrupoUsuario = estrutura.RetornarTipoConsultaImplementaInterface<IGrupoUsuario>(true);
            this.TipoPermissaoEntidade = estrutura.RetornarTipoConsultaImplementaInterface<IPermissaoEntidade>(true);
            this.TipoPermissaoCampo = estrutura.RetornarTipoConsultaImplementaInterface<IPermissaoCampo>(true);
            this.TipoRegraOperacao = estrutura.RetornarTipoConsultaImplementaInterface<IRegraOperacao>(true);
            this.TipoUsuarioAdicionarGrupo = estrutura.RetornarTipoConsultaImplementaInterface<ITipoUsuarioAdicionarGrupoUsuario>(true);
            this.TipoLogAlteracao = estrutura.RetornarTipoConsultaImplementaInterface<ILogAlteracao>(true);
            this.TipoRelacaoIdentificacaoGrupoUsuario = estrutura.RetornarTipoConsultaImplementaInterface<IRelacaoIdentificacaoGrupoUsuario>(true);
            this.TipoRelacaoTipoUsuarioAdicionarGrupoUsuarioGrupoUsuario = estrutura.RetornarTipoConsultaImplementaInterface<IRelacaoTipoUsuarioAdicionarGrupoUsuarioGrupoUsuario>(true);
            this.TipoRestricaoEntidade = estrutura.RetornarTipoConsultaImplementaInterface<IRestricaoEntidade>(true);
            this.AtivarSeguranca = this.RetornarAtivaSeguranca();
        }

        /// <summary>
        /// Verificando se todos os tipos não estão nulos
        /// </summary>
        /// <returns></returns>
        private bool RetornarAtivaSeguranca()
        {
            var propriedadesTipos = this.GetType().GetProperties().Where(x => x.PropertyType == typeof(Type));
            var valoresPropriedade = propriedadesTipos.Select(x => (Type)x.GetValue(this)).ToList();
            return valoresPropriedade.All(x => x != null);
        }
    }
}