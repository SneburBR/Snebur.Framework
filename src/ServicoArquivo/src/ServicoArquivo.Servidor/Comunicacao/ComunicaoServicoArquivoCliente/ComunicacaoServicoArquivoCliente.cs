using Snebur.Comunicacao;
using Snebur.Dominio;
using Snebur.Seguranca;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;

#if NET50
using Microsoft.AspNetCore.Http;
#endif

namespace Snebur.ServicoArquivo
{
    public delegate string FuncaoNormalizadorOrigem(string origem);

    public class ComunicacaoServicoArquivoCliente : BaseComunicacaoCliente, IComunicacaoServicoArquivo
    {

        private Guid IdentificadorSessaoUsuario { get; set; }
        public FuncaoNormalizadorOrigem FuncaoNormalizadorOrigem { get; }

        private CredencialUsuario CredencialUsuarioRequisicao { get; }

        protected override Dictionary<string, string> ParametrosCabecalhoAdicionais
        {
            get
            {
                var parametros = new Dictionary<string, string>();
                var request = AplicacaoSnebur.Atual.HttpContext?.Request;
                if (request != null)
                {
                    var host = request.RetornarUrlRequisicao().Host.ToLower();
                    //var host = request.GetTypedHeaders().Referer.Host.ToLower();
                    var identificadorPropriedade = request.Headers[ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO];
                    if (!String.IsNullOrEmpty(host))
                    {
                        parametros.Add(ConstantesCabecalho.ORIGIN, host);
                    }
                    if (!String.IsNullOrEmpty(identificadorPropriedade))
                    {
                        parametros.Add(ConstantesCabecalho.IDENTIFICADOR_PROPRIETARIO, identificadorPropriedade);
                    }
                }
                return parametros;
            }
        }

        public ComunicacaoServicoArquivoCliente(string urlServico,
                                               CredencialUsuario credencialUsuarioRequisicao,
                                               Guid identificadorSessaoUsuario,
                                               FuncaoNormalizadorOrigem funcaoNormalizadorOrigem) : base(urlServico)
        {
            this.CredencialUsuarioRequisicao = credencialUsuarioRequisicao;
            this.IdentificadorSessaoUsuario = identificadorSessaoUsuario;
            this.FuncaoNormalizadorOrigem = funcaoNormalizadorOrigem;
        }

        protected override string RetornarNomeManipulador()
        {
            return base.RetornarNomeManipulador();
        }

        #region  IComunicacaoServicoArquivo

        public bool ExisteIdArquivo(long idArquivo)
        {
            object[] parametros = { idArquivo };
            return this.ChamarServico<bool>(MethodBase.GetCurrentMethod(), parametros);
        }

        public bool ExisteArquivo(long idArquivo)
        {
            object[] parametros = { idArquivo };
            return this.ChamarServico<bool>(MethodBase.GetCurrentMethod(), parametros);
        }

        public bool NotificarInicioEnvioArquivo(long idArquivo)
        {
            object[] parametros = { idArquivo };
            return this.ChamarServico<bool>(MethodBase.GetCurrentMethod(), parametros);
        }

        public bool NotificarFimEnvioArquivo(long idArquivo, long totalBytes, string checksum)
        {
            object[] parametros = { idArquivo, totalBytes, checksum };
            return this.ChamarServico<bool>(MethodBase.GetCurrentMethod(), parametros);
        }

        public bool NotificarArquivoDeletado(long idArquivo)
        {
            object[] parametros = { idArquivo };
            return this.ChamarServico<bool>(MethodBase.GetCurrentMethod(), parametros);
        }

        public bool NotificarProgresso(long idArquivo, double progresso)
        {
            object[] parametros = { idArquivo, progresso };
            return this.ChamarServico<bool>(MethodBase.GetCurrentMethod(), parametros);
        }
        #endregion

        #region Métodos protedidos

        protected override InformacaoSessaoUsuario RetornarInformacaoSessaoUsuario()
        {
            var informacao = new InformacaoSessaoUsuario();
            informacao.IdentificadorSessaoUsuario = this.IdentificadorSessaoUsuario;
            //informacao.IPInformacao = SessaoUtil.RetornarIpInformacao();
            return informacao;
        }

        protected override CredencialUsuario CredencialAvalista
        {
            get
            {
                return CredencialUsuarioComunicacaoServicoArquivo.UsuarioComunicacaoServicoArquivo;
            }
        }
        #endregion

        #region Credenciais

        protected override CredencialServico CredencialServico => CredencialComunicacaoServicoArquivo.ServicoArquivo;

        #endregion
    }
}