namespace Snebur.Comunicacao;

public class ArgsResultadoChamadaServico : System.ComponentModel.AsyncCompletedEventArgs
{

    public object? Resultado { get; }

    public ArgsResultadoChamadaServico(Exception? erro, object? resultado, object? userState)
        : base(erro, false, userState)
    {
        this.Resultado = resultado;
    }
}
