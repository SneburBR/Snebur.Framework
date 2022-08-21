using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Snebur.Dominio;
using Snebur.Net;
using Snebur.Utilidade;

#if NET50
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
#endif

namespace Snebur.ServicoArquivo
{
    public abstract class BaseVisualizarImagem : IHttpHandler
    {
        public bool IsReusable => true;

        public static HashSet<EnumTamanhoImagem> TamanhosSuportados = new HashSet<EnumTamanhoImagem>(new EnumTamanhoImagem[] { EnumTamanhoImagem.Miniatura,
                                                                                                                               EnumTamanhoImagem.Pequena,
                                                                                                                               EnumTamanhoImagem.Media,
                                                                                                                               EnumTamanhoImagem.Grande,
                                                                                                                               EnumTamanhoImagem.Impressao });
#if NET50
        public async Task ProcessRequestAsync(HttpContext context)
        {
            using (var zyonHttpContext = new ZyonHttpContextCore(context))
            {
                await this.ProcessRequestAsyc(zyonHttpContext);
            }
        }

        public async Task ProcessRequestAsyc(ZyonHttpContext context)
        {
            var caminhoImagem = this.RetornarCaminhoImagem(context);
            var response = context.Response;

            if (File.Exists(caminhoImagem))
            {
                response.ContentType = "image/jpeg";
                response.StatusCode = 200;

                await response.WriteFileAsync(caminhoImagem);
            }
            else
            {
                LogUtil.ErroAsync(new ErroArquivoNaoEncontrado(caminhoImagem));
                response.SubStatusCode = 5;
                response.StatusCode = 405;
            }
        }

#endif

        public void ProcessRequest(HttpContext context)
        {
            using (var zyonHttpContext = new ZyonHttpContextCore(context))
            {
                this.ProcessRequest(zyonHttpContext);
            }
        }

        public void ProcessRequest(ZyonHttpContext context)
        {
            var caminhoImagem = this.RetornarCaminhoImagem(context);
            var response = context.Response;

            if (File.Exists(caminhoImagem))
            {
                response.ContentType = "image/jpeg";
                response.StatusCode = 200;
                response.WriteFile(caminhoImagem);
            }
            else
            {
                LogUtil.ErroAsync(new ErroArquivoNaoEncontrado(caminhoImagem));
                response.SubStatusCode = 5;
                response.StatusCode = 405;
            }
        }

        public virtual string RetornarCaminhoImagem(ZyonHttpContext zyonHttpContext)
        {
            var tamanhoImagem = this.RetornarTamanhoImagem(zyonHttpContext);
            var idImagem = this.RetornarIdImagem(zyonHttpContext);
            var nomeTipoImagem = this.RetornarNomeTipoImagem(zyonHttpContext);
            var caminhoDiretorioImagem = this.RetornarDiretorioImagem(idImagem, nomeTipoImagem, tamanhoImagem);
            var nomeArquivo = ServicoArquivoUtil.RetornarNomeArquivo(idImagem, ServicoImagemUtil.RetornarExtensaoImagem(tamanhoImagem));
            var caminhoImagem = Path.Combine(caminhoDiretorioImagem, nomeArquivo);
            return caminhoImagem;
            //if (!File.Exists(caminhoImagem) || isAceitarOutrosTamanho)
            //{
            //    var tamanhosImagem = EnumUtil.RetornarValoresEnum<EnumTamanhoImagem>();
            //    foreach (var tamanhoImagemOpcional in tamanhosImagem)
            //    {
            //        nomeArquivo = ServicoArquivoUtil.RetornarNomeArquivo(idImagem, ServicoImagemUtil.RetornarExtensaoImagem(tamanhoImagemOpcional));
            //        caminhoDiretorioImagem = this.RetornarDiretorioImagem(nomeTipoImagem, idImagem, tamanhoImagem);
            //        caminhoImagem = Path.Combine(caminhoDiretorioImagem, nomeArquivo);
            //        if (File.Exists(caminhoImagem))
            //        {
            //            return caminhoImagem;
            //        }
            //    }
            //}
            //return caminhoImagem;
        }


        public string RetornarDiretorioImagem(long idArquivo, string nomeTipoImagem, EnumTamanhoImagem tamanhoImagem)
        {
            var info = new InformacaoRepositorioImagem(idArquivo, nomeTipoImagem, tamanhoImagem);
            return ServicoArquivoUtil.RetornarCaminhoDiretorioArquivo(this.RetornarRepositorioImagem(info), idArquivo);
        }

        #region Métodos privados

        protected long RetornarIdImagem(ZyonHttpContext zyonHttpContext)
        {
            var idImagem = Convert.ToInt64(this.RetornarValorParametro(ConstantesServicoImagem.ID_IMAGEM, zyonHttpContext));
            if (!(idImagem > 0))
            {
                throw new Exception(String.Format("Parâmetro '{0}' não foi definido.", ConstantesServicoImagem.ID_IMAGEM));
            }
            return idImagem;
        }

        protected string RetornarNomeTipoImagem(ZyonHttpContext zyonHttpContext)
        {
            var nomeTipoArquivo = this.RetornarValorParametro(ConstantesServicoArquivo.NOME_TIPO_ARQUIVO, zyonHttpContext);
            if (String.IsNullOrWhiteSpace(nomeTipoArquivo))
            {
                throw new Exception(String.Format("Parâmetro '{0}' não foi definido.", ConstantesServicoArquivo.NOME_TIPO_ARQUIVO));
            }
            return nomeTipoArquivo;
        }


        protected virtual EnumTamanhoImagem RetornarTamanhoImagem(ZyonHttpContext zyonHttpContext)
        {
            var tamanhoImagem = (EnumTamanhoImagem)Convert.ToInt32(this.RetornarValorParametro(ConstantesServicoImagem.TAMANHO_IMAGEM, zyonHttpContext));
            if (!Enum.IsDefined(typeof(EnumTamanhoImagem), tamanhoImagem))
            {
                throw new Exception(String.Format("Parâmetro '{0}' não suportado.", ConstantesServicoImagem.TAMANHO_IMAGEM));
            }
            return tamanhoImagem;
        }

        protected string RetornarValorParametro(string parametro, ZyonHttpContext zyonHttpContext)
        {
            var parametroBase64 = Base64Util.Encode(parametro);
            return Base64Util.Decode(zyonHttpContext.Request.QueryString[parametroBase64]);
        }
        #endregion

        protected abstract string RetornarRepositorioImagem(IInformacaoRepositorioImagem informacaoRepositorio);


    }


}