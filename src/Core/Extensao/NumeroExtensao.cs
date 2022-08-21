namespace System
{
    public static class NumeroExtensao
    {
        public static int Absoluto(this int _inteiro)
        {
            return Math.Abs(_inteiro);
        }

        public static int Contrario(this int _inteiro)
        {
            return _inteiro * -1;
        }

        public static int Negativo(this int _inteiro)
        {
            return _inteiro.Absoluto() * -1;
        }

        public static int ToInt32(this double value, MidpointRounding midpointRounding = MidpointRounding.ToEven)
        {
            return (int)Math.Round(value, midpointRounding);
        }
    }
}
