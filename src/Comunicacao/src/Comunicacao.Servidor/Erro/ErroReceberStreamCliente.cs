using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.Comunicacao
{
    public class ErroReceberStreamCliente : ErroComunicacao
    {
        public ErroReceberStreamCliente(string mensagem = "",
                                       Exception erroInterno = null,
                                       [CallerMemberName] string nomeMetodo = "",
                                       [CallerFilePath] string caminhoArquivo = "",
                                       [CallerLineNumber] int linhaDoErro = 0) :
                                       base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
    }
}
