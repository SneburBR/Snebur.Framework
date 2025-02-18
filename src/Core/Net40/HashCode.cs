//#if NET40
//namespace System
//{
//    public static class HashCode
//    {
//        public static int Combine(int hash1, int hash2)
//        {
//            return (hash1 << 5) + hash1 ^ hash2;
//        }

//        public static int Combine(int hash1, int hash2, int hash3)
//        {
//            return Combine(Combine(hash1, hash2), hash3);
//        }
//    }
//}
//#endif