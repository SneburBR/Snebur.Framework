using System.Text;

#if NetCore
using System.Threading.Tasks;
#endif

namespace Snebur.Net
{
    public abstract class SnResponse
    {
        public abstract string ContentType { get; set; }
        public abstract Encoding ContentEncoding { get; set; }
        public abstract int StatusCode { get; set; }
        public abstract int SubStatusCode { get; set; }

        public abstract void WriteFile(string caminhoCompletoArquivo);
        public abstract void Write(string respostaString);

#if NetCore
        public abstract Task WriteFileAsync(string caminhoCompletoArquivo);
        public abstract Task WriteAsync(string respostaString);
#endif
    }
}