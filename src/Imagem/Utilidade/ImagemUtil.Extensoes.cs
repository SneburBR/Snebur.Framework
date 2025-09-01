using System.IO;
using System.Windows.Media.Imaging;
using Snebur.Dominio;
using Snebur.Imagens;

namespace Snebur.Utilidade;

public partial class ImagemUtil
{

#pragma warning disable IDE0032
    private static List<string>? _extensoes;
#pragma warning restore IDE0032  

    private static object _bloqueio = new object();
    public static List<string> ExtensoesSuportadas
    {
        get
        {

            if (_extensoes == null)
            {
                lock (_bloqueio)
                {
                    if (_extensoes == null)
                    {
                        _extensoes = new List<string>();

                        _extensoes.AddRange(ExtensoesJpeg);
                        _extensoes.AddRange(ExtensoesTif);
                        _extensoes.AddRange(ExtensoesBmp);
                        _extensoes.AddRange(ExtensoesPng);
                        _extensoes.AddRange(ExtensoesIco);
                        _extensoes.AddRange(ExtensoesGif);

                        if (!PngResolver.UsarPng)
                        {
                            foreach (var extensaoPng in ExtensoesPng)
                            {
                                if (_extensoes.Contains(extensaoPng))
                                {
                                    _extensoes.Remove(extensaoPng);
                                }
                            }
                        }
                    }
                }
            }
            return _extensoes;
        }
    }

    public static List<string> ExtensoesJpeg = new List<string> {
        ".jpg", ".jpeg", "Jfif", "jif", "jpe" };

    public static EnumFormatoImagem RetornarFormatoArquivo(string caminhoArquivo)
    {
        using (var fs = StreamUtil.OpenRead(caminhoArquivo))
        {
            return RetornarFormatoStream(fs);
        }
    }

    public static EnumFormatoImagem RetornarFormatoStream(Stream stream)
    {
        if (stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }

        try
        {
            var encoder = BitmapDecoder.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
            switch (encoder)
            {
                case JpegBitmapDecoder jpeg:

                    return EnumFormatoImagem.JPEG;

                case BmpBitmapDecoder bmp:

                    return EnumFormatoImagem.BMP;

                case PngBitmapDecoder png:

                    return EnumFormatoImagem.PNG;

                case GifBitmapDecoder gif:

                    return EnumFormatoImagem.GIF;

                case TiffBitmapDecoder tif:

                    return EnumFormatoImagem.TIFF;

                case IconBitmapDecoder icon:

                    return EnumFormatoImagem.ICO;

                default:

                    return EnumFormatoImagem.Desconhecido;
            }
        }
        catch
        {
            return EnumFormatoImagem.Desconhecido;
        }
    }

    public static EnumFormatoImagem RetornarFormatoExtensao(string caminhoArquivo)
    {
        return RetornarFormatoExtensao(new FileInfo(caminhoArquivo));

    }

    public static EnumFormatoImagem RetornarFormatoExtensao(FileInfo arquivo)
    {
        if (ImagemUtil.IsExtensaoJpeg(arquivo))
        {
            return EnumFormatoImagem.JPEG;
        }

        if (ImagemUtil.IsExtensaoPng(arquivo))
        {
            return EnumFormatoImagem.PNG;
        }

        if (ImagemUtil.IsExtensaoTif(arquivo))
        {
            return EnumFormatoImagem.TIFF;
        }

        if (ImagemUtil.IsExtensaoGif(arquivo))
        {
            return EnumFormatoImagem.GIF;
        }

        if (ImagemUtil.IsExtensaoIco(arquivo))
        {
            return EnumFormatoImagem.ICO;
        }

        if (ImagemUtil.IsExtensaoBmp(arquivo))
        {
            return EnumFormatoImagem.BMP;
        }

        return EnumFormatoImagem.Desconhecido;

    }

    public static List<string> ExtensoesTif = new List<string> { ".tif", ".tiff" };

    public static List<string> ExtensoesBmp = new List<string> { ".bmp", "dib", "rle" };

    public static List<string> ExtensoesGif = new List<string> { ".gif", "giff" };

    public static List<string> ExtensoesPng = new List<string> { ".png", "pns" };

    public static List<string> ExtensoesIco = new List<string> { ".ico" };

    public static List<string> ExtensoesPsd = new List<string> { ".psd" };

    public static bool IsExtensaoJpeg(string caminhoArquivo)
    {
        return IsExtensaoJpeg(new FileInfo(caminhoArquivo));
    }

    public static bool IsExtensaoJpeg(FileInfo arquivo)
    {
        return ImagemUtil.ExtensoesJpeg.Contains(arquivo.Extension, new IgnorarCasoSensivel());
    }

    public static bool IsExtensaoBmp(string caminhoArquivo)
    {
        return IsExtensaoBmp(new FileInfo(caminhoArquivo));
    }

    public static bool IsExtensaoBmp(FileInfo arquivo)
    {
        return ImagemUtil.ExtensoesBmp.Contains(arquivo.Extension, new IgnorarCasoSensivel());
    }

    public static bool IsExtensaoTif(string caminhoArquivo)
    {
        return IsExtensaoTif(new FileInfo(caminhoArquivo));
    }

    public static bool IsExtensaoTif(FileInfo arquivo)
    {
        return ImagemUtil.ExtensoesTif.Contains(arquivo.Extension, new IgnorarCasoSensivel());
    }

    public static bool IsExtensaoPng(string caminhoArquivo)
    {
        return IsExtensaoPng(new FileInfo(caminhoArquivo));
    }

    public static bool IsExtensaoPng(FileInfo arquivo)
    {
        return ImagemUtil.ExtensoesPng.Contains(arquivo.Extension, new IgnorarCasoSensivel());
    }

    public static bool IsExtensaoGif(string caminhoArquivo)
    {
        return IsExtensaoGif(new FileInfo(caminhoArquivo));
    }

    public static bool IsExtensaoGif(FileInfo fi)
    {
        return ImagemUtil.ExtensoesGif.Contains(fi.Extension, new IgnorarCasoSensivel());
    }

    public static bool IsExtensaoIco(string caminhoArquivo)
    {
        return IsExtensaoIco(new FileInfo(caminhoArquivo));
    }

    public static bool IsExtensaoIco(FileInfo arquivo)
    {
        return ImagemUtil.ExtensoesIco.Contains(arquivo.Extension, new IgnorarCasoSensivel());
    }

    public static bool IsExtensaoSuportada(string caminhoArquivo)
    {
        return IsExtensaoSuportada(new FileInfo(caminhoArquivo));
    }

    public static bool IsExtensaoSuportada(FileInfo arquivo)
    {
        return IsExtensaoExtensaoSuportada(arquivo.Extension);
    }

    public static bool IsExtensaoExtensaoSuportada(string extensao)
    {
        Guard.NotNull(extensao);
        if (!extensao.StartsWith("."))
        {
            extensao = "." + extensao;
        }
        return ImagemUtil.ExtensoesSuportadas.Contains(extensao, new IgnorarCasoSensivel());
    }

    public static string RetornarExtensao(EnumFormatoImagem formato)
    {
        switch (formato)
        {
            case EnumFormatoImagem.JPEG:
                return ".jpg";
            case EnumFormatoImagem.BMP:
                return ".bmp";
            case EnumFormatoImagem.PNG:
                return ".png";
            case EnumFormatoImagem.TIFF:
                return ".tif";
            case EnumFormatoImagem.ICO:
                return ".ico";
            case EnumFormatoImagem.GIF:
                return ".gif";
            default:

                throw new Exception("Formato não suportado");
        }
    }
}
