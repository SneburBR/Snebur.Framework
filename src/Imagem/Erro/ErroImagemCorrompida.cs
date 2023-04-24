using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace Snebur.Imagens
{
    [Serializable()]
    public class ErroImagemCorrompida : Erro
    {
        protected override bool IsNotificarErro { get; } = false;

        public ErroImagemCorrompida(string mensagem = "",
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

        public ErroImagemCorrompida()
        {
            this.IsNotificarErro = false;
        }

        protected ErroImagemCorrompida(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.IsNotificarErro = false;
        }

        #endregion
    }
}
