//using System;
//using System.Collections.Generic;
//using System.Drawing.Imaging;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace System.Drawing
//{
//    public static class BitmapExtensao
//    {
//        public static ImageFormat GetImageFormat(this Bitmap bitmap)
//        {
//            if (bitmap.RawFormat.Guid == new Guid("fb7e8e89-34f1-412f-981c-da70801f67ec"))
//            {
//                return ImageFormat.Png;
//            }
//            if (bitmap.RawFormat.Guid == ImageFormat.Png.Guid)
//            {
//                return ImageFormat.Jpeg;
//            }
//            //if (bitmap.RawFormat.Equals(ImageFormat.Jpeg))
//            //    return ImageFormat.Jpeg;
//            //if (bitmap.RawFormat.Equals(ImageFormat.Bmp))
//            //    return ImageFormat.Bmp;
//            //if (bitmap.RawFormat.Equals(ImageFormat.Png))
//            //    return ImageFormat.Png;
//            //if (bitmap.RawFormat.Equals(ImageFormat.Emf))
//            //    return ImageFormat.Emf;
//            //if (bitmap.RawFormat.Equals(ImageFormat.Exif))
//            //    return ImageFormat.Exif;
//            //if (bitmap.RawFormat.Equals(ImageFormat.Gif))
//            //    return ImageFormat.Gif;
//            //if (bitmap.RawFormat.Equals(ImageFormat.Icon))
//            //    return ImageFormat.Icon;
//            //if (bitmap.RawFormat.Equals(ImageFormat.MemoryBmp))
//            //    return ImageFormat.MemoryBmp;
//            //if (bitmap.RawFormat.Equals(ImageFormat.Tiff))
//            //    return ImageFormat.Tiff;

//            return ImageFormat.Jpeg;
//        }
//    }
//}
