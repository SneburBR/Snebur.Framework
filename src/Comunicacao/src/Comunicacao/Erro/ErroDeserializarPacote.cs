using System.Runtime.CompilerServices;

namespace Snebur.Comunicacao;

[Serializable]
public class ErroDeserializarContrato : ErroComunicacao
{
    public string Json { get; set; }

    public ErroDeserializarContrato(string json, Exception erroInterno,
                                  [CallerMemberName] string nomeMetodo = "",
                                  [CallerFilePath] string caminhoArquivo = "",
                                  [CallerLineNumber] int linhaDoErro = 0) :
                                   base("Não foi possivel deserializar o conteudo", erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {
        this.Json = json;
    }
}