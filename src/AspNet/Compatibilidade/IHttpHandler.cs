using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http;

public interface IHttpHandler
{

    //bool IsReusable { get; }
    Task ProcessRequestAsync(HttpContext context);
}
