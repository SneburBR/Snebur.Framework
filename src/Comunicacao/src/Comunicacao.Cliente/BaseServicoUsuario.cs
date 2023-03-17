using Snebur.AcessoDados;
using Snebur.Dominio;
using Snebur.Seguranca;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Snebur.Comunicacao.Cliente
{
    public abstract class BaseServicoUsuarioCliente : BaseComunicacaoCliente, IServicoUsuario, IServicoUsuarioAsync
    {
        public BaseServicoUsuarioCliente(string urlServico) : base(urlServico)
        {

        }

        #region IServicoUsuario

        public bool IsExisteInformacaoIp()
        {
            var parametros = new object[] { };
            return this.ChamarServico<bool>(MethodBase.GetCurrentMethod(), parametros);
        }

        public void AtualizarInformacaoIp(DadosIPInformacao ipInformacao)
        {
            var parametros = new object[] { ipInformacao };
            this.ChamarServico<bool>(MethodBase.GetCurrentMethod(), parametros);
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
        #endregion

        #region IServicoUsuarioAsync
        public Task<ResultadoExisteIdentificadoUsuario> ExisteIdentificadorUsuarioAsync(string identificadorUsuario)
        {
            return Task.Factory.StartNew(() => this.ExisteIdentificadorUsuario(identificadorUsuario));
        }

        public Task<EnumResultadoValidacaoCredencial> ValidarCredencialAsync(CredencialUsuario credencial)
        {
            return Task.Factory.StartNew(() => this.ValidarCredencial(credencial));
        }

        public Task<bool> SessaoUsuarioAtivaAsync(CredencialUsuario credencial, Guid identificadorSessaoUsuario)
        {
            return Task.Factory.StartNew(() => this.SessaoUsuarioAtiva(credencial, identificadorSessaoUsuario));
        }

        public Task<IUsuario> RetornarUsuarioAsync(CredencialUsuario credencial)
        {
            return Task.Factory.StartNew(() => this.RetornarUsuario(credencial));
        }

        public Task<ResultadoAutenticacao> AutenticarAsync(CredencialUsuario credencial)
        {
            return Task.Factory.StartNew(() => this.Autenticar(credencial));
        }

        public Task<ISessaoUsuario> RetornarSessaoUsuarioAsync(Guid identificadorSessaoUsuario)
        {
            return Task.Factory.StartNew(() => this.RetornarSessaoUsuario(identificadorSessaoUsuario));
        }

        public Task<IUsuario> CadastrarNovoUsuarioAsync(NovoUsuario novoUsuario, bool isAlterarSenhaProximoAcesso)
        {
            return Task.Factory.StartNew(() => this.CadastrarNovoUsuario(novoUsuario, isAlterarSenhaProximoAcesso));
        }

        public Task<ResultadoEnviarCodigoRecuperarSenha> EnviarCodigoRecuperarSenhaAsync(string identificadorAmigavel)
        {
            return Task.Factory.StartNew(() => this.EnviarCodigoRecuperarSenha(identificadorAmigavel));
        }

        public Task<ResultadoValidarCodigoRecuperarSenha> ValidarCodigRecuperarSenhaAsync(string identificadorAmigavel, string codigoRecuperarSenha)
        {
            return Task.Factory.StartNew(() => this.ValidarCodigRecuperarSenha(identificadorAmigavel, codigoRecuperarSenha));
        }

        public Task<ResultadoRecuperarSenha> RecuperarSenhaAsync(string identificadorAmigavel, string codigoRecuperarSenha, string novaSenha)
        {
            return Task.Factory.StartNew(() => this.RecuperarSenha(identificadorAmigavel, codigoRecuperarSenha, novaSenha));
        }

        public Task<ResultadoAlterarSenha> AlterarSenhaAsync(CredencialUsuario credencial, string novaSenha)
        {
            return Task.Factory.StartNew(() => this.AlterarSenha(credencial, novaSenha));
        }

        public Task FinalizarSessaoUsuarioAsync(Guid identificadorSessaoUsuario)
        {
            return Task.Factory.StartNew(() => this.FinalizarSessaoUsuario(identificadorSessaoUsuario));
        }

        public Task<bool> IsExisteInformacaoIpAsync()
        {
            return Task.Factory.StartNew(() => this.IsExisteInformacaoIp());
        }

        public Task AtualizarInformacaoIpAsync(DadosIPInformacao ipInformacao)
        {
            return Task.Factory.StartNew(() => this.AtualizarInformacaoIp(ipInformacao));
        }



        #endregion
    }
}
