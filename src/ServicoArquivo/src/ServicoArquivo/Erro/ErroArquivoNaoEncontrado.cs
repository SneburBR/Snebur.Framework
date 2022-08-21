using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace Snebur.ServicoArquivo
{
    [Serializable]
    public class ErroArquivoNaoEncontrado : ErroServicoArquivo
    {

        public ErroArquivoNaoEncontrado(string mensagem = "",
                                        Exception erroInterno = null,
                                        [CallerMemberName] string nomeMetodo = "",
                                        [CallerFilePath] string caminhoArquivo = "",
                                        [CallerLineNumber] int linhaDoErro = 0) : 
                                        base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
        }
        #region Serializacao 

        public ErroArquivoNaoEncontrado()
        {
        }

        protected ErroArquivoNaoEncontrado(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }

    
}