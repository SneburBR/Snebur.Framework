using Snebur.Servicos;
using System.Text;

namespace Snebur.AcessoDados.Seguranca
{
    internal class LogSegurancaUtil
    {
        #region Leitura

        internal static void LogAlteracaoEntidade(
            IUsuario usuario,
            IUsuario? usuarioAvalista,
            List<AutorizacaoEntidade> autorizacoes)
        {
            if (autorizacoes.Count > 0)
            {
                throw new NotImplementedException();
            }
        }

        internal static void LogAvalistaRequerido(
            IUsuario usuario,
            IUsuario? usuarioAvalista,
            List<AutorizacaoEntidade> autorizacoes)
        {
            if (autorizacoes.Count > 0)
            {
                throw new NotImplementedException();
            }
        }

        internal static void LogSeguranca(
            IUsuario usuario,
            IUsuario? usuarioAvalista,
            List<AutorizacaoEntidade> autorizacoes)
        {
            LogSegurancaUtil.SalvarLogSegurancaAsync(usuario, usuarioAvalista, autorizacoes);
        }

        internal static void LogPermissaoNegada(
            IUsuario usuario,
            IUsuario? usuarioAvalista,
            List<AutorizacaoEntidade> autorizacoes)
        {
            LogSegurancaUtil.SalvarLogSegurancaAsync(usuario, usuarioAvalista, autorizacoes);
        }
        #endregion
        private static void SalvarLogSegurancaAsync(
            IUsuario usuario,
            IUsuario? usuarioAvalista,
            List<AutorizacaoEntidade> autorizacoes)
        {
            ThreadUtil.ExecutarAsync(() =>
            {
                LogSegurancaUtil.SalvarLogSeguranca(usuario, usuarioAvalista, autorizacoes);

            }, true);
        }

        private static void SalvarLogSeguranca(
            IUsuario usuario,
            IUsuario? usuarioAvalista,
            List<AutorizacaoEntidade> autorizacoes)
        {

            var sb = new StringBuilder();
            sb.Append($"Usuário: {usuario.IdentificadorUsuario},");
            if (usuarioAvalista is not null)
            {
                sb.Append($" Avalista: {usuarioAvalista.IdentificadorUsuario}");
            }

            //sb.AppendLine("");
            foreach (var autorizacao in autorizacoes)
            {
                sb.AppendFormat(" Operação : '{0}',  Entidade : '{1}', Permissao : '{2}'",
                                EnumUtil.RetornarDescricao(autorizacao.Operacao),
                                autorizacao.NomeTipoEntidade,
                                EnumUtil.RetornarDescricao(autorizacao.Permissao));

                if (autorizacao is AutorizacaoEntidadeSalvar autorizacaoSalvar)
                {
                    //sb.AppendLine("");
                    sb.AppendFormat(" Ids  '{0}'", String.Join(",", autorizacaoSalvar.Entidades.Select(x => x.Id)));
                }
            }
            if (DebugUtil.IsAttached)
            {
                if (autorizacoes.Any(x => x.Permissao == EnumPermissao.Negado))
                {
                    throw new Exception(sb.ToString());
                }
            }
            LogUtil.SegurancaAsync(sb.ToString(), EnumTipoLogSeguranca.PermissaoAcessoDados);
        }
    }
}