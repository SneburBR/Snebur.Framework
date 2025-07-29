using System;

namespace Snebur.Utilidade
{
    public class ThreadControleErro
    {
        private Action? _acao;
        private string NomeThread { get; }

        public ThreadControleErro(Action acao, string nomeThread)
        {
            this._acao = acao;
            this.NomeThread = nomeThread;
        }

        public void ExecutarAsync()
        {
            ThreadUtil.ExecutarThread(this.Executar, this.NomeThread);
        }

        public void Executar()
        {
            Guard.NotNull(this._acao);

            try
            {
                this._acao.Invoke();
            }
            catch (Exception ex)
            {
                if (!DebugUtil.IsAttached)
                {
                    var mensagem = $"Erro na thread {this.NomeThread}";
                    LogUtil.ErroAsync(new Exception(mensagem, ex));
                }
            }
            finally
            {
                this._acao = null;
            }
        }
    }
}