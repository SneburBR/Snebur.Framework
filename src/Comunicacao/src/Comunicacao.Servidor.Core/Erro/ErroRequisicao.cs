using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.Comunicacao
{
    public class ErroRequisicao : Erro
    {
        public string NomeManipulador { get; }

        public ErroRequisicao(Exception erroInterno,
                             Requisicao requisicao,
                             [CallerMemberName] string nomeMetodo = "",
                             [CallerFilePath] string caminhoArquivo = "",
                             [CallerLineNumber] int linhaDoErro = 0) :
                             base(RetornarMensagemErro(erroInterno, requisicao),
                                  erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
        {
            this.NomeManipulador = requisicao.NomeManipulador;
        }

        private static string RetornarMensagemErro(Exception erro, Requisicao requisicao)
        {
            return $"Manipulador {requisicao.NomeManipulador} \r\n " +
                   $"Operacao : {requisicao.Operacao} \r\n " +
                   $"Erro: {erro.Message}";
        }
    }
}
