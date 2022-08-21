using System;
using Snebur.Dominio;
using System.Web;
using Snebur.Net;

namespace Snebur.ServicoArquivo
{
    public class CabecalhoServicoImagem : CabecalhoServicoArquivo, IInformacaoRepositorioImagem
    {
        public EnumTamanhoImagem TamanhoImagem { get; }

        public EnumFormatoImagem Formato { get; }

        public CabecalhoServicoImagem(ZyonHttpContext context, bool enviarArquivo = false) : base(context, enviarArquivo)
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
