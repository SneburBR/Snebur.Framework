//using System;
//using System.Linq;
//using System.Text;
//using System.Collections.Generic;

//namespace Snebur.Utilidade
//{

//    public class Md5Util
//    {
//        public static string RetornarHash(string value)
//        {

//            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
//            var data = System.Text.Encoding.UTF8.GetBytes(value);
//            data = md5.ComputeHash(data);
//            System.Text.StringBuilder sb = new System.Text.StringBuilder();
//            foreach (var b in data)
//            {
//                sb.Append(b.ToString("x2"));
//            }
//            return sb.ToString();
//        }
//    }
//}
