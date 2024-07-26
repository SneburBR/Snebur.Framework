using Snebur.Computador;
using Snebur.Utilidade;
using System;
using System.Configuration;
using System.Runtime.InteropServices;

namespace Snebur.ServicoArquivo.Servidor
{
    public abstract class BaseAplicacaoServicoArquivo : AplicacaoSneburAspNet
    {
        private const string CHAVE_AUTENTICAR_ACESSO_COMPARTILHADO = "IsAutenticarAcessoCompartilhado";
        private const string CHAVE_NOME_COMPUTADOR = "NomeComputadorAcesso";
        private const string CHAVE_USUARIO = "Usuario";
        private const string CHAVE_SENHA = "Senha";
        public bool IsAutenticarAcessoCompartilhado => Convert.ToBoolean(ConfiguracaoUtil.AppSettings[CHAVE_AUTENTICAR_ACESSO_COMPARTILHADO]);
        private string NomeComputadorAcesso => ConfiguracaoUtil.AppSettings[CHAVE_NOME_COMPUTADOR];
        private string Usuario => ConfiguracaoUtil.AppSettings[CHAVE_USUARIO];
        private string Senha => ConfiguracaoUtil.AppSettings[CHAVE_SENHA];


        public AcessoCompartilhamentoRede AcessoCompartilhamentoRede { get; private set; }

        public BaseAplicacaoServicoArquivo() : base()
        {

        }

        public void AcessarRede()
        {
            if (this.IsAutenticarAcessoCompartilhado &&
                this.AcessoCompartilhamentoRede == null)
            {

                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    throw new InvalidOperationException("Sistema operacional não suportado para acesso a rede autenticado");
                }

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
    }
}
