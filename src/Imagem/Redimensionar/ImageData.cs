using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snebur.Imagens
{
    public interface ImageData
    {
        int width { get; set; }
        int height { get; set; }
        byte[] data { get; set; }
    }

    public class ImagemDataInstancia : ImageData
    {
        public int width { get; set; }
        public int height { get; set; }
        public byte[] data { get; set; }
    }
}
