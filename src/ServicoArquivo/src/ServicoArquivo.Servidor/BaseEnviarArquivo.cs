using System;
using System.IO;
using Snebur.Net;
using Snebur.Utilidade;

namespace Snebur.ServicoArquivo
{
    //public delegate void AsyncTaskDelegate(HttpContext httpContext);

    public abstract partial class BaseEnviarArquivo<TCabecalhoServicoArquivo, TInformacaoReposotiroArquivo> : BaseServicoArquivo<TCabecalhoServicoArquivo, TInformacaoReposotiroArquivo> where TCabecalhoServicoArquivo : CabecalhoServicoArquivo, TInformacaoReposotiroArquivo
                                                                                                                                                                                         where TInformacaoReposotiroArquivo : IInformacaoRepositorioArquivo
    {
        #region Construtores

        public BaseEnviarArquivo() : base()
        {
        }

        #endregion

        #region reescritos

        protected override void Iniciar(SnHttpContext context, TCabecalhoServicoArquivo cabecalho, MemoryStream inputStream)
        {
            this.SalvarArquivo(context, cabecalho, inputStream);
        }

        protected override TCabecalhoServicoArquivo RetornarCabecalhoServicoArquivo(SnHttpContext httpContext)
        {
            return new CabecalhoServicoArquivo(httpContext, true) as TCabecalhoServicoArquivo;
        }

        #endregion

        #region Métodos protegidos

        protected virtual void SalvarArquivo(SnHttpContext httpContext, TCabecalhoServicoArquivo cabecalho,  MemoryStream inputStreasm)
        {
            if (!(cabecalho.IdArquivo > 0))
            {
                throw new ErroOperacaoInvalida(String.Format("O idArquivo deve ser maior 0. '{0}", cabecalho.IdArquivo));
            }

            var idArquivo = cabecalho.IdArquivo;
            if (cabecalho.ParteAtual <= 1)
            {
                if (!this.ServicoArquivoCliente.ExisteIdArquivo(idArquivo))
                {
                    throw new ErroIdArquivoNaoExiste(String.Format("O arquivo com Id {0} não foi encontrada.", idArquivo));
                }
            }
            if (cabecalho.ParteAtual == 1)
            {
                this.NotificarInicioEnvioArquivo(cabecalho, cabecalho.IdArquivo);
            }
            this.SalvarArquivoTempoariario(httpContext, idArquivo, cabecalho, inputStreasm);
        }

        protected virtual string RetornarCaminhoRepositoTemporario(TCabecalhoServicoArquivo cabecalho)
        {
            var caminhoTemporario = Path.Combine(this.RetornarRepositoArquivo(cabecalho), "Temp");
            DiretorioUtil.CriarDiretorio(caminhoTemporario);
            return caminhoTemporario;
        }

        protected virtual void NotificarInicioEnvioArquivo(TCabecalhoServicoArquivo cabecalho, long idArquivo)
        {
            this.ServicoArquivoCliente.NotificarInicioEnvioArquivo(idArquivo);
        }

        protected virtual void NotificarFimEnvioArquivo(TCabecalhoServicoArquivo cabecalho, long idArquivo, long totalBytes, string checksum)
        {
            this.ServicoArquivoCliente.NotificarFimEnvioArquivo(idArquivo, totalBytes, checksum);
        }

        protected virtual void NotificarProgresso(TCabecalhoServicoArquivo cabecalho, long idArquivo, double progresso)
        {
            this.ServicoArquivoCliente.NotificarProgresso(idArquivo, progresso);
        }
        #endregion

        #region Métodos privados

        private void FinalizarEnvioArquivo(TCabecalhoServicoArquivo cabecalho, long idArquivo, string caminhoCompletoStream, string checksum, long totalBytesCliente)
        {
            if (!String.IsNullOrWhiteSpace(checksum))
            {
                var checkSumArquivo = this.RetornarChecksum(caminhoCompletoStream);
                if (checkSumArquivo != checksum)
                {
                    throw new ErroChecksumArquivo($"ServicoArquivo. Os checksum são diferentes. Id: '{idArquivo}' - '{checksum}' - '{checkSumArquivo}' ParteAtual {cabecalho.ParteAtual}");
                }
            }

            var totalBytesArquivo = new FileInfo(caminhoCompletoStream).Length;
            if ((totalBytesArquivo != totalBytesCliente))
            {
                throw new ErroTotalBytesDiferente(String.Format("ServicoArquivo. O total de bytes são diferentes. Id: {0} - '{1}' - '{2}'", idArquivo, totalBytesArquivo, totalBytesCliente));
            }
            this.NotificarFimEnvioArquivo(cabecalho, idArquivo, totalBytesArquivo, checksum);
        }

        private string RetornarChecksum(string caminhoArquivo)
        {
            return ChecksumUtil.RetornarChecksum(caminhoArquivo);
        }
        #endregion

        protected virtual int FrequenciaNotificarProgresso { get; } = 1;
    }

    public abstract class BaseEnviarArquivo : BaseEnviarArquivo<CabecalhoServicoArquivo, IInformacaoRepositorioArquivo>
    {

    }
}