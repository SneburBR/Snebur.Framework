using Snebur.Utilidade;
using System;
using System.Configuration;

#if NET6_0_OR_GREATER == false
using Snebur.Computador;
#endif

namespace Snebur.ServicoArquivo.Servidor
{
    public abstract class BaseAplicacaoServicoArquivo : AplicacaoSneburAspNet
    {
        private const string CHAVE_AUTENTICAR_ACESSO_COMPARTILHADO = "IsAutenticarAcessoCompartilhado";
        private const string CHAVE_NOME_COMPUTADOR = "NomeComputadorAcesso";
        private const string CHAVE_USUARIO = "Usuario";
        private const string CHAVE_SENHA = "Senha";
        public bool IsAutenticarAcessoCompartilhado => Convert.ToBoolean(ConfigurationManager.AppSettings[CHAVE_AUTENTICAR_ACESSO_COMPARTILHADO]);
        private string NomeComputadorAcesso => ConfiguracaoUtil.AppSettings[CHAVE_NOME_COMPUTADOR];
        private string Usuario => ConfiguracaoUtil.AppSettings[CHAVE_USUARIO];
        private string Senha => ConfiguracaoUtil.AppSettings[CHAVE_SENHA];

#if NET6_0_OR_GREATER == false
        public AcessoCompartilhamentoRede AcessoCompartilhamentoRede { get; private set; }
#endif
        public BaseAplicacaoServicoArquivo() : base()
        {

        }
#if NET6_0_OR_GREATER == false
        public void AcessarRede()
        {
            if (this.IsAutenticarAcessoCompartilhado && this.AcessoCompartilhamentoRede == null)
            {
                this.AcessoCompartilhamentoRede = new AcessoCompartilhamentoRede(this.NomeComputadorAcesso,
                                                                                 this.Usuario,
                                                                                 this.Senha);


            }
            AppDomain.CurrentDomain.ProcessExit += this.AppDomain_ProcessExit;
        }

        private void AppDomain_ProcessExit(object sender, EventArgs e)
        {
            this.AcessoCompartilhamentoRede?.Dispose();
        }
#else
        public void AcessarRede()
        {
            throw new Exception("Não implementado para .net 7.0");
        }
#endif
        }
    }
