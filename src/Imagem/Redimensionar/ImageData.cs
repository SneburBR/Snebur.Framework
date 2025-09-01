namespace Snebur.Imagens;

public interface ImageData
{
    int width { get; }
    int height { get; }
    byte[] data { get; }
}

public class ImagemDataInstancia : ImageData
{
    public required int width { get; init; }
    public required int height { get; init; }
    public required byte[] data { get; init; }
}
