using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace Snebur.Imagem
{
    
    [Serializable()]
    public class ErroMemoriaInsuficiente : Erro
    {
        protected override bool IsNotificarErro { get; } = false;

        public ErroMemoriaInsuficiente(string mensagem = "",
                                   Exception erroInterno = null,
                                   [CallerMemberName] string nomeMetodo = "",
                                   [CallerFilePath] string caminhoArquivo = "",
                                   [CallerLineNumber] int linhaDoErro = 0) :
                                   base(mensagem,
                                       erroInterno,
                                       nomeMetodo,
                                       caminhoArquivo,
                                       linhaDoErro)
        {

        }

        #region Serializacao 

        public ErroMemoriaInsuficiente()
        {
            this.IsNotificarErro = true;
        }

        protected ErroMemoriaInsuficiente(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.IsNotificarErro = true;
        }

        #endregion
    }
}
