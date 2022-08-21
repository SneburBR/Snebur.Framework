using System;
using System.Security.Cryptography;
using System.Text;

namespace Snebur.Utilidade
{
    public static class Md5Util
    {
        public static string RetornarHash(string texto)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var bytes = Encoding.UTF8.GetBytes(texto);
                bytes = md5.ComputeHash(bytes);
                var sb = new StringBuilder();
                foreach (var b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                var r = sb.ToString();
                return r;
            }
        }

        public static Guid RetornarHashGuid(string texto)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(texto);
                bytes = md5.ComputeHash(bytes);
                return new Guid(bytes);
            }
        }
    }
}
