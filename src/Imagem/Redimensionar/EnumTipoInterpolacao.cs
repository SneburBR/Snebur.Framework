using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snebur.Imagem
{
    public enum EnumTipoInterpolacao
    {
        Triangle = 5,
        Hermite = 6,
        Bell = 7,
        QuadraticBSpline = 8,
        CubicBSpline = 9,
        BoxFilter = 10,
        Lanczos = 11,
        Michell = 12,
        Cosine = 13,
        Catrom = 14,
        Quadratic = 15,
        CubicConvolution = 16,
        Bilinear = 17,
        Bresenham = 18
    }
}
