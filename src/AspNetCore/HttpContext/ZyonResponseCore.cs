using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Snebur.Net
{
    public class ZyonResponseCore : ZyonResponse
    {
        public HttpResponse Response { get; }
        public ZyonResponseCore(HttpResponse response)
        {
            this.Response = response;
        }

        public override string ContentType { get => this.Response.ContentType; set => this.Response.ContentType = value; }
        public override Encoding ContentEncoding { get; set; }
        public override int StatusCode { get => this.Response.StatusCode; set => this.Response.StatusCode = value; }
        public override int SubStatusCode { get; set; }

        public override async Task WriteFileAsync(string caminhoCompletoArquivo)
        {
            var bytesArquivo = File.ReadAllBytes(caminhoCompletoArquivo);
            await this.Response.Body.WriteAsync(bytesArquivo.AsMemory(0, bytesArquivo.Length));
        }

        public override Task WriteAsync(string respostaString)
        {
            return this.Response.WriteAsync(respostaString, Encoding.UTF8);
        }

        public override void WriteFile(string caminhoCompletoArquivo)
        {
            throw new NotSupportedException($"Utilizar {nameof(this.WriteFileAsync)}");
        }

        public override void Write(string respostaString)
        {
            throw new NotSupportedException($"Utilizar {nameof(this.WriteAsync)}");
        }
    }
}