using Snebur.Utilidade;
using System;
using System.IO;
using System.Text;

namespace Snebur.Servicos
{
    public abstract class BaseServicoLocal
    {

        private static object Bloqueio = new object();
        protected bool IsDebugAttachDispararErro { get; set; }

        private string _retornarRepositorioLogs;
        private string RetornarRepositorioLogs
        {
            get
            {
                if (this._retornarRepositorioLogs == null)
                {
                    this._retornarRepositorioLogs = Path.Combine(ConfiguracaoUtil.CaminhoAppDataAplicacao, "Logs", this.RetornarPastaLog());
                    DiretorioUtil.CriarDiretorio(this._retornarRepositorioLogs);
                }
                return this._retornarRepositorioLogs;
            }
        }

        private string RetornarPastaLog()
        {
            if (this is ServicoErroLocal)
            {
                return "Erros";
            }
            if (this is ServicoLogSegurancaLocal)
            {
                return "Seguranca";
            }
            if (this is ServicoLogAplicacaoLocal)
            {
                return "Aplicacao";
            }
            if (this is ServicoLogDesempenhoLocal)
            {
                return "Desempenho";
            }
            throw new ErroNaoSuportado("O servico local não é suportado");
        }

        protected void SalvarLog(string log)
        {
            if (!DebugUtil.IsAttached)
            {
                ThreadUtil.ExecutarAsync((Action)(() =>
                {
                    this.SalvarArquivoLogLocal(log);
                }));
            }
        }

        private void SalvarArquivoLogLocal(string log)
        {
            try
            {
                var nomeArquivo = String.Format("{0}.log", DateTime.Now.ToString("yyyy-MMMM-dd"));
                var caminhoArquivo = System.IO.Path.Combine(this.RetornarRepositorioLogs, nomeArquivo);

                lock (Bloqueio)
                {
                    using (var sw = new StreamWriter(caminhoArquivo, true, Encoding.UTF8))
                    {
                        sw.WriteLine(String.Format("{0} -- Utc {1}", DateTime.Now.ToLongTimeString(), DateTime.UtcNow.ToString()));
                        sw.WriteLine(log);
                        sw.Flush();
                        sw.Close();
                    }
                }
            }
            catch (Exception)
            {
                if (DebugUtil.IsAttached)
                {
                    throw;
                }
            }
        }
    }
}