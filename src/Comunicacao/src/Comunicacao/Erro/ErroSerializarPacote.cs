using System.Runtime.CompilerServices;

namespace Snebur.Comunicacao;

public class ErroSerializarPacote : ErroComunicacao
{
    public Type Tipo { get; set; }

    public ErroSerializarPacote(Type tipo, Exception erroInterno,
                                [CallerMemberName] string nomeMetodo = "",
                                [CallerFilePath] string caminhoArquivo = "",
                                [CallerLineNumber] int linhaDoErro = 0) :
                                base("Não foi possivel deserializar o conteudo", erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {
        this.Tipo = tipo;
    }
}
