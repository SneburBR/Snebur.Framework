using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Snebur.EntityFramework
{
    [Serializable()]
    public class ErroEntity : Erro
    {
        public ErroEntity(string mensagem = "",
                          Exception erroInterno = null,
                          [CallerMemberName] string nomeMetodo = "",
                          [CallerFilePath] string caminhoArquivo = "",
                          [CallerLineNumber] int linhaDoErro = 0) :
                          base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        #region Serializacao 

        public ErroEntity()
        {
        }

        protected ErroEntity(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }
}