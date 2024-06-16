using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Http;
using Snebur.Utilidade;
#else
using System.Web;
#endif  


namespace Snebur.ServicoArquivo
{

    public abstract class BaseBaixarImagem : BaseBaixarArquivo<CabecalhoServicoImagem, IInformacaoRepositorioImagem>
    {

        private ComunicacaoServicoImagemCliente ServicoImagemCliente
        {
            get { return (ComunicacaoServicoImagemCliente)this.ServicoArquivoCliente; }
        }

        protected override ComunicacaoServicoArquivoCliente RetornarServicoArquivoCliente(CabecalhoServicoImagem cabecalho)
        {
            var urlServicoArquivo = this.RetornarUrlServicoArquivoCliente();
            return new ComunicacaoServicoImagemCliente(urlServicoArquivo,
                                                       cabecalho.CredencialRequisicao,
                                                       cabecalho.IdentificadorSessaoUsuario,
                                                       cabecalho.IdentificadorProprietario,
                                                       this.NormalizarOrigem);
        }

#if NET6_0_OR_GREATER

        protected override async Task BaixarArquivoAsync(HttpContext httpContext, CabecalhoServicoImagem cabecalho)
        {
            if (cabecalho == null)
            {
                throw new ErroOperacaoInvalida(String.Format("O cabeçalho não foi definido."));
            }
            if (!(cabecalho.IdArquivo > 0))
            {
                throw new ErroOperacaoInvalida(String.Format("O idArquivo deve ser maior 0. '{0}", cabecalho.IdArquivo));
            }
            var IdBaseStream = cabecalho.IdArquivo;
            var tamanhoImagem = (cabecalho as CabecalhoServicoImagem).TamanhoImagem;

            if (!this.ServicoImagemCliente.ExisteImagem(IdBaseStream, tamanhoImagem))
            {
                throw new Exception(String.Format("A BaseStream com Id {0} não foi encontrada.", IdBaseStream));
            }
            string caminhoCompletoArquivo = this.RetornarCaminhoCompletoArquivo(cabecalho);
            if (!File.Exists(caminhoCompletoArquivo))
            {
                throw new Exception(String.Format("A Arquivo '{0}' não foi encontrada. {1}. Camnho completo: {2}", Path.GetFileName(caminhoCompletoArquivo), IdBaseStream.ToString(), caminhoCompletoArquivo));
            }
            try
            {
                 
                var nomeArquivo = cabecalho.IdArquivo + ".png";
                var response = httpContext.Response;
                response.ContentType = "image/png";
                response.Headers.Append("Content-Disposition", $"attachment; filename=\"{nomeArquivo}\"");

                using (var fs = StreamUtil.OpenRead(caminhoCompletoArquivo))
                {
                    await StreamUtil.SalvarStreamBufferizadaAsync(fs, response.Body);
                }
                  
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler stream" + ex.Message);
            }
        }
#else

        protected override void BaixarArquivo(HttpContext httpContext, CabecalhoServicoImagem cabecalho)
        {
            if (cabecalho == null)
            {
                throw new ErroOperacaoInvalida(String.Format("O cabeçalho não foi definido."));
            }

            if (!(cabecalho.IdArquivo > 0))
            {
                throw new ErroOperacaoInvalida(String.Format("O idArquivo deve ser maior 0. '{0}", cabecalho.IdArquivo));
            }

            var IdBaseStream = cabecalho.IdArquivo;
            var tamanhoImagem = (cabecalho as CabecalhoServicoImagem).TamanhoImagem;

            if (!this.ServicoImagemCliente.ExisteImagem(IdBaseStream, tamanhoImagem))
            {
                throw new Exception(String.Format("A BaseStream com Id {0} não foi encontrada.", IdBaseStream));
            }

            string caminhoCompletoArquivo = this.RetornarCaminhoCompletoArquivo(cabecalho);
            if (!File.Exists(caminhoCompletoArquivo))
            {
                throw new Exception(String.Format("A Arquivo '{0}' não foi encontrada. {1}. Camnho completo: {2}", Path.GetFileName(caminhoCompletoArquivo), IdBaseStream.ToString(), caminhoCompletoArquivo));
            }

            try
            {
 
                httpContext.Response.ContentType = "image/png";
                httpContext.Response.ContentEncoding = Encoding.UTF8;
                httpContext.Response.WriteFile(caminhoCompletoArquivo);
 


                //var bytesArquivo = File.ReadAllBytes(caminhoCompletoArquivo);
                //httpContext.Response.OutputStream.Write(bytesArquivo.ToArray(), 0, bytesArquivo.Length);

                //Using ms As New IO.MemoryStream
                //    Using fs As New IO.FileStream(caminhoCompletoArquivo, IO.FileMode.Open, IO.FileAccess.Read)
                //        fs.CopyTo(ms)
                //    End Using
                //    ms.Position = 0
                //    httpContext.Response.OutputStream.Write(ms.ToArray.Reverse.ToArray, 0, ms.Length)
                //End Using
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler stream" + ex.Message);
            }
        }

#endif

        protected override string RetornarCaminhoCompletoArquivo(IInformacaoRepositorioImagem informacaoRepositorio)
        {
            var tamanhoImagem = informacaoRepositorio.TamanhoImagem;
            var caminhoDiretorioImagem = this.RetornarDiretorioImagem(informacaoRepositorio);
            var nomeArquivo = ServicoArquivoUtil.RetornarNomeArquivo(informacaoRepositorio.IdArquivo, ServicoImagemUtil.RetornarExtensaoImagem(tamanhoImagem));
            return Path.Combine(caminhoDiretorioImagem, nomeArquivo);
        }

        #region  Métodos sobre-escritos

        protected override CabecalhoServicoImagem RetornarCabecalhoServicoArquivo(HttpContext httpContext)
        {
            return new CabecalhoServicoImagem(httpContext, true);
        }

        protected override sealed string RetornarDiretorioArquivo(IInformacaoRepositorioImagem informacaoRepositorioImagem)
        {
            var tamanhoImagem = (informacaoRepositorioImagem as CabecalhoServicoImagem).TamanhoImagem;
            return this.RetornarDiretorioImagem(informacaoRepositorioImagem);
        }

        protected override sealed string RetornarRepositoArquivo(IInformacaoRepositorioImagem informacaoRepositorio)
        {
            return this.RetornarRepositorioImagem(informacaoRepositorio);
        }

        public string RetornarDiretorioImagem(IInformacaoRepositorioImagem informacaoRepositorioImagem)
        {
            return ServicoArquivoUtil.RetornarCaminhoDiretorioArquivo(this.RetornarRepositorioImagem(informacaoRepositorioImagem), informacaoRepositorioImagem.IdArquivo);
        }
        #endregion


        protected abstract string RetornarRepositorioImagem(IInformacaoRepositorioImagem informacaoRepositorio);
    }
}