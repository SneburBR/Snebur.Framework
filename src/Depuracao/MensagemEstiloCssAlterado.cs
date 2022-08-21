
namespace Snebur.Depuracao
{
    public class MensagemEstiloCssAlterado : Mensagem
    {

		#region Campos Privados

        private string _nomeArquivo;

		#endregion

        public string NomeArquivo { get => this.RetornarValorPropriedade(this._nomeArquivo); set => this.NotificarValorPropriedadeAlterada(this._nomeArquivo, this._nomeArquivo = value); }


    }
}