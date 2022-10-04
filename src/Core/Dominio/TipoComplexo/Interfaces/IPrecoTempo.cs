namespace Snebur.Dominio
{
    public interface IPrecoTempo
    {
        double Prazo { get; set; }
        EnumTipoPrazo TipoPrazo { get; set; }
    }
}