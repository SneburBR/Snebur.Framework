namespace Snebur.Imagens;

public abstract class SobrePosicaoGradiente : SobrePosicao
{
    public required string Cor1 { get; set; }

    public required string Cor2 { get; set; }

    public required double LimiteCor1 { get; set; }

    public required double LimiteCor2 { get; set; }
}
