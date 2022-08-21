using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.Imagem
{
    [Serializable()]
    public class ErroImagem : Erro
    {

        public ErroImagem(string mensagem = "",
                              Exception erroInterno = null,
                              [CallerMemberName] string nomeMetodo = "",
                              [CallerFilePath] string caminhoArquivo = "",
                              [CallerLineNumber] int linhaDoErro = 0) :
                              base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {

        }

        #region Serializacao 

        public ErroImagem()
        {

        }

        protected ErroImagem(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        #endregion
    }
}
