using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Snebur.Imagens
{
    internal class RedimensionarInterpolacaoIamgem
    {
        private string _perspective = "TOP_LEFT";
        private object _progressData = null;

        internal BitmapSource RedimensionarInterno(BitmapSource imagem, int largura, int altura, EnumTipoInterpolacao tipoInterpolacao)
        {
    
            var imagemData = new ImagemDataInstancia()
            {
                data = imagem.GetPixels(EnumPixelFormato.Rgba),
                width = imagem.PixelWidth,
                height = imagem.PixelHeight
            };

            var imageDataDestino = this.RetornarPixelsRedimensionado(imagemData, largura, altura, tipoInterpolacao);
            var pixelsBgra32 = PixelsConverterUtil.RgbaParaBgra(imageDataDestino.data, largura, altura);
            var stride = largura * 4;
            return BitmapSource.Create(largura, altura, imagem.DpiX, imagem.DpiY, PixelFormats.Bgra32, null, pixelsBgra32, stride);

        }

        internal  ImageData RetornarPixelsRedimensionado(ImageData origem, int largura, int altura, EnumTipoInterpolacao tipoInterpolacao)
        {

            if (0 >= largura || 0 >= altura)
            {
                throw new Exception("A largura e altura devem ser maior q zero");
            }

            if (largura == origem.width && altura == origem.height)
            {
                return origem;
            }

            switch (tipoInterpolacao)
            {
                case EnumTipoInterpolacao.Triangle:
                case EnumTipoInterpolacao.Hermite:

                case EnumTipoInterpolacao.Bell:
                case EnumTipoInterpolacao.QuadraticBSpline:
                case EnumTipoInterpolacao.CubicBSpline:
                case EnumTipoInterpolacao.BoxFilter:
                    //e = MiddleSpeedFilters(a, c, b, d, f);
                    //break;
                    throw new Exception("Não implementado");

                case EnumTipoInterpolacao.Lanczos:
                case EnumTipoInterpolacao.Michell:
                case EnumTipoInterpolacao.Cosine:
                case EnumTipoInterpolacao.Catrom:
                case EnumTipoInterpolacao.Quadratic:
                case EnumTipoInterpolacao.CubicConvolution:

                    return this.RetornarPixelsRedimensionadoAltaQualidade(origem, largura, altura, tipoInterpolacao);

                case EnumTipoInterpolacao.Bilinear:

                    //e = BilinearResizeBitmap(a, c, b, d);
                    //break;

                    throw new Exception("Não implementado - bilenear");

                case EnumTipoInterpolacao.Bresenham:

                    //e = BresenhamResizeBitmap(a, c, b, d);
                    //break;
                    throw new Exception("Não implementado - Bresenham");

                default:

                    throw new Exception("Tipo de interpolação não suportado");
            }
        }

        private ImageData RetornarPixelsRedimensionadoAltaQualidade(ImageData origem, int largura, int altura, EnumTipoInterpolacao tipoInterpolacao)
        {
            //var scalarX: number = null;
            //var scalarY: number = null;
            var i = 0;

            var resultado = new ImagemDataInstancia();
            resultado.data = this.RetornarArrayBytes(4 * largura * altura);
            resultado.width = largura;
            resultado.height = altura;

            var alturaNova = 1;
            var pixelsRedimensionado = this.RetornarArrayBytes(3 * origem.width + 12);

            //Definido o alpha como 255;
            for (i = 0; i < (largura * altura); i++)
            {
                resultado.data[4 * i + 3] = 255;
            }

            var alturaOriginal = origem.height;
            var larguraOriginal = origem.width;
            var diminuir = false;

            var larguraRedimensionada_3x = 3 * resultado.width;
            var coficienteInterpolacao = 1.0;

            switch (tipoInterpolacao)
            {
                case EnumTipoInterpolacao.Lanczos:
                    coficienteInterpolacao = 3;
                    break;
                case EnumTipoInterpolacao.Michell:
                    coficienteInterpolacao = 2;
                    break;
                case EnumTipoInterpolacao.Cosine:
                    coficienteInterpolacao = 1;
                    break;
                case EnumTipoInterpolacao.Catrom:
                    coficienteInterpolacao = 2;
                    break;
                case EnumTipoInterpolacao.Quadratic:
                    coficienteInterpolacao = 1.5;
                    break;
                case EnumTipoInterpolacao.CubicConvolution:
                    coficienteInterpolacao = 3;
                    break;
            }

            var scalarX = largura / this.ParseFloat(larguraOriginal);
            var scalarY = altura / this.ParseFloat(alturaOriginal);

            if (1 <= scalarY)
            {
                alturaNova = this.ParseInt(2 * coficienteInterpolacao + 2);
            }
            else
            {
                alturaNova = this.ParseInt(2 * (coficienteInterpolacao / scalarY) + 3);
                diminuir = true;
            }

            if (alturaNova > alturaOriginal)
            {
                alturaNova = alturaOriginal;
            }

            var array3x = this.RetornarArrayBytes(larguraRedimensionada_3x * alturaNova);

            var arrayPIndex = new pIndexIntancia[alturaNova];

            for (i = 0; i < alturaNova; i++)
            {
                arrayPIndex[i] = new pIndexIntancia();
            }


            //resultadoImagem = larguraRedimensionar;

            var array32Horizontal1 = this.RetornarArrayInt32(largura);
            var array32Horizontal2 = this.RetornarArrayInt32(largura);

            //resultadoImagem = alturaRedimensionar;

            var array32Vertical_1 = this.RetornarArrayInt32(altura);
            var array32Vertical_2 = this.RetornarArrayInt32(altura);
            var array32Vertical_3 = this.RetornarArrayInt32(altura);

            var larguraMaxima = 1 <= scalarX ? this.ParseInt(2 * coficienteInterpolacao + 2) : this.ParseInt(2 * (coficienteInterpolacao / scalarX) + 2);

            if (larguraMaxima > larguraOriginal)
            {
                larguraMaxima = larguraOriginal;
            }

            var array32_duplo_horizontal = this.RetornarArrayInt32(2 * larguraMaxima * largura);
            var array32_duplo_vertical = this.RetornarArrayInt32(2 * alturaNova * altura);

            //resultadoImagem = larguraRedimensionada_3x;

            var arrayFinal = this.RetornarArrayBytes(larguraRedimensionada_3x);
            var alturaNova2x = 2 * alturaNova;
            larguraMaxima *= 2;

            var alturaNovaMenus_1 = alturaNova - 1;

            if (1 > scalarY)
            {
                alturaNova--;
            }

            scalarY = alturaNova - 1;

            this.SetPixelsData(largura, 3, larguraMaxima, larguraOriginal, false, 0,
                tipoInterpolacao, coficienteInterpolacao, array32Horizontal1, array32Horizontal2, null, array32_duplo_horizontal, alturaNovaMenus_1);

            this.SetPixelsData(altura, 3, alturaNova2x, alturaOriginal, true, scalarY, tipoInterpolacao,
                coficienteInterpolacao, array32Vertical_1, array32Vertical_2, array32Vertical_3,
                array32_duplo_vertical, alturaNovaMenus_1);

            larguraOriginal = 0;
            coficienteInterpolacao = alturaNova;



            for (i = 0; i < coficienteInterpolacao && i < alturaOriginal; i++)
            {

                this.L_ImageProcessGet(origem, pixelsRedimensionado, i);

                this.ColorSlowSpeedLineInterpolate8(pixelsRedimensionado,
                    array3x, larguraOriginal, array32Horizontal1,
                    array32Horizontal2, array32_duplo_horizontal,
                    larguraMaxima,
                    largura, 3);

                arrayPIndex[i].pIndex = larguraOriginal;
                larguraOriginal += larguraRedimensionada_3x;
            }

            if (diminuir)
            {
                arrayPIndex[alturaNovaMenus_1].pIndex = larguraOriginal;
                this.L_CopyArrayRange(array3x, arrayPIndex[0].pIndex, array3x, arrayPIndex[alturaNovaMenus_1].pIndex, larguraRedimensionada_3x);
            }

            //y && 
            larguraOriginal = array32Vertical_3[0];

            var intCoficienteInterpolacao = this.ParseInt(coficienteInterpolacao);

            for (i = larguraRedimensionada_3x = alturaOriginal = 0; i < altura; i++)
            {
                alturaNova = array32Vertical_3[larguraRedimensionada_3x];

                if (alturaNova != larguraOriginal)

                    for (larguraOriginal = intCoficienteInterpolacao; larguraOriginal <= alturaNova; larguraOriginal++)
                    {

                        intCoficienteInterpolacao++;

                        if (diminuir)
                        {
                            this.L_CopyArrayRange(array3x, arrayPIndex[0].pIndex, array3x, arrayPIndex[alturaNovaMenus_1].pIndex, larguraRedimensionada_3x);
                        }
                        //y && y &&

                        var k = 0;
                        for (k = 0; k < scalarY; k++)
                        {

                            var k_1 = k + 1;
                            var k_pIndex = arrayPIndex[k].pIndex;

                            arrayPIndex[k].pIndex = arrayPIndex[k_1].pIndex;
                            arrayPIndex[k_1].pIndex = k_pIndex;
                            this.L_ImageProcessGet(origem, pixelsRedimensionado, larguraOriginal);
                        }

                        this.ColorSlowSpeedLineInterpolate8(pixelsRedimensionado, array3x, arrayPIndex[k].pIndex, array32Horizontal1, array32Horizontal2, array32_duplo_horizontal, larguraMaxima, largura, 3);
                    }

                this.ColorVerticalInterpolate8(array3x, arrayPIndex, arrayFinal, array32Vertical_1, array32Vertical_2, array32_duplo_vertical, alturaOriginal, i, largura, 3);

                this.L_ImageProcessPut(resultado, arrayFinal, i);

                larguraOriginal = alturaNova;
                alturaOriginal += alturaNova2x;
                larguraRedimensionada_3x++;
            }
            return resultado;
        }

        internal ImageData RetornarPixelsRedimensionadoLentoBackup(ImageData origem, int largura, int altura, EnumTipoInterpolacao tipoInterpolacao)
        {
            var resultado = new ImagemDataInstancia();
            resultado.data = this.RetornarArrayBytes(4 * largura * altura);
            resultado.width = largura;
            resultado.height = altura;

            //Definido o alpha como 255;
            var i = 0;
            for (i = 0; i < (largura * altura); i++)
            {
                resultado.data[4 * i + 3] = 255;
            }

            int alturaOrigem = origem.height;
            int larguraOrigem = origem.width;
            var diminuir = false;

            int alturaNova = 1;
            int alturaNova2x = 1;

            var pixelsRedimensionado = this.RetornarArrayBytes(3 * origem.width + 12);
            var larguraRedimensionada_3x = 3 * resultado.width;

            double coficienteInterpolacao = 1;

            switch (tipoInterpolacao)
            {
                case EnumTipoInterpolacao.Lanczos:
                    coficienteInterpolacao = 3;
                    break;
                case EnumTipoInterpolacao.Michell:
                    coficienteInterpolacao = 2;
                    break;
                case EnumTipoInterpolacao.Cosine:
                    coficienteInterpolacao = 1;
                    break;
                case EnumTipoInterpolacao.Catrom:
                    coficienteInterpolacao = 2;
                    break;
                case EnumTipoInterpolacao.Quadratic:
                    coficienteInterpolacao = 1.5;
                    break;
                case EnumTipoInterpolacao.CubicConvolution:
                    coficienteInterpolacao = 3;
                    break;
            }

            var scalarX = largura / (double)larguraOrigem;
            var scalarY = altura / (double)alturaOrigem;

            if (1 <= scalarY)
            {
                alturaNova = this.ParseInt(2 * coficienteInterpolacao + 2);
            }
            else
            {
                alturaNova = this.ParseInt(ParseInt(2 * (coficienteInterpolacao / scalarY) + 3));
                diminuir = true;
            }

            if (alturaNova > alturaOrigem)
            {
                alturaNova = alturaOrigem;
            }

            var array3x = RetornarArrayBytes(larguraRedimensionada_3x * alturaNova);

            var arrayPIndex = new IpIndex[this.ParseInt(alturaNova)];

            for (i = 0; i < alturaNova; i++)
            {
                arrayPIndex[i] = new pIndexIntancia();
            }


            //resultadoImagem = larguraRedimensionar;

            var array32Horizontal1 = this.RetornarArrayInt32(largura);
            var array32Horizontal2 = this.RetornarArrayInt32(largura);

            //resultadoImagem = alturaRedimensionar;

            var array32Vertical_1 = this.RetornarArrayInt32(altura);
            var array32Vertical_2 = this.RetornarArrayInt32(altura);
            var array32Vertical_3 = this.RetornarArrayInt32(altura);

            int larguraMaxima = (1 <= scalarX) ? this.ParseInt(2 * coficienteInterpolacao + 2) :
                                                 this.ParseInt(2 * (coficienteInterpolacao / scalarX) + 2);

            if (larguraMaxima > larguraOrigem)
            {
                larguraMaxima = larguraOrigem;
            }

            var array32_duplo_horizontal = this.RetornarArrayInt32(2 * larguraMaxima * largura);
            var array32_duplo_vertical = this.RetornarArrayInt32(2 * alturaNova * altura);

            //resultadoImagem = larguraRedimensionada_3x;

            var arrayFinal = RetornarArrayBytes(larguraRedimensionada_3x);
            alturaNova2x = 2 * alturaNova;
            larguraMaxima *= 2;

            var alturaNovaMenus_1 = alturaNova - 1;

            if (1 > scalarY) alturaNova--;
            scalarY = alturaNova - 1;

            this.SetPixelsData(largura, 3, larguraMaxima, larguraOrigem, false, 0,
                 tipoInterpolacao, this.ParseInt(coficienteInterpolacao),
                 array32Horizontal1, array32Horizontal2, null, array32_duplo_horizontal, alturaNovaMenus_1);

            this.SetPixelsData(altura, 3, alturaNova2x, alturaOrigem, true, this.ParseInt(scalarY), tipoInterpolacao,
                this.ParseInt(coficienteInterpolacao), array32Vertical_1, array32Vertical_2, array32Vertical_3,
                array32_duplo_vertical, alturaNovaMenus_1);

            larguraOrigem = 0;
            coficienteInterpolacao = alturaNova;

            //var i = 0;
            for (i = 0; Convert.ToInt32(i) < coficienteInterpolacao && Convert.ToInt32(i) < alturaOrigem; i++)
            {

                L_ImageProcessGet(origem, pixelsRedimensionado, i);

                ColorSlowSpeedLineInterpolate8(pixelsRedimensionado, array3x, larguraOrigem, array32Horizontal1, array32Horizontal2, array32_duplo_horizontal,
                    larguraMaxima, largura, 3);

                arrayPIndex[i].pIndex = this.ParseInt(larguraOrigem);
                larguraOrigem += larguraRedimensionada_3x;
            }

            if (diminuir)
            {

                arrayPIndex[this.ParseInt(alturaNovaMenus_1)].pIndex = this.ParseInt(larguraOrigem);
                this.L_CopyArrayRange(array3x, arrayPIndex[0].pIndex, array3x, arrayPIndex[this.ParseInt(alturaNovaMenus_1)].pIndex, larguraRedimensionada_3x);
            }

            //y && 
            larguraOrigem = array32Vertical_3[0];


            for (i = larguraRedimensionada_3x = alturaOrigem = 0; i < altura; i++)
            {
                alturaNova = array32Vertical_3[(int)larguraRedimensionada_3x];

                if (alturaNova != larguraOrigem)

                    for (var j = this.ParseInt(coficienteInterpolacao); j <= alturaNova; j++)
                    {

                        coficienteInterpolacao++;

                        if (diminuir)
                        {
                            L_CopyArrayRange(array3x, arrayPIndex[0].pIndex, array3x, arrayPIndex[this.ParseInt(alturaNovaMenus_1)].pIndex, larguraRedimensionada_3x);
                        }
                        //y && y &&

                        int k = 0;
                        for (k = 0; k < scalarY; k++)
                        {

                            var k_1 = k + 1;
                            var k_pIndex = arrayPIndex[k].pIndex;

                            arrayPIndex[k].pIndex = arrayPIndex[k_1].pIndex;
                            arrayPIndex[k_1].pIndex = k_pIndex;

                            this.L_ImageProcessGet(origem, pixelsRedimensionado, j);
                        }

                        this.ColorSlowSpeedLineInterpolate8(pixelsRedimensionado, array3x, arrayPIndex[k].pIndex, array32Horizontal1, array32Horizontal2, array32_duplo_horizontal, larguraMaxima, largura, 3);
                        //this.ColorSlowSpeedLineInterpolate8(pixelsRedimensionado, array3x, arrayPIndex[k].pIndex, array32Horizontal1, array32Horizontal2, array32_duplo_horizontal, larguraMaxima, larguraRedimensionar, 3)
                    }

                this.ColorVerticalInterpolate8(array3x, arrayPIndex, arrayFinal, array32Vertical_1, array32Vertical_2, array32_duplo_vertical, alturaOrigem, i, largura, 3);

                this.L_ImageProcessPut(resultado, arrayFinal, i);

                larguraOrigem = alturaNova;
                alturaOrigem += alturaNova2x;
                larguraRedimensionada_3x++;
            }

            //this.ColorVerticalInterpolate8(array3x, arrayPIndex, arrayFinal, array32Vertical_1, array32Vertical_2, array32_duplo_vertical, alturaOriginal, alturaRedimensionar, larguraRedimensionar, 3);


            return resultado;
        }

        private void ColorSlowSpeedLineInterpolate8(byte[] a,
           byte[] c, int b, int[] d, int[] f, int[] e,
           int h, int g, int j)
        {

            int k;
            int m;
            int n;
            int l;
            int o;
            int p;
            int r;
            int q;
            int s;
            int u;

            for (k = q = s = 0; k < g; k++)
            {
                l = o = p = 0;
                n = d[k]; u = s;
                for (m = 0; m < n; m++)
                {
                    r = e[u];
                    l += a[r + 2] * e[u + 1];
                    o += a[r + 1] * e[u + 1];
                    p += a[r] * e[u + 1];
                    u += 2;
                }

                m = f[k];

                c[b + q + 2] = this.ParseByte(Math.Max(0, Math.Min(255, l / m)));
                c[b + q + 1] = this.ParseByte(Math.Max(0, Math.Min(255, o / m)));
                c[b + q + 0] = this.ParseByte(Math.Max(0, Math.Min(255, p / m)));

                q += j;
                s += h;
            }
        }

        private void ColorVerticalInterpolate8(byte[] bytes_1, IpIndex[] arrayPindex,
           byte[] bytes_2, int[] d, int[] f, int[] e,
           int h, int g, int comprimento, int coficiente)
        {

            int n, l, o, p, r, q, s;
            q = 0;

            var fg = f[g];
            var m = d[g];

            for (var i = 0; i < comprimento; i++)
            {
                n = l = o = 0; p = i * coficiente;
                s = h;
                for (g = 0; g < m; g++)
                {
                    r = arrayPindex[e[s]].pIndex + p;
                    n += bytes_1[r + 2] * e[s + 1];
                    l += bytes_1[r + 1] * e[s + 1];
                    o += bytes_1[r] * e[s + 1];
                    s += 2;
                }


                bytes_2[q + 2] = this.ParseByte(Math.Max(0, Math.Min(255, n / fg)));
                bytes_2[q + 1] = this.ParseByte(Math.Max(0, Math.Min(255, l / fg)));
                bytes_2[q] = this.ParseByte(Math.Max(0, Math.Min(255, o / fg)));
                q += coficiente;
            }
        }


        private void SetPixelsData(double a, int c, int b, int d, bool f, double e, EnumTipoInterpolacao tipoInterpolacao, double coficienteInterpolacao,
            int[] j, int[] k, int[] m, int[] n, int l)
        {

            Func<double, double> funcaoInterpolacao;

            double r, q, t;

            int w;
            int s = 0;
            int o = 0;
            int y = 0;
            int p;
            int u;

            y = 0;
            w = 0;

            funcaoInterpolacao = this.LanczosWeight;

            r = a / d;
            t = 1 <= r ? coficienteInterpolacao : coficienteInterpolacao / r; coficienteInterpolacao = 0;


            switch (tipoInterpolacao)
            {
                case EnumTipoInterpolacao.Triangle:
                    funcaoInterpolacao = this.TriangleWeight;
                    break;
                case EnumTipoInterpolacao.Hermite:
                    funcaoInterpolacao = this.HermiteWeight;
                    break;
                case EnumTipoInterpolacao.Bell:
                    funcaoInterpolacao = this.BellWeight;
                    break;
                case EnumTipoInterpolacao.QuadraticBSpline:
                    funcaoInterpolacao = this.QuadraticBSplineWeight;
                    break;
                case EnumTipoInterpolacao.CubicBSpline:
                    funcaoInterpolacao = this.CubicBSpline;
                    break;
                case EnumTipoInterpolacao.BoxFilter:
                    funcaoInterpolacao = this.BoxFilter;
                    break;
                case EnumTipoInterpolacao.Lanczos:
                    funcaoInterpolacao = this.LanczosWeight;
                    break;
                case EnumTipoInterpolacao.Michell:
                    funcaoInterpolacao = this.MitchellWeight;
                    break;
                case EnumTipoInterpolacao.Cosine:
                    funcaoInterpolacao = this.CosineWeight;
                    break;
                case EnumTipoInterpolacao.Catrom:
                    funcaoInterpolacao = this.CatmullRomWeight;
                    break;
                case EnumTipoInterpolacao.Quadratic:
                    funcaoInterpolacao = this.QuadraticWeight;
                    break;
                case EnumTipoInterpolacao.CubicConvolution:
                    funcaoInterpolacao = this.CubicConvolutionWeight;
                    break;
            }

            for (var i = 0; i < a; i++)
            {
                j[i] = 0;
                k[i] = 0;
                q = (i + 0.5) / r;
                u = w;
                o = (int)Math.Floor(q - t);
                s = (int)Math.Ceiling(q + t);

                if (0 > o)
                {
                    o = 0;
                }

                //0 > o && (o = 0);
                if (s >= d)
                {
                    s = d - 1;
                }
                //s >= d && (s = d - 1);

                if (f)
                {
                    if (s > e)
                    {
                        if (s < d)
                        {

                            coficienteInterpolacao += s - e;
                            e = s;
                            m[y] = s;

                        }
                    }
                }
                //f && (s > e && s < d && ,);

                for (; o <= s; o++)
                {

                    p = (1 < r) ?
                        this.ParseInt(1024 * funcaoInterpolacao(q - o - 0.5)) :
                        this.ParseInt(1024 * funcaoInterpolacao((q - o - 0.5) * r));

                    if (0 != p)
                    {

                        if (f)
                        {

                            n[u] = this.ParseByte(o - coficienteInterpolacao);

                            if ((-1 == n[u]))
                            {
                                n[u] = l;
                            }
                            //(-1 == n[u] && (n[u] = l));

                        }
                        else
                        {
                            n[u] = o * c;
                        }

                        //f ? (-1 == n[u] && (n[u] = l)) : n[u] = o * c;

                        n[u + 1] = p;
                        k[i] += p;
                        j[i]++;
                        u += 2;
                    }

                }
                w += b;
                y++;
            }
        }


        private double TriangleWeight(double a)
        {
            if (0 > a)
            {
                a = -a;
            }
            return 1 > a ? 1 - a : 0;
        }

        private double HermiteWeight(double a)
        {
            if (0 > a)
            {
                a = -a;
            }
            return 1 > a ? (2 * a - 3) * a * a + 1 : 0;
        }

        private double BellWeight(double a)
        {
            if (0 > a)
            {
                a = -a;
            }
            if (0.5 > a)
            {
                return 0.75 - a * a;
            }
            else
            {
                if (1.5 > a)
                {
                    a -= 1.5;
                    return 0.5 * a * a;
                }
            }
            return 0;
        }

        private double QuadraticBSplineWeight(double a)
        {
            if (0 > a)
            {
                a = -a;
            }
            return 0.5 >= a ? -a * a + 0.75 : 1.5 >= a ? 0.5 * a * a - 1.5 * a + 1.125 : 0;
        }

        private double CubicBSpline(double a)
        {
            double c, b, d;
            c = 0 < a + 2 ? a + 2 : 0;
            b = 0 < a + 1 ? a + 1 : 0;
            d = 0 < a ? a : 0;
            a = 0 < a - 1 ? a - 1 : 0;
            return (c * c * c - 4 * b * b * b + 6 * d * d * d - 4 * a * a * a) / 6;
        }

        private double BoxFilter(double a)
        {
            if (0 > a)
            {
                a = -a;
            }
            return 0.5 >= a ? 1 : 0;
        }

        private double Sinc(double a)
        {
            if (0 > a)
            {
                a = -a;
            }
            if (0 != a)
            {
                a *= 3.142;
                return Math.Sin(a) / a;
            }
            return 1D;
        }

        private double LanczosWeight(double a)
        {
            if (0 > a)
            {
                a = -a;
            }
            return 3 > a ? Sinc(a) * Sinc(a / 3) : 0;
        }

        private double MitchellWeight(double a)
        {
            double c;
            if (0 > a)
            {
                a = -a;
            }
            c = a * a;

            return 1D > a ? (7 * a * c + -12 * c + 5.333333) / 6 : 2 > a ? (-2.3333333 * a * c + 12 * c + -20 * a + 10.666666) / 6 : 0D;
        }

        private double CosineWeight(double a)
        {
            return -1D <= a && 1 >= a ? (Math.Cos(3.142 * a) + 1D) / 2D : 0;
        }

        private double CatmullRomWeight(double a)
        {
            double c;
            if (0 > a)
            {
                a = -a;
            }
            c = a * a;
            return 1 >= a ? 1.5 * c * a - 2.5 * c + 1 : 2 >= a ? -0.5 * c * a + 2.5 * c - 4 * a + 2 : 0;
        }

        private double QuadraticWeight(double a)
        {
            if (0 > a)
            {
                a = -a;
            }
            return 0.5 >= a ? -2 * a * a + 1 : 1.5 >= a ? a * a - 2.5 * a + 1.5 : 0;
        }

        private double CubicConvolutionWeight(double a)
        {
            double c;
            if (0 > a)
            {
                a = -a;
            }
            c = a * a;
            return 1D >= a ? 1.3333333 * c * a - 2.3333333 * c + 1 : 2 >= a ? -0.5833333333 * c * a + 3 * c - 4.916666666666 * a + 2.5 : 3 >= a ? 0.0833333333 * c * a - 0.666666666 * c + 1.75 * a - 1.5 : 0;
        }

        private   void L_CopyArrayRange(byte[] origem, int c, byte[] destino, int d, int f)
        {
            //fixed (byte* pSrc = origem)
            //{
            //    fixed (byte* pDest = destino)
            //    {
            //        for (var e = 0; e < f; e++)
            //        {
            //            pDest[e + d] = pSrc[e + c];
            //        }
            //    }
            //}
            //Array.Copy(origem, destinationArray,)

            //for (var e = 0; e < f; e++)
            //{
            //    destino[e + d] = origem[e + c];
            //}
        }

        private void L_ImageProcessGet(ImageData origem, byte[] destino, int b)
        {

            //d = "undefined" === typeof d? 0 : d;
            var d = 0;
            if ("TOP_LEFT" == _perspective)
            {
                var fim = 4 * origem.width * (b + 1);
                var inicio = 4 * origem.width * b;
                for (var i = inicio; i < fim; i += 4)
                {

                    destino[d] = origem.data[i + 2];
                    destino[1 + d] = origem.data[i + 1];
                    destino[2 + d] = origem.data[i];
                    d += 3;
                }
            }


            else if ("LEFT_TOP" == _perspective)
            {
                var f = 4 * origem.width;
                var e = 4 * b;

                for (b = 0; b < (int)origem.height; b += 1)
                {
                    destino[d] = origem.data[b * f + e + 2];
                    destino[1 + d] = origem.data[b * f + e + 1];
                    destino[2 + d] = origem.data[b * f + e];
                    d += 3;
                }
            }
        }

        private void L_ImageProcessPut(ImageData a, byte[] c, int b)
        {

            var d = 0;
            if ("TOP_LEFT" == _perspective)
            {
                var f = 4 * (int)a.width * (b + 1);
                var inicio = 4 * (int)a.width * b;

                for (b = inicio; b < f; b += 4)
                {
                    a.data[b + 2] = c[d];
                    a.data[b + 1] = c[d + 1];
                    a.data[b] = c[d + 2];
                    d += 3;
                }

            }
            else if ("LEFT_TOP" == _perspective)
            {
                var f = 4 * a.width;
                var e = 4 * b;
                for (b = 0; b < a.height; b += 1)
                {
                    a.data[b * f + e + 2] = c[d];
                    a.data[b * f + e + 1] = c[d + 1];
                    a.data[b * f + e] = c[d + 2];
                    d += 3;
                    if (this._progressData != null)
                    {
                        this.NotificarProgresso();
                    }
                }
            }
        }

        private int[] RetornarArrayInt32(int len)
        {
            return new int[len];
        }

        private byte[] RetornarArrayBytes(int len)
        {
            return new byte[len];
        }

        private void NotificarProgresso()
        {
            //não implementado
        }

        private int ParseInt(int a)
        {
            return a;
        }

        private int ParseInt(double a)
        {
            if (double.IsNaN(a))
            {
                return 0;
            }
            return Convert.ToInt32(Math.Round(a));
        }

        private double ParseFloat(int a)
        {
            return Convert.ToDouble(a);
        }

        private double ParseFloat(double a)
        {
            return a;
        }

        private byte ParseByte(double a)
        {
            if (double.IsNaN(a))
            {
                return 0;
            }
            if (a > byte.MaxValue)
            {
                throw new NotSupportedException();
            }
            return Convert.ToByte(Math.Round(a));
        }
    }
}
