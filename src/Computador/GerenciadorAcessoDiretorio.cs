using Snebur.IO;
using System;

namespace Snebur.Computador;

public class GerenciadorAcessoDiretorio : IDisposable
{
    public IAcessoDiretorio AcessoDiretorio { get; }

    public AcessoDiscoComOutroUsuario? AcessoDiscoComOutroUsuario { get; }

    public AcessoCompartilhamentoRede? AcessoCompartilhamentoRede { get; }

    public GerenciadorAcessoDiretorio(IAcessoDiretorio acessoDiretorio)
    {
        this.AcessoDiretorio = acessoDiretorio;
        if (this.AcessoDiretorio.IsAutenticar)
        {
            if (this.AcessoDiretorio.IsRede)
            {
                this.AcessoCompartilhamentoRede = new AcessoCompartilhamentoRede(this.AcessoDiretorio);
            }
            else
            {
                this.AcessoDiscoComOutroUsuario = new AcessoDiscoComOutroUsuario(this.AcessoDiretorio);
            }
        }
    }

    //public bool ExisteDiretorio(string caminho)
    //{
    //    return Directory.Exists(caminho);
    //}

    //public bool ExisteArquivo(string caminho)
    //{

    //}

    //public bool CopiarArquivo(string caminhoOrigem, string caminhoDestino)
    //{

    //}

    //public bool SalvarArquivo(Stream streamOrigem, string caminhoDestino)
    //{

    //}

    public void Dispose()
    {
        this.AcessoDiscoComOutroUsuario?.Dispose();
        this.AcessoCompartilhamentoRede?.Dispose();
    }
}
