namespace Snebur.Depuracao
{
    public class MensagemLog : Mensagem
    {
        public string Mensagem { get; set; } = "";

        public EnumTipoLog TipoLog { get; set; }
    }
}
