using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Snebur.Utilidade;

#if NET40
using Ionic.Zip;
#endif

namespace Snebur.Comunicacao
{
    public class PacoteUtil
    {
        private const string NOME_ARQUIVO_PACOTE = "pacote.json";
        //private const string SENHA_PACOTE = "f1e0f12c-3b34-429c-b1e0-c5f5925d5c87";

#if NET45 || NET50
        public static byte[] CompactarPacote(string json)
        {
            using (var msZip = new MemoryStream())
            {
                using (var ms = new MemoryStream())
                {
                    using (var zip = new ZipArchive(msZip, ZipArchiveMode.Create))
                    {
                        var arquivo = zip.CreateEntry(NOME_ARQUIVO_PACOTE);
                        var bytesUft8 = Encoding.UTF8.GetBytes(json);
                        using (var sr = arquivo.Open())
                        {
                            sr.Write(bytesUft8, 0, bytesUft8.Length);
                            sr.Flush();
                        }
                    }
                }
                return BaralhoUtil.Embaralhar(msZip.ToArray());
            }
        }

        public static string DescompactarPacote(Stream stream)
        {
            if (stream != null)
            {
                using (var ms = StreamUtil.RetornarMemoryStream(stream))
                {
                    var bytes = BaralhoUtil.Desembaralhar(ms.ToArray());
                    using (var msReverso = new MemoryStream(bytes))
                    {
                        using (var zip = new ZipArchive(msReverso))
                        {
                            using (var arquivo = zip.Entries.First().Open())
                            {
                                using (var sr = new StreamReader(arquivo, Encoding.UTF8))
                                {
                                    var conteudo = sr.ReadToEnd();
                                    return conteudo;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

#endif

#if NET40

        public static byte[] CompactarPacote(string json)
        {
            throw new Exception("Depreciado");
            using (var msZip = new MemoryStream())
            {
                using (var zip = new ZipFile())
                {
                    var bytesUft8 = Encoding.UTF8.GetBytes(json);
                    zip.AddEntry(NOME_ARQUIVO_PACOTE, bytesUft8);
                    zip.Save(msZip);
                }
                return msZip.ToArray();
            }
        }

        public static string DescompactarPacote(Stream stream)
        {
            throw new Exception("Depreciado");
            if (stream != null)
            {
                using (var ms = StreamUtil.RetornarMemoryStream(stream))
                {
                    using (var zip = ZipFile.Read(ms))
                    {
                        using (var arquivo = zip.Entries.First().OpenReader())
                        {
                            using (var sr = new StreamReader(arquivo, Encoding.UTF8))
                            {
                                var resultado = sr.ReadToEnd();
                                return resultado;
                            }
                        }
                    }
                }
            }
            return null;
        }
#endif
    }
}