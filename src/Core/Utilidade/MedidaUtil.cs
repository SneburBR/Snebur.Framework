﻿using Snebur.Dominio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Snebur.Utilidade
{
    public static class MedidaUtil
    {
        public const int BITS_POR_PIXELS_RGBA = 32;
        public const int BITS_POR_PIXELS_JPEG = 24;

        public const double DPI_IMPRESSAO_PADRAO = 300;
        public const double POLEGADA = 2.54;
        public const double DPI_VISUALIZACAO_WPF = 96;

        public const double MAXIMO_AREA_IMPRESSAO_SUPORTADA = 200 * 1024 * 1024;

        public static readonly Dimensao DIMENSAO_RECIPIENTE_APRESENTACAO = new Dimensao(1920, 1080);

        public static double RetornarPixelsImpressao(double medidaEmCentimetros)
        {
            return MedidaUtil.ParaPixels(medidaEmCentimetros, MedidaUtil.DPI_IMPRESSAO_PADRAO);
        }

        public static int RetornarPixelsVisualizacao(double medidiaEmCentimetros, double dpiVisualizacao)
        {
            return MedidaUtil.ParaPixels(medidiaEmCentimetros, dpiVisualizacao);
        }

        public static int ParaPixels(double medidaEmCentimetos, double dpi)
        {
            return Convert.ToInt32((medidaEmCentimetos / MedidaUtil.POLEGADA) * dpi);
        }

        public static double ParaCentimentros(double mediaEmPixels, double dpi)
        {
            return (mediaEmPixels / dpi) * MedidaUtil.POLEGADA;
        }

        public static double RetornarDpiVisualizacao(double medidaEmCentimetros, double medidaEmPixel)
        {
            return (medidaEmPixel * 300) / MedidaUtil.RetornarPixelsImpressao(medidaEmCentimetros);
        }

        public static int CentimetrosParaDecimilimetros(double espessuraLaminasCm)
        {
            return Convert.ToInt32(Math.Round(espessuraLaminasCm * 10 * 10));
        }

        public static int RetornarDpiVisualizacao(Dimensao dimensaoCentimetros, Dimensao dimensaoPixels, double tolerancia = 1)
        {
            var dpiX = (int)Math.Round(MedidaUtil.RetornarDpiVisualizacao(dimensaoCentimetros.Largura, dimensaoPixels.Largura));
            var dpiY = (int)Math.Round(MedidaUtil.RetornarDpiVisualizacao(dimensaoCentimetros.Altura, dimensaoPixels.Altura));
            if (Math.Abs(dpiX - dpiX) > tolerancia)
            {
                throw new Erro("Dpi X e diferente do Y, foram da margem tolerancia");
            }
            return Math.Min(dpiX, dpiY);
        }

        public static void DefinirDpiImpressao(object objeto)
        {
            DefinirDpiVisualizacao(objeto, MedidaUtil.DPI_IMPRESSAO_PADRAO);
        }

        public static void DefinirDpiVisualizacao(object objeto, double dpi)
        {
            Func<double> funcao = () =>
            {
                return dpi;
            };
            DefinirDpiVisualizacao(objeto, funcao);
        }

        public static void DefinirDpiVisualizacao(object objeto, Func<double> funcaoRetornarDpi)
        {
            if (objeto != null)
            {
                var objetosAnalisados = new HashSet<object>();
                MedidaUtil.DefinirDpiTipoComplexo(objeto, funcaoRetornarDpi, objetosAnalisados);
            }
        }

        private static void DefinirDpiTipoComplexo(object objeto, Func<double> funcaoRetornarDpi, HashSet<object> objetosAnalisados)
        {
            var tipo = objeto.GetType();
            if (ReflexaoUtil.TipoRetornaColecao(tipo))
            {
                foreach (var item in (IEnumerable)objeto)
                {
                    MedidaUtil.DefinirDpiTipoComplexo(item, funcaoRetornarDpi, objetosAnalisados);
                }
            }
            else
            {
                if ((objetosAnalisados.Contains(objeto)))
                {
                    return;
                }
                objetosAnalisados.Add(objeto);

                if (objeto is IDpiVisualizacao objetoDpi)
                {
                    objetoDpi.FuncaoDpiVisualizacao = funcaoRetornarDpi;
                }
                if (!tipo.IsValueType && tipo != typeof(string) && !tipo.IsSubclassOf(typeof(BaseTipoComplexo)))
                {
                    var propriedades = objeto.GetType().GetProperties().Where(x => x.SetMethod != null && x.SetMethod.IsPublic);
                    foreach (var p in propriedades)
                    {
                        var objetoFilho = p.GetValue(objeto);
                        if (objetoFilho != null)
                        {
                            DefinirDpiTipoComplexo(objetoFilho, funcaoRetornarDpi, objetosAnalisados);
                        }
                    }
                }
            }
        }

        public static double FuncaoRetornarDpiImpressao()
        {
            return DPI_IMPRESSAO_PADRAO;
        }
        public static double RetornarDpiImpressao(IDimensao dimensao)
        {
            var larguraPixels = MedidaUtil.RetornarPixelsImpressao(dimensao.Largura);
            var alturaPixels = MedidaUtil.RetornarPixelsImpressao(dimensao.Largura);
            var escala = Math.Sqrt(MAXIMO_AREA_IMPRESSAO_SUPORTADA) / Math.Sqrt((larguraPixels * alturaPixels));
            if (escala > 1)
            {
                return MedidaUtil.DPI_IMPRESSAO_PADRAO;
            }
            var dpi = MedidaUtil.DPI_IMPRESSAO_PADRAO * escala;
            if (DebugUtil.IsAttached)
            {
                var larguraPixelsNormalizada = MedidaUtil.RetornarPixelsVisualizacao(dimensao.Largura, dpi);
                var alturaPixelsNormalizada = MedidaUtil.RetornarPixelsVisualizacao(dimensao.Altura, dpi);

                if (((larguraPixelsNormalizada * alturaPixelsNormalizada) * 0.95) > MAXIMO_AREA_IMPRESSAO_SUPORTADA)
                {
                    throw new Exception("Falha no calculo dpi de impressão");
                }
            }
            return dpi;
        }
    }
}