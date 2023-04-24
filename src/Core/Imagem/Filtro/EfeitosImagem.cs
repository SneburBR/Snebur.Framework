using Snebur.Dominio;
using Snebur.Imagens;
using System.Collections.Generic;

namespace Snebur.UI
{
    public static class EfeitosImagem
    {
        public static Dictionary<EnumEfeitoImagem, EfeitoImagem> Efeitos { get; } = new Dictionary<EnumEfeitoImagem, EfeitoImagem>
        {
            {EnumEfeitoImagem.Nenhum, EfeitosImagem.Normal },
            {EnumEfeitoImagem.PretoBranco, EfeitosImagem.PretoBranco },
            {EnumEfeitoImagem.Sepia, EfeitosImagem.Sepia },
            {EnumEfeitoImagem.Cancum, EfeitosImagem.Cancum },
            {EnumEfeitoImagem.Moscou, EfeitosImagem.Moscou },
            {EnumEfeitoImagem.Dubai, EfeitosImagem.Dubai },
            {EnumEfeitoImagem.Paris, EfeitosImagem.Paris },
            {EnumEfeitoImagem.Chicago, EfeitosImagem.Chicago },
            {EnumEfeitoImagem.Veneza, EfeitosImagem.Veneza },
            {EnumEfeitoImagem.Cairo, EfeitosImagem.Cairo },
            {EnumEfeitoImagem.Acapulco, EfeitosImagem.Acapulco },
            {EnumEfeitoImagem.Fortaleza, EfeitosImagem.Fortaleza },
            {EnumEfeitoImagem.Pequim, EfeitosImagem.Pequim },
            {EnumEfeitoImagem.Atenas, EfeitosImagem.Atenas },
            {EnumEfeitoImagem.Manaus, EfeitosImagem.Dakar },
            {EnumEfeitoImagem.Rio, EfeitosImagem.Rio },
            {EnumEfeitoImagem.Sydney, EfeitosImagem.Sydney },
            {EnumEfeitoImagem.Vancouver, EfeitosImagem.Vancouver },
            {EnumEfeitoImagem.SaoPaulo, EfeitosImagem.SaoPaulo },
            {EnumEfeitoImagem.Jaipur, EfeitosImagem.Jaipur },
            {EnumEfeitoImagem.Medellin, EfeitosImagem.Medellin },
            {EnumEfeitoImagem.Londres, EfeitosImagem.Londres }
        };

        private static EfeitoImagem _normal;
        private static EfeitoImagem _pretoBranco;
        private static EfeitoImagem _sepia;
        private static EfeitoImagem _cancum;
        private static EfeitoImagem _moscou;
        private static EfeitoImagem _dubai;
        private static EfeitoImagem _paris;
        private static EfeitoImagem _chicago;
        private static EfeitoImagem _veneza;
        private static EfeitoImagem _cairo;
        private static EfeitoImagem _acapulco;
        private static EfeitoImagem _fortaleza;
        private static EfeitoImagem _pequim;
        private static EfeitoImagem _atenas;
        private static EfeitoImagem _dakar;
        private static EfeitoImagem _rio;
        private static EfeitoImagem _sydney;
        private static EfeitoImagem _vancouver;
        private static EfeitoImagem _saoPaulo;
        private static EfeitoImagem _jaipur;
        private static EfeitoImagem _medellin;
        private static EfeitoImagem _londres;

        public static EfeitoImagem Normal
        {
            get
            {
                if (_normal == null)
                {
                    _normal = new EfeitoImagem(EnumEfeitoImagem.Nenhum, FiltroImagem.Empty);
                }
                return _normal;
            }
        }

        public static EfeitoImagem PretoBranco
        {
            get
            {
                if (EfeitosImagem._pretoBranco == null)
                {
                    var filtro = new FiltroImagem
                    {
                        PretoBranco = 100
                    };
                    EfeitosImagem._pretoBranco = new EfeitoImagem(EnumEfeitoImagem.PretoBranco, filtro);
                }
                return EfeitosImagem._pretoBranco;
            }
        }

        public static EfeitoImagem Sepia
        {
            get
            {
                if (EfeitosImagem._sepia == null)
                {
                    var filtro = new FiltroImagem
                    {
                        Sepia = 100
                    };
                    EfeitosImagem._sepia = new EfeitoImagem(EnumEfeitoImagem.Sepia, filtro);
                }
                return EfeitosImagem._sepia;
            }
        }
        //1977
        public static EfeitoImagem Cancum
        {
            get
            {
                if (EfeitosImagem._cancum == null)
                {
                    var sobrePosicao = new SobrePosicaoSolida
                    {
                        Cor = "rgba(243,106,188,1)",
                        Mixagem = EnumMixagem.SoftLight,
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 110,
                        Brilho = 110,
                        Saturacao = 130
                    };

                    EfeitosImagem._cancum = new EfeitoImagem(EnumEfeitoImagem.Cancum, filtro, sobrePosicao);
                }
                return EfeitosImagem._cancum;
            }
        }
        //Aden
        public static EfeitoImagem Moscou
        {
            get
            {
                if (EfeitosImagem._moscou == null)
                {
                    var sobreposicao = new SobrePosicaoGradienteLinear
                    {
                        Cor1 = "rgba(66, 10, 14, 1)",
                        Cor2 = "rgba(255, 255, 255, 1)",
                        LimiteCor1 = 0,
                        LimiteCor2 = 100,
                        Direcao = EnumDirecaoGradiente.ToRight,
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 90,
                        Brilho = 120,
                        Saturacao = 85,
                        Matriz = 20,
                    };

                    EfeitosImagem._moscou = new EfeitoImagem(EnumEfeitoImagem.Moscou, filtro, sobreposicao);
                }
                return EfeitosImagem._moscou;
            }
        }
        //Amaro
        public static EfeitoImagem Dubai
        {
            get
            {
                if (EfeitosImagem._dubai == null)
                {
                    var filtro = new FiltroImagem
                    {
                        Contraste = 90,
                        Brilho = 110,
                        Saturacao = 150,
                        Matriz = 350
                    };
                    EfeitosImagem._dubai = new EfeitoImagem(EnumEfeitoImagem.Dubai, filtro);
                }
                return EfeitosImagem._dubai;
            }
        }
        //Brannan
        public static EfeitoImagem Paris
        {
            get
            {
                if (EfeitosImagem._paris == null)
                {
                    var sobrePosicao = new SobrePosicaoSolida
                    {
                        Cor = "rgba(161, 44, 199, 1)",
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 140,
                        Sepia = 50
                    };
                    EfeitosImagem._paris = new EfeitoImagem(EnumEfeitoImagem.Paris, filtro, sobrePosicao);
                }
                return EfeitosImagem._paris;
            }
        }
        //Brooklyn
        public static EfeitoImagem Chicago
        {
            get
            {
                if (EfeitosImagem._chicago == null)
                {
                    var sobrePosicao = new SobrePosicaoGradienteLinear
                    {
                        Cor1 = "rgba(168, 223, 193, 1)",
                        Cor2 = "rgba(183, 196, 200, 1)",
                        LimiteCor1 = 0,
                        LimiteCor2 = 100,
                        Direcao = EnumDirecaoGradiente.ToRight,
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 90,
                        Brilho = 110
                    };

                    EfeitosImagem._chicago = new EfeitoImagem(EnumEfeitoImagem.Chicago, filtro, sobrePosicao);
                }
                return EfeitosImagem._chicago;
            }
        }
        //Clarendon
        public static EfeitoImagem Veneza
        {
            get
            {
                if (EfeitosImagem._veneza == null)
                {
                    var sobrePosicao = new SobrePosicaoSolida
                    {
                        Cor = "rgba(127,187,227,1)",
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 120,
                        Saturacao = 125
                    };

                    EfeitosImagem._veneza = new EfeitoImagem(EnumEfeitoImagem.Veneza, filtro, sobrePosicao);
                }
                return EfeitosImagem._veneza;
            }
        }
        //Earlybird
        public static EfeitoImagem Cairo
        {
            get
            {
                if (EfeitosImagem._cairo == null)
                {
                    var sobrePosicao = new SobrePosicaoGradienteLinear
                    {
                        Cor1 = "rgba(208,186,142,1)",
                        Cor2 = "rgba(29,2,16,1)",
                        LimiteCor1 = 0,
                        LimiteCor2 = 100,
                        Direcao = EnumDirecaoGradiente.ToRight,
                        Mixagem = EnumMixagem.SoftLight,
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 90,
                        Sepia = 20
                    };

                    EfeitosImagem._cairo = new EfeitoImagem(EnumEfeitoImagem.Cairo, filtro, sobrePosicao);
                }
                return EfeitosImagem._cairo;
            }
        }
        //Gingham
        public static EfeitoImagem Acapulco
        {
            get
            {
                if (EfeitosImagem._acapulco == null)
                {
                    var sobreposicao = new SobrePosicaoGradienteLinear
                    {
                        Cor1 = "rgba(66, 10, 14, 1)",
                        Cor2 = "rgba(255,255,255,1)",
                        LimiteCor1 = 0,
                        LimiteCor2 = 100,
                        Direcao = EnumDirecaoGradiente.ToRight,
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Brilho = 105,
                        Matriz = 350
                    };

                    EfeitosImagem._acapulco = new EfeitoImagem(EnumEfeitoImagem.Acapulco, filtro, sobreposicao);
                }
                return EfeitosImagem._acapulco;
            }
        }
        //Hudson
        public static EfeitoImagem Fortaleza
        {
            get
            {
                if (EfeitosImagem._fortaleza == null)
                {
                    var sobrePosicao = new SobrePosicaoGradienteLinear
                    {
                        Cor1 = "rgba(255,177,166,1)",
                        Cor2 = "rgba(52,33,52,1)",
                        LimiteCor1 = 0,
                        LimiteCor2 = 100,
                        Direcao = EnumDirecaoGradiente.ToRight,
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 90,
                        Brilho = 120,
                        Saturacao = 110
                    };

                    EfeitosImagem._fortaleza = new EfeitoImagem(EnumEfeitoImagem.Fortaleza, filtro, sobrePosicao);
                }
                return EfeitosImagem._fortaleza;
            }
        }
        //InkWell
        public static EfeitoImagem Pequim
        {
            get
            {
                if (EfeitosImagem._pequim == null)
                {
                    var filtro = new FiltroImagem
                    {
                        Brilho = 110,
                        Contraste = 110,
                        Sepia = 30
                    };

                    EfeitosImagem._pequim = new EfeitoImagem(EnumEfeitoImagem.Pequim, filtro/*, sobrePosicao*/);
                }
                return EfeitosImagem._pequim;
            }
        }
        //lofi
        public static EfeitoImagem Atenas
        {
            get
            {
                if (EfeitosImagem._atenas == null)
                {
                    var sobrePosicao = new SobrePosicaoGradienteLinear
                    {
                        Cor1 = "rgba(255,255,255,1)",
                        Cor2 = "rgba(34,34,34,1)",
                        LimiteCor1 = 0,
                        LimiteCor2 = 100,
                        Direcao = EnumDirecaoGradiente.ToRight,
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 150,
                        Saturacao = 110
                    };

                    EfeitosImagem._atenas = new EfeitoImagem(EnumEfeitoImagem.Atenas, filtro, sobrePosicao);
                }
                return EfeitosImagem._atenas;
            }
        }
        //Maven
        public static EfeitoImagem Dakar
        {
            get
            {
                if (EfeitosImagem._dakar == null)
                {
                    var sobrePosicao = new SobrePosicaoSolida
                    {
                        Cor = "rgba(3,230,26,1)",
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 95,
                        Brilho = 95,
                        Saturacao = 150,
                        Sepia = 25
                    };

                    EfeitosImagem._dakar = new EfeitoImagem(EnumEfeitoImagem.Manaus, filtro, sobrePosicao);
                }
                return EfeitosImagem._dakar;
            }
        }
        //Perpetua
        public static EfeitoImagem Rio
        {
            get
            {
                if (EfeitosImagem._rio == null)
                {
                    var sobreposicao = new SobrePosicaoGradienteLinear
                    {
                        Cor1 = "rgba(0, 91,154,1)",
                        Cor2 = "rgba(255,255,255,1)",
                        LimiteCor1 = 0,
                        LimiteCor2 = 100,
                        Direcao = EnumDirecaoGradiente.ToBottom,
                        Mixagem = EnumMixagem.SoftLight
                    };
                    EfeitosImagem._rio = new EfeitoImagem(EnumEfeitoImagem.Rio, FiltroImagem.Empty, sobreposicao);
                }
                return EfeitosImagem._rio;
            }
        }
        //Reyes
        public static EfeitoImagem Sydney
        {
            get
            {
                if (EfeitosImagem._sydney == null)
                {
                    var sobrePosicao = new SobrePosicaoSolida
                    {
                        Cor = "rgba(173, 205, 239, 1)",
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 85,
                        Brilho = 110,
                        Saturacao = 75,
                        Sepia = 22
                    };

                    EfeitosImagem._sydney = new EfeitoImagem(EnumEfeitoImagem.Sydney, filtro, sobrePosicao);
                }
                return EfeitosImagem._sydney;
            }
        }
        //Stinson
        public static EfeitoImagem Vancouver
        {
            get
            {
                if (EfeitosImagem._vancouver == null)
                {
                    var sobrePosicao = new SobrePosicaoSolida
                    {
                        Cor = "rgba(240, 149, 128, 1)",
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 75,
                        Brilho = 115,
                        Saturacao = 85
                    };

                    EfeitosImagem._vancouver = new EfeitoImagem(EnumEfeitoImagem.Vancouver, filtro, sobrePosicao);
                }
                return EfeitosImagem._vancouver;
            }
        }
        //Toaster
        public static EfeitoImagem SaoPaulo
        {
            get
            {
                if (EfeitosImagem._saoPaulo == null)
                {
                    var sobreposicao = new SobrePosicaoGradienteLinear
                    {
                        Cor1 = "rgba(15, 78, 128, 1)",
                        Cor2 = "rgba(59, 0, 59, 1)",
                        LimiteCor1 = 0,
                        LimiteCor2 = 100,
                        Direcao = EnumDirecaoGradiente.ToRight,
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 150,
                        Brilho = 90
                    };

                    EfeitosImagem._saoPaulo = new EfeitoImagem(EnumEfeitoImagem.SaoPaulo, filtro, sobreposicao);
                }
                return EfeitosImagem._saoPaulo;
            }
        }
        //Walden
        public static EfeitoImagem Jaipur
        {
            get
            {
                if (EfeitosImagem._jaipur == null)
                {
                    var sobrePosicao = new SobrePosicaoSolida
                    {
                        Cor = "rgba(204, 68, 0, 1)",
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Brilho = 110,
                        Saturacao = 160,
                        Sepia = 30,
                        Matriz = 350
                    };

                    EfeitosImagem._jaipur = new EfeitoImagem(EnumEfeitoImagem.Jaipur, filtro, sobrePosicao);
                }
                return EfeitosImagem._jaipur;
            }
        }
        //Valencia
        public static EfeitoImagem Medellin
        {
            get
            {
                if (EfeitosImagem._medellin == null)
                {
                    var sobrePosicao = new SobrePosicaoSolida
                    {
                        Cor = "rgba(58, 3, 57, 1)",
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Contraste = 108,
                        Brilho = 108,
                        Sepia = 8
                    };

                    EfeitosImagem._medellin = new EfeitoImagem(EnumEfeitoImagem.Medellin, filtro, sobrePosicao);
                }
                return EfeitosImagem._medellin;
            }
        }
        //XPro2
        public static EfeitoImagem Londres
        {
            get
            {
                if (EfeitosImagem._londres == null)
                {
                    var sobreposicao = new SobrePosicaoGradienteLinear
                    {
                        Cor1 = "rgba(224, 231, 230, 1)",
                        Cor2 = "rgba(43, 42, 161, 1)",
                        LimiteCor1 = 0,
                        LimiteCor2 = 100,
                        Direcao = EnumDirecaoGradiente.ToRight,
                        Mixagem = EnumMixagem.SoftLight
                    };

                    var filtro = new FiltroImagem
                    {
                        Sepia = 30
                    };

                    EfeitosImagem._londres = new EfeitoImagem(EnumEfeitoImagem.Londres, filtro, sobreposicao);
                }
                return EfeitosImagem._londres;
            }
        }
    }
}