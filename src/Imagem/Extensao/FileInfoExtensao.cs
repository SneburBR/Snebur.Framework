using System.Windows.Media.Imaging;
using Snebur.Imagens;
using Snebur.Utilidade;

namespace System.IO;

public static class FileInfoExtensao
{
    public static async Task<BitmapSource?> RetornarMiniaturaAwait(this FileInfo arquivo, double alturaMaxima = ConstantesImagemApresentacao.ALTURA_IMAGEM_MINIATURA)
    {
        var miniatura = await Task.Factory.StartNew(() =>
        {
            return arquivo.RetornarMiniatura(alturaMaxima);
        });
        return miniatura;
    }

    public static BitmapSource? RetornarMiniatura(this FileInfo arquivo, double alturaMaxima = ConstantesImagemApresentacao.ALTURA_IMAGEM_PEQUENA)
    {
        return ImagemUtil.RetornarMiniatura(arquivo.FullName, alturaMaxima);
    }
}
