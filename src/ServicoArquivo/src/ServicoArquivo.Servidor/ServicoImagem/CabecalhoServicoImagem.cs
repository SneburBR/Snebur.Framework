#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Http;
using Snebur.Helpers;
#else
using System.Web;
#endif  

namespace Snebur.ServicoArquivo;

public class CabecalhoServicoImagem : CabecalhoServicoArquivo, IInformacaoRepositorioImagem
{
    public EnumTamanhoImagem TamanhoImagem { get; }

    public EnumFormatoImagem Formato { get; }

    public CabecalhoServicoImagem(HttpContext context, bool enviarArquivo = false) : base(context, enviarArquivo)
    {
        this.TamanhoImagem = (EnumTamanhoImagem)this.RetornarInteger(ConstantesServicoImagem.TAMANHO_IMAGEM);
        this.Formato = (EnumFormatoImagem)this.RetornarInteger(ConstantesServicoImagem.FORMATO_IMAGEM);

        if (!EnumHelpers.IsDefined(typeof(EnumFormatoImagem), this.Formato))
        {
            this.Formato = EnumFormatoImagem.JPEG;
        }
    }

    //#region IInformacaoRepositorioImagem

    //long IInformacaoRepositorioImagem.IdImagem => this.IdArquivo;

    //#endregion
}
