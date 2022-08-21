using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.Comunicacao
{
    public class ErroManipualdorNaoEncontrado : ErroComunicacao
    {
        public ErroManipualdorNaoEncontrado(string mensagem,
                                 [CallerMemberName] string nomeMetodo = "",
                                 [CallerFilePath] string caminhoArquivo = "",
                                 [CallerLineNumber] int linhaDoErro = 0) :
                                   base(mensagem,
                                       null, nomeMetodo, caminhoArquivo, linhaDoErro)
        {

        }
    }
}
