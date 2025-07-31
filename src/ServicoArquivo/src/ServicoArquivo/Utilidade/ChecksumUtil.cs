namespace Snebur.ServicoArquivo.Utilidade;

//public class ChecksumUtil
//{
//    public const int TAMANHO_BUFFEER = 1024 * 1024;

//    public static string RetornarChecksum(string caminhoArquivo)
//    {
//        using (var stream = new BufferedStream(File.OpenRead(caminhoArquivo), TAMANHO_BUFFEER))
//        {
//            using (var md5 = new MD5CryptoServiceProvider())
//            {
//                var checksumHash = md5.ComputeHash(stream);
//                return ChecksumUtil.FormatarChecksum(checksumHash);
//            }
//        }
//    }

//    public static string RetornarChecksum(byte[] bytes)
//    {
//        using (var md5 = new MD5CryptoServiceProvider())
//        {
//            var checksumHash = md5.ComputeHash(bytes);
//            return ChecksumUtil.FormatarChecksum(checksumHash);
//        }
//    }

//    private static string FormatarChecksum(byte[] checksumHash)
//    {
//        var sb = new StringBuilder();
//        foreach (var b in checksumHash)
//        {
//            sb.Append(b.ToString("x2"));
//        }
//        return sb.ToString();
//    }

//    //public static string RetornarChecksumJavascript(string caminhoArquivo)
//    //{
//    //    return ChecksumUtil.RetornarChecksumJavascript(File.ReadAllBytes(caminhoArquivo));
//    //}

//    //public static string RetornarChecksumJavascript(byte[] bytes)
//    //{
//    //    return Crc32.Crc32Algorithm.Compute(bytes).ToString();
//    //}

//}