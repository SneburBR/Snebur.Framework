using System.Windows.Media.Imaging;
using Snebur.Utilidade;

namespace Snebur.Imagens;

public class MetadataUtil
{
    public const string QUERY_ORIENTACAO = "/app1/ifd/{ushort=274}";
    public const string QUERY_REPRESENTACAO_COR = "/app1/ifd/exif/{ushort=40961}";

    ///{ushort=40961}/

    public static int RetornarRotacao(BitmapMetadata metadata)
    {

        var orientacaoString = metadata.GetQuery(QUERY_ORIENTACAO)?.ToString();
        if (!String.IsNullOrEmpty(orientacaoString) && Int32.TryParse(orientacaoString, out var orientacao) && orientacao > 0)
        {
            switch (orientacao)
            {
                case 1:
                    return 0;
                case 2:
                    //return RotateFlipType.RotateNoneFlipX;
                    return 0;
                case 3:
                    //return RotateFlipType.Rotate180FlipNone;
                    return 180;
                case 4:
                    //return RotateFlipType.Rotate180FlipX;
                    return 180;
                case 5:
                    //return RotateFlipType.Rotate90FlipX;
                    return 90;
                case 6:
                    //return RotateFlipType.Rotate90FlipNone;
                    return 90;
                case 7:
                    //return RotateFlipType.Rotate270FlipX;
                    return 270;
                case 8:

                    //return RotateFlipType.Rotate270FlipNone;
                    return 270;

                default:

                    return 0;
            }
        }
        return 0;
    }

    public static int RetornarRotacao(int orientacaoExif)
    {
        switch (orientacaoExif)
        {
            case 1:
                return 0;
            case 2:
                //return RotateFlipType.RotateNoneFlipX;
                return 0;
            case 3:
                //return RotateFlipType.Rotate180FlipNone;
                return 180;
            case 4:
                //return RotateFlipType.Rotate180FlipX;
                return 180;
            case 5:
                //return RotateFlipType.Rotate90FlipX;
                return 90;
            case 6:
                //return RotateFlipType.Rotate90FlipNone;
                return 90;
            case 7:
                //return RotateFlipType.Rotate270FlipX;
                return 270;
            case 8:

                //return RotateFlipType.Rotate270FlipNone;
                return 270;

            default:

                return 0;
        }
    }

    public static BitmapMetadata? RetornarMetadata(BitmapFrame bitmapFrame, bool clonar)
    {
        if (SistemaUtil.IsWindowsXp)
        {
            return null;
        }

        if (bitmapFrame.Metadata is BitmapMetadata metadataOrigem)
        {
            try
            {
                if (clonar)
                {
                    return MetadataUtil.ClonarMetadata(metadataOrigem);
                }
                return (BitmapMetadata)bitmapFrame.Metadata;
            }
            catch (Exception)
            {
                return MetadataUtil.ClonarMetadata(metadataOrigem);
            }
        }
        return null;
    }

    public static BitmapMetadata? ClonarMetadata(BitmapMetadata? metadataOrigem)
    {
        if (metadataOrigem == null)
        {
            return null;
        }
        if (SistemaUtil.IsWindowsXp)
        {
            return null;
        }

        try
        {
            var metadata = new BitmapMetadata(metadataOrigem.Format);
            AutoMapearUtil.Mapear<BitmapMetadata>(metadataOrigem, metadata, true,
                                                  x => x.Format,
                                                  x => x.DateTaken,
                                                  x => x.IsFrozen);

            if (metadataOrigem.DateTaken != null)
            {
                try
                {
                    DateTime dateTakenCurrentCulture = DateTime.Parse(metadataOrigem.DateTaken, System.Threading.Thread.CurrentThread.CurrentCulture);
                    string dateTakenStringInvariantCultute = dateTakenCurrentCulture.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    metadata.DateTaken = dateTakenStringInvariantCultute;
                }
                catch
                {
                }
            }
            return metadata;
        }
        catch (Exception ex)
        {
            LogUtil.ErroAsync(new Exception("Não foi possivel clonar o metedata ", ex));
        }
        return null;
    }
}
