using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace System
{
    [Serializable]
    public abstract class ErroServicoArquivo : Erro
    {
        public ErroServicoArquivo(string mensagem = "",
                                    Exception erroInterno = null,
                                    [CallerMemberName] string nomeMetodo = "",
                                    [CallerFilePath] string caminhoArquivo = "",
                                    [CallerLineNumber] int linhaDoErro = 0) : 
                                     base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        #region Serializacao 

        public ErroServicoArquivo()
        {
        }

        protected ErroServicoArquivo(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }
}