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
    public class ErroDimensaoImagem : Erro
    {
        protected override bool IsNotificarErro { get; } = false;

        public ErroDimensaoImagem(string mensagem = "",
                              Exception erroInterno = null,
                              [CallerMemberName] string nomeMetodo = "",
                              [CallerFilePath] string caminhoArquivo = "",
                              [CallerLineNumber] int linhaDoErro = 0) :
                              base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {

        }

        #region Serializacao 

        public ErroDimensaoImagem()
        {

        }

        protected ErroDimensaoImagem(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

   

        #endregion
    }
}
