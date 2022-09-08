using System;

namespace Snebur.Comunicacao
{
    [Serializable]
    public class ErroMetodoOperacaoNaoFoiEncontrado : Exception
    {
        public string NomeMetodo { get; set; }

        public ErroMetodoOperacaoNaoFoiEncontrado(string nomeMetodo, string mensagem) : base(mensagem)
        {
            this.NomeMetodo = nomeMetodo;
        }
    }
}
