#if NET6_0_OR_GREATER
#else
using System.Web;
#endif

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Snebur;

public static class HttpResponseExtensions
{
    public static Task WriteFileAsync(this HttpResponse httpResponse, string path)
    {
        return httpResponse.SendFileAsync(path);
    }
}
