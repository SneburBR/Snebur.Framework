using Snebur.Utilidade;
using System;
using System.IO;
using System.Web;

namespace Snebur.ServicoArquivo
{

    public abstract partial class BaseEnviarArquivo<TCabecalhoServicoArquivo, TInformacaoReposotiroArquivo> : BaseServicoArquivo<TCabecalhoServicoArquivo, TInformacaoReposotiroArquivo> where TCabecalhoServicoArquivo : CabecalhoServicoArquivo, TInformacaoReposotiroArquivo
                                                                                                                                                                                         where TInformacaoReposotiroArquivo : IInformacaoRepositorioArquivo
    {
        private void SalvarArquivoTempoariario(HttpContext httpContext, long idArquivo, TCabecalhoServicoArquivo cabecalho, MemoryStream inputStream)
        {
            if (!(cabecalho.ParteAtual > 0))
            {
                throw new Exception(String.Format("Partal atual da arquivo deve ser maior que 0. Parte atual: '{0}'", cabecalho.ParteAtual));
            }
            //if (cabecalho.ParteAtual == 1)
            //{
            //    this.NotificarInicioEnvioArquivo(cabecalho, idArquivo);
            //}

            //var pacote = new byte[httpContext.Request.ContentLength - 2];
            var pacote = inputStream.ToArray();
            var checksumPacote = ChecksumUtil.RetornarChecksum(pacote);

            if (checksumPacote != cabecalho.CheckSumPacote)
            {
                throw new ErroChecksumPacote($"O checksums '{checksumPacote}', '{cabecalho.CheckSumPacote}' informato no cabecalho são diferente do pacote.  Id Arquivo {cabecalho.IdArquivo}, ParteAtual {cabecalho.ParteAtual}. ");
            }

            using (var streamTemporaria = this.RetornarStreamArquivoTemporario(cabecalho, cabecalho.ParteAtual, cabecalho.IdArquivo, cabecalho.TamanhoPacote))
            {

                streamTemporaria.Write(pacote, 0, pacote.Length);
                streamTemporaria.Flush(true);
            }

            if (cabecalho.ParteAtual == cabecalho.TotalPartes)
            {
                var caminhoTemporario = this.RetornarCaminhoArquivoTempoarario(cabecalho, idArquivo);
                var caminhoCompletoArquivo = this.RetornarCaminhoCompletoArquivo(cabecalho);
                ArquivoUtil.DeletarArquivo(caminhoCompletoArquivo, false, true);
                File.Move(caminhoTemporario, caminhoCompletoArquivo);
                this.FinalizarEnvioArquivo(cabecalho, idArquivo, caminhoCompletoArquivo, cabecalho.CheckSumArquivo, cabecalho.TotalBytes);
            }
            else
            {
                if (this.FrequenciaNotificarProgresso > 1)
                {
                    var progresso = (cabecalho.ParteAtual / cabecalho.TotalPartes) * 100;
                    if (this.FrequenciaNotificarProgresso > 0 && (cabecalho.ParteAtual % this.FrequenciaNotificarProgresso) == 0)
                    {
                        this.NotificarProgresso(cabecalho, idArquivo, progresso);
                    }
                }
            }
        }



        private void DeletarArquivoTempoarario(string caminhoTemporario)
        {
            ArquivoUtil.DeletarArquivo(caminhoTemporario, true, true);
        }

        private FileStream RetornarStreamArquivoTemporario(TCabecalhoServicoArquivo cabecalho, double parteAtual, long idArquivo, long tamanhoPacote, int tentativa = 0)
        {

            var caminhoTemporario = this.RetornarCaminhoArquivoTempoarario(cabecalho, idArquivo);
            if (parteAtual == 1)
            {
                ArquivoUtil.DeletarArquivo(caminhoTemporario, false, true);
            }
            var ponteiroAtual = Convert.ToInt32((parteAtual - 1) * tamanhoPacote);
            try
            {
                //var stream = File.OpenWrite(caminhoTemporario);
                var stream = new FileStream(caminhoTemporario, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                stream.Seek(ponteiroAtual, SeekOrigin.Begin);
                return stream;
            }
            catch (IOException ex)
            {
                //arquivo em uso
                tentativa += 1;

                var mensagemErro = String.Format("Arquivo em uso Id: {0} - Tentativa : {1} - Arquivo: {2}", idArquivo, tentativa, caminhoTemporario);
                var erro = new ErroArquivoEmUso(mensagemErro, ex);

                LogUtil.ErroAsync(erro);

                System.Threading.Thread.Sleep(Convert.ToInt32(TimeSpan.FromSeconds(3).TotalMilliseconds));

                if ((tentativa > 5))
                {
                    throw erro;
                }
                return this.RetornarStreamArquivoTemporario(cabecalho, parteAtual, idArquivo, tamanhoPacote, tentativa);
            }
            catch (Exception ex)
            {
                LogUtil.ErroAsync(ex);
                throw;
            }
        }

        protected abstract string RetornarCaminhoArquivoTempoarario(TCabecalhoServicoArquivo cabecalho, long idArquivo);
    }
}