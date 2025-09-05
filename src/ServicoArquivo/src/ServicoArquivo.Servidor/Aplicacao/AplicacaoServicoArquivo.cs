using Snebur.Computador;
using System.Runtime.InteropServices;

namespace Snebur.ServicoArquivo.Servidor;

public abstract class BaseAplicacaoServicoArquivo : AplicacaoSneburAspNet
{
    public bool IsAutenticarAcessoCompartilhado
        => ConfiguracaoLocalUtil.IsAutenticarAcessoCompartilhado;
    private string NomeComputadorAcesso
        => ConfiguracaoLocalUtil.NomeComputadorAcesso;
    private string Usuario
        => ConfiguracaoLocalUtil.Usuario;
    private string Senha
        => ConfiguracaoLocalUtil.Senha;

    public AcessoCompartilhamentoRede? AcessoCompartilhamentoRede { get; private set; }

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

    private void AppDomain_ProcessExit(object? sender, EventArgs e)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            this.AcessoCompartilhamentoRede?.Dispose();
        }
    }
}
