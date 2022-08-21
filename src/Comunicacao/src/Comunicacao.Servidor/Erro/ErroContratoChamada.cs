using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using System.Runtime.CompilerServices;

namespace Snebur.Comunicacao
{
    public class ErroContratoChamada : ErroComunicacao
    {
        public ErroContratoChamada(Exception erroInterno,
                                   [CallerMemberName] string nomeMetodo = "",
                                   [CallerFilePath] string caminhoArquivo = "",
                                   [CallerLineNumber] int linhaDoErro = 0) :
                                   base("Não foi possivel deserializar o conteudo", 
                                       erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {

        }
    }
}
