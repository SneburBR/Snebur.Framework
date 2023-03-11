using Snebur.Dominio;
using System;
#if NET7_0
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif  


namespace Snebur.ServicoArquivo
{
    public class CabecalhoServicoImagem : CabecalhoServicoArquivo, IInformacaoRepositorioImagem
    {
        public EnumTamanhoImagem TamanhoImagem { get; }

        public EnumFormatoImagem Formato { get; }

        public CabecalhoServicoImagem(HttpContext context, bool enviarArquivo = false) : base(context, enviarArquivo)
        {
            this.TamanhoImagem = (EnumTamanhoImagem)this.RetornarInteger(ConstantesServicoImagem.TAMANHO_IMAGEM);
            this.Formato = (EnumFormatoImagem)this.RetornarInteger(ConstantesServicoImagem.FORMATO_IMAGEM);


            if (!Enum.IsDefined(typeof(EnumFormatoImagem), this.Formato))
            {
                this.Formato = EnumFormatoImagem.JPEG;
            }
        }


        //#region IInformacaoRepositorioImagem

        //long IInformacaoRepositorioImagem.IdImagem => this.IdArquivo;

        //#endregion
    }
}
