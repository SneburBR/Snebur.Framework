#if NET7_0
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace System.Web
{
   
    public interface IHttpHandler
    {

        //bool IsReusable { get; }
        Task ProcessRequestAsync(HttpContext context);
    }
}
#endif