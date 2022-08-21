using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using Snebur.Dominio;

namespace Snebur.Comunicacao
{
    [Serializable]
    public class ErroMetodoOperacaoNaoFoiEncontrado : Exception
    {
        public string NomeMetodo { get; set; }

        public ErroMetodoOperacaoNaoFoiEncontrado(string nomeMetodo, string mensagem):base(mensagem)
        {
            this.NomeMetodo = nomeMetodo;
        }
    }
}
