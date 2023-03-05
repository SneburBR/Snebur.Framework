using Snebur.Utilidade;
using System.Net;
using System.Text;

#if NetCore
using System.Threading.Tasks;
#endif

namespace Snebur.Net
{
    internal class SnResponseHttpListener : SnResponse
    {
        private const int TAMANHO_BUFFER = 64 * 1024;
        private HttpListenerResponse Response { get; }

        public override string ContentType { get => this.Response.ContentType; set => this.Response.ContentType = value; }
        public override Encoding ContentEncoding { get => this.Response.ContentEncoding; set => this.Response.ContentEncoding = value; }
        public override int StatusCode { get => this.Response.StatusCode; set => this.Response.StatusCode = value; }

        public override int SubStatusCode { get; set; }

        public SnResponseHttpListener(HttpListenerResponse response)
        {
            this.Response = response;
        }

        public override void WriteFile(string caminhoCompletoArquivo)
        {
            using (var fs = StreamUtil.OpenRead(caminhoCompletoArquivo))
            {
                while (true)
                {
                    var buffer = new byte[TAMANHO_BUFFER];
                    var lidos = fs.Read(buffer, 0, buffer.Length);
                    if (lidos == 0)
                    {
                        break;
                    }
                    this.Response.OutputStream.Write(buffer, 0, lidos);
                }
            }
        }

        public override void Write(string respostaString)
        {
            var bytesUtf8 = Encoding.UTF8.GetBytes(respostaString);
            this.Response.OutputStream.Write(bytesUtf8, 0, bytesUtf8.Length);
        }

#if NetCore

        public override Task WriteFileAsync(string caminhoCompletoArquivo)
        {
            throw new System.NotImplementedException();
        }

        public override Task WriteAsync(string respostaString)
        {
            throw new System.NotImplementedException();
        }

#endif
    }
}