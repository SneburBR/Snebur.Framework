namespace System.Numerics
{

    public static class BitOperations
    {

        public static uint RotateLeft(uint value, int offset)
           => (value << offset) | (value >> (32 - offset));
    }
}