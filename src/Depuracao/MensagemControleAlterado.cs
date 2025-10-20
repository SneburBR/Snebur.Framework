namespace Snebur.Depuracao
{
    public class MensagemControleAlterado : Mensagem
    {

        public bool IsScript { get; set; }

        public string UrlScriptRuntime { get; set; } = "";

        public string CaminhoConstrutor { get; set; } = "";

        public string NomeControle { get; set; } = "";
    }
}

