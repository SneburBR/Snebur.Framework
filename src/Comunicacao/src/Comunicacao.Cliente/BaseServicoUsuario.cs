using System;
using System.Reflection;
using Snebur.AcessoDados;
using Snebur.Dominio;
using Snebur.Seguranca;

namespace Snebur.Comunicacao.Cliente
{
    public abstract class BaseServicoUsuarioCliente : BaseComunicacaoCliente, IServicoUsuario
    {
        public BaseServicoUsuarioCliente(string urlServico) : base(urlServico)
        {

        }

        public ResultadoAlterarSenha AlterarSenha(CredencialUsuario credencial, string novaSenha)
        {
            var parametros = new object[] { novaSenha };
            return this.ChamarServico<ResultadoAlterarSenha>(MethodBase.GetCurrentMethod(), parametros);
        }

        public ResultadoAutenticacao Autenticar(CredencialUsuario credencial)
        {
            var parametros = new object[] { credencial };
            return this.ChamarServico<ResultadoAutenticacao>(MethodBase.GetCurrentMethod(), parametros);
        }

        public IUsuario CadastrarNovoUsuario(NovoUsuario novoUsuario, bool isAlterarSenhaProximoAcesso)
        {
            var parametros = new object[] { isAlterarSenhaProximoAcesso };
            return this.ChamarServico<IUsuario>(MethodBase.GetCurrentMethod(), parametros);
        }

        public ResultadoEnviarCodigoRecuperarSenha EnviarCodigoRecuperarSenha(string identificadorAmigavel)
        {
            var parametros = new object[] { identificadorAmigavel };
            return this.ChamarServico<ResultadoEnviarCodigoRecuperarSenha>(MethodBase.GetCurrentMethod(), parametros);
        }

        public ResultadoExisteIdentificadoUsuario ExisteIdentificadorUsuario(string identificadorUsuario)
        {
            var parametros = new object[] { identificadorUsuario };
            return this.ChamarServico<ResultadoExisteIdentificadoUsuario>(MethodBase.GetCurrentMethod(), parametros);
        }

        public void FinalizarSessaoUsuario(Guid identificadorSessaoUsuario)
        {
            var parametros = new object[] { identificadorSessaoUsuario };
            this.ChamarServico<bool>(MethodBase.GetCurrentMethod(), parametros);
        }

        public ResultadoRecuperarSenha RecuperarSenha(string identificadorAmigavel, string codigoRecuperarSenha, string novaSenha)
        {
            var parametros = new object[] { identificadorAmigavel, codigoRecuperarSenha, novaSenha };
           return this.ChamarServico<ResultadoRecuperarSenha>(MethodBase.GetCurrentMethod(), parametros);
        }

        public ISessaoUsuario RetornarSessaoUsuario(Guid identificadorSessaoUsuario)
        {
            var parametros = new object[] { identificadorSessaoUsuario };
            return this.ChamarServico<ISessaoUsuario>(MethodBase.GetCurrentMethod(), parametros);
        }

        public IUsuario RetornarUsuario(CredencialUsuario credencial)
        {
            var parametros = new object[] { credencial };
            return this.ChamarServico<IUsuario>(MethodBase.GetCurrentMethod(), parametros);
        }

        public bool SessaoUsuarioAtiva(CredencialUsuario credencial, Guid identificadorSessaoUsuario)
        {
            try
            {
                var parametros = new object[] { credencial, identificadorSessaoUsuario };
                return this.ChamarServico<bool>(MethodBase.GetCurrentMethod(), parametros);
            }
            catch (ErroSessaoUsuarioExpirada)
            {
                return false;
            }
        }

        public ResultadoValidarCodigoRecuperarSenha ValidarCodigRecuperarSenha(string identificadorAmigavel, string codigoRecuperarSenha)
        {
            var parametros = new object[] { identificadorAmigavel, codigoRecuperarSenha };
            return this.ChamarServico<ResultadoValidarCodigoRecuperarSenha>(MethodBase.GetCurrentMethod(), parametros);
        }

        public EnumResultadoValidacaoCredencial ValidarCredencial(CredencialUsuario credencial)
        {
            var parametros = new object[] { credencial };
            return this.ChamarServico<EnumResultadoValidacaoCredencial>(MethodBase.GetCurrentMethod(), parametros);
        }
    }
}
