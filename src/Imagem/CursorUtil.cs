using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Snebur.Imagens;

public class CursorUtil
{

    public static Cursor CriarCursor(double raio)
    {
        using (var msPng = CursorUtil.RetornarStreamCursorPng(raio))
        {
            using (var msCursor = CursorUtil.ConverterPngToCursor(msPng, raio))
            {
                var cursor = new Cursor(msCursor);
                return cursor;
            }
        }
    }

    public static MemoryStream RetornarStreamCursorPng(double raio)
    {
        var vis = new DrawingVisual();
        using (var dc = vis.RenderOpen())
        {
            var canetaPontilhada = new Pen(Brushes.White, 2);
            canetaPontilhada.DashStyle = DashStyles.Dot;

            dc.DrawEllipse(Brushes.Transparent, canetaPontilhada, new Point(raio + 1, raio + 1), raio, raio);

            // var cinza = new SolidColorBrush(Color.FromArgb(50, 50, 50, 50));

            var caneta = new Pen(Brushes.Black, 1);

            //Linha vertical
            dc.DrawLine(caneta, new Point(raio, 0), new Point(raio, 2 * raio));
            dc.DrawLine(caneta, new Point(0, raio), new Point(2 * raio, raio));

            // dc.DrawRectangle(,, new Rect(0, 0, raio, raio));
            //dc.DrawRectangle(Brushes.Gold, new Pen(Brushes.Black, 0.1), new Rect(0, 0, 50, 50));
            dc.Close();
        }

        var rtb = new RenderTargetBitmap(64, 64, 96, 96, PixelFormats.Pbgra32);
        rtb.Render(vis);

        var ms = new MemoryStream();
        var penc = new PngBitmapEncoder();
        penc.Frames.Add(BitmapFrame.Create(rtb));
        penc.Save(ms);
        return ms;
    }

    public static MemoryStream ConverterPngToCursor(MemoryStream msPng, double raio)
    {
        var pngBytes = msPng.ToArray();
        var size = pngBytes.GetLength(0);
        var ms = new MemoryStream();

        ms.Write(BitConverter.GetBytes((Int16)0), 0, 2);
        ms.Write(BitConverter.GetBytes((Int16)2), 0, 2);
        ms.Write(BitConverter.GetBytes((Int16)1), 0, 2);

        ms.WriteByte(32);
        ms.WriteByte(32);

        ms.WriteByte(0);
        ms.WriteByte(0);

        ms.Write(BitConverter.GetBytes((Int16)(raio / 2.0)), 0, 2);
        ms.Write(BitConverter.GetBytes((Int16)(raio / 2.0)), 0, 2);

        ms.Write(BitConverter.GetBytes(size), 0, 4);
        ms.Write(BitConverter.GetBytes((Int32)22), 0, 4);

        ms.Write(pngBytes, 0, size);
        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }
}
