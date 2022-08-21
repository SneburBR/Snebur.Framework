using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.Comunicacao
{
    public class ErroWebService: ErroComunicacao
    {
        public ErroWebService(Exception erroInterno,
                              string nomeManipulador,
                              [CallerMemberName] string nomeMetodo = "",
                              [CallerFilePath] string caminhoArquivo = "",
                              [CallerLineNumber] int linhaDoErro = 0) :
                              base(erroInterno.Message,
                                      erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {

        }
    }
}
