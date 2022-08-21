using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Snebur.Imagem
{


    public enum EnumPerfilIcc
    {
        [Description("sRGB")]
        sRGB = 1,

        [Description("Adobe RGB (1998)")]
        AdobeRGB = 2,

        [Description("Apple RGB")]
        AppleRGB = 3,

        [Description("Color Match RGB")]
        ColorMatchRGB = 4,

        [Description("U.S Web Coated SWOP (CMYK)")]
        CmykUSWebCoatedSWOP = 100,

        [Description("Coated 39 (CMYK)")]
        CmykCoatedFOGRA39 = 101,

        [Description("Outro")]
        Outro = -99
    }
}
