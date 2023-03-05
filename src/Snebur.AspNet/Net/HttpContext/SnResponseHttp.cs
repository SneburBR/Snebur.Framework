using System.Text;
using System.Web;

namespace Snebur.Net
{
    public class SnResponseHttp : SnResponse
    {
        private const int TAMANHO_BUFFER = 64 * 1024;
        public HttpResponse Response { get; }


        public SnResponseHttp(HttpResponse response)
        {
            this.Response = response;
        }

        public override string ContentType { get => this.Response.ContentType; set => this.Response.ContentType = value; }
        public override Encoding ContentEncoding { get => this.Response.ContentEncoding; set => this.Response.ContentEncoding = value; }
        public override int StatusCode { get => this.Response.StatusCode; set => this.Response.StatusCode = value; }

        public override int SubStatusCode { get => this.Response.SubStatusCode; set => this.Response.SubStatusCode = value; }

        public override void WriteFile(string caminhoCompletoArquivo)
        {
            //using (var fs = StreamUtil.OpenRead(caminhoCompletoArquivo))
            //{
            //    while (true)
            //    {
            //        var buffer = new byte[TAMANHO_BUFFER];
            //        var lidos = fs.Read(buffer, 0, buffer.Length);
            //        if (lidos == 0)
            //        {
            //            break;
            //        }
            //        this.Response.BinaryWrite(buffer);
            //        //this.Response.OutputStream.Write(buffer, 0, lidos);
            //    }
            //}

            this.Response.WriteFile(caminhoCompletoArquivo);
        }

        public override void Write(string respostaString)
        {
            this.Response.Write(respostaString);
        }
    }
}
