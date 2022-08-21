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