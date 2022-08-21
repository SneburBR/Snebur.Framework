using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Snebur.Dominio;
using Snebur.Utilidade;

namespace Snebur.Imagem
{
    public class DimensaoImagemUtil
    {
        private const string errorMessage = "";

        public static Size RetornarDimensaoImagem(string caminhoArquivo, bool ignorarErro = true)
        {

            if (!ImagemUtil.IsExtensaoSuportada(caminhoArquivo))
            {
                return Size.Empty;
            }

            try
            {
                if (ImagemUtil.IsExtensaoJpeg(caminhoArquivo))
                {
                    var tamanho = DimensaoImagemUtil.RetornarDimensageoJpegBinary(caminhoArquivo);
                    if (!tamanho.HasValue || (tamanho.Value.Width <= 1 || tamanho.Value.Height == 1))
                    {
                        return DimensaoImagemUtil.RetornarDimensagemImagemComDecoder(caminhoArquivo, ignorarErro);
                    }
                    return tamanho.Value;
                }
                else
                {
                    return DimensaoImagemUtil.RetornarDimensagemImagemComDecoder(caminhoArquivo, ignorarErro);
                }
            }
            catch
            {
                return DimensaoImagemUtil.RetornarDimensagemImagemComDecoder(caminhoArquivo, ignorarErro);
            }
        }

        //private static Size RetornarDimensagemImagemMagick(string caminhoArquivo, bool ignorarErro = true)
        //{
        //    try
        //    {
        //        var mii = new MagickImageInfo(caminhoArquivo);
        //        if (mii.Width > 1 && mii.Height > 1)
        //        {
        //            return new Size(mii.Width, mii.Height);
        //        }
        //        else
        //        {
        //            return RetornarDimensagemImagemComDecoder(caminhoArquivo, ignorarErro);
        //        }
        //    }
        //    catch
        //    {
        //        return RetornarDimensagemImagemComDecoder(caminhoArquivo, ignorarErro);
        //    }
        //}

        private static Size RetornarDimensagemImagemComDecoder(string caminhoArquivo, bool ignorarErro = true)
        {
            try
            {
                using (var fs = StreamUtil.OpenRead(caminhoArquivo))
                {
                    var decoder = BitmapDecoder.Create(fs, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
                    var frame = decoder.Frames.First();
                    return new Size(frame.PixelWidth, frame.PixelHeight);

                }
            }
            catch
            {
                return RetornarDimensagemImagemComDrawing(caminhoArquivo, ignorarErro);
            }
        }

        private static Size RetornarDimensagemImagemComDrawing(string caminhoArquivo, bool ignorarErro = true)
        {
            try
            {
                using (var imagem = System.Drawing.Image.FromFile(caminhoArquivo))
                {
                    return new Size((double)imagem.Width, (double)imagem.Height);
                }
            }
            catch (Exception ex)
            {
                if (!ignorarErro)
                {
                    throw new ErroDimensaoImagem("Ñão foi possivel obter a dimensão da imagem", ex);
                }
                return new Size(0, 0);
            }
        }

        private static Size? RetornarDimensageoJpegBinary(string caminhoArquivo)
        {
            using (var fs = StreamUtil.OpenRead(caminhoArquivo))
            {
                using (var binaryReader = new BinaryReader(fs))
                {
                    try
                    {
                        return RetornarDimensageoJpegBinary(fs, binaryReader);
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }

        private static Size? RetornarDimensageoJpegBinary(FileStream stream, BinaryReader rdr)
        {

            // keep reading packets until we find one that contains Size info
            for (;;)
            {
                byte code = rdr.ReadByte();
                if (code != 0xFF)
                {
                    //throw new Exception("Unexpected value in file");
                    return null;

                }

                code = rdr.ReadByte();
                switch (code)
                {
                    // filler byte
                    case 0xFF:

                        stream.Position--;
                        break;
                    // packets without data
                    case 0xD0:
                    case 0xD1:
                    case 0xD2:
                    case 0xD3:
                    case 0xD4:
                    case 0xD5:
                    case 0xD6:
                    case 0xD7:
                    case 0xD8:
                    case 0xD9:

                        break;
                    // packets with size information
                    case 0xC0:
                    case 0xC1:
                    case 0xC2:
                    case 0xC3:
                    case 0xC4:
                    case 0xC5:
                    case 0xC6:
                    case 0xC7:
                    case 0xC8:
                    case 0xC9:
                    case 0xCA:
                    case 0xCB:
                    case 0xCC:
                    case 0xCD:
                    case 0xCE:
                    case 0xCF:

                        ReadBEUshort(rdr);
                        rdr.ReadByte();
                        ushort h = ReadBEUshort(rdr);
                        ushort w = ReadBEUshort(rdr);
                        return new Size(w, h);
                    // irrelevant variable-length packets
                    default:

                        int len = ReadBEUshort(rdr);
                        stream.Position += len - 2;
                        break;
                }
            }
        }

        private static ushort ReadBEUshort(BinaryReader rdr)
        {
            ushort hi = rdr.ReadByte();
            hi <<= 8;
            ushort lo = rdr.ReadByte();
            return (ushort)(hi | lo);
        }

        private static Size RetornarDimensaoImagem(BinaryReader binaryReader)
        {
            int maxMagicBytesLength = ImagemFormatDecoders.Keys.OrderByDescending(x => x.Length).First().Length;

            byte[] magicBytes = new byte[maxMagicBytesLength];

            for (int i = 0; i < maxMagicBytesLength; i += 1)
            {
                magicBytes[i] = binaryReader.ReadByte();

                foreach (var kvPair in ImagemFormatDecoders)
                {
                    if (magicBytes.StartsWith(kvPair.Key))
                    {
                        return kvPair.Value(binaryReader);
                    }
                }
            }

            throw new ArgumentException(errorMessage, "binaryReader");
        }

        private static Dictionary<byte[], Func<BinaryReader, Size>> ImagemFormatDecoders = new Dictionary<byte[], Func<BinaryReader, Size>>()
        {
            { new byte[]{ 0x42, 0x4D }, DecodeBitmap},
            { new byte[]{ 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }, DecodeGif },
            { new byte[]{ 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }, DecodeGif },
            { new byte[]{ 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, DecodePng },
            { new byte[]{ 0xff, 0xd8 }, DecodeJfif },
        };

        private static Size DecodeBitmap(BinaryReader binaryReader)
        {
            binaryReader.ReadBytes(16);
            int width = binaryReader.ReadInt32();
            int height = binaryReader.ReadInt32();
            return new Size(width, height);
        }

        private static Size DecodeGif(BinaryReader binaryReader)
        {
            int width = binaryReader.ReadInt16();
            int height = binaryReader.ReadInt16();
            return new Size(width, height);
        }

        private static Size DecodePng(BinaryReader binaryReader)
        {
            binaryReader.ReadBytes(8);
            int width = binaryReader.ReadLittleEndianInt32();
            int height = binaryReader.ReadLittleEndianInt32();
            return new Size(width, height);
        }

        private static Size DecodeJfif(BinaryReader binaryReader)
        {
            while (binaryReader.ReadByte() == 0xff)
            {
                byte marker = binaryReader.ReadByte();
                short chunkLength = binaryReader.ReadLittleEndianInt16();

                if (marker == 0xc0)
                {
                    binaryReader.ReadByte();

                    int height = binaryReader.ReadLittleEndianInt16();
                    int width = binaryReader.ReadLittleEndianInt16();
                    return new Size(width, height);
                }

                binaryReader.ReadBytes(chunkLength - 2);
            }

            throw new ArgumentException(errorMessage);
        }

        public static EnumOrientacao RetornarOrientacao(double largura, double altura)
        {
            return RetornarOrientacao(new Size(largura, altura));
        }

        public static EnumOrientacao RetornarOrientacao(Size tamanho)
        {
            if(tamanho.Width > tamanho.Height)
            {
                return EnumOrientacao.Horizontal;
            }
            if(tamanho.Height > tamanho.Width)
            {
                return EnumOrientacao.Vertical;
            }
            return EnumOrientacao.Quadrado;
        }

    }
 
}
