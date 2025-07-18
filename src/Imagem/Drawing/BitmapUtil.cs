using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace Snebur.Imagens
{
    public class BitmapUtil
    {
        public static Bitmap RetornarBitmap(string caminhoArquivo)
        {
            var bitmapImage = new BitmapImage();
            var ms = new MemoryStream(File.ReadAllBytes(caminhoArquivo));
            return new Bitmap(ms);
        }

        public static Bitmap RecortarImagem(Bitmap imagem, int x, int y, int largura, int altura)
        {
            var imagemRecortada = new Bitmap(largura, altura);
            using (var g = Graphics.FromImage(imagemRecortada))
            {
                g.DrawImage(imagem, new Rectangle(0, 0, largura, altura), x, y, largura, altura, GraphicsUnit.Pixel);
            }
            return imagemRecortada;
        }

        public static ImageCodecInfo RetornarEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public static Bitmap RedimencioanrImagem(Bitmap imagem, int largura, int altura)
        {
            var novaIamgem = new Bitmap(largura, altura);
            using (var g = Graphics.FromImage(novaIamgem))
            {
                g.InterpolationMode = InterpolationMode.Bicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(imagem, 0, 0, largura, altura);
            }
            return novaIamgem;
        }

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            var jpgEncoder = BitmapUtil.RetornarEncoder(ImageFormat.Jpeg);
            var myEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            var ms = new MemoryStream();
            bitmap.Save(ms, jpgEncoder, myEncoderParameters);
            ms.Seek(0, SeekOrigin.Begin);

            var bitmapImagem = new BitmapImage();
            bitmapImagem.BeginInit();
            bitmapImagem.StreamSource = ms;
            bitmapImagem.EndInit();
            return bitmapImagem;
        }

        public static void SalvarBitmap(string caminhoArquivo, Bitmap bitmap, int qualidade)
        {
            using (var ms = SalvarBitmap(bitmap, qualidade))
            {
                File.WriteAllBytes(caminhoArquivo, ms.ToArray());
            }
        }

        public static MemoryStream SalvarBitmap(Bitmap bitmap, int qualidade)
        {
            var jpgEncoder = BitmapUtil.RetornarEncoder(ImageFormat.Jpeg);
            var myEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            var myEncoderParameter = new EncoderParameter(myEncoder, qualidade);
            myEncoderParameters.Param[0] = myEncoderParameter;

            var ms = new MemoryStream();
            bitmap.Save(ms, jpgEncoder, myEncoderParameters);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        internal static MemoryStream RetornarStreamImagemVazia()
        {
            return BitmapUtil.SalvarBitmap(new System.Drawing.Bitmap(1, 1), 1);
       }
    }
}
