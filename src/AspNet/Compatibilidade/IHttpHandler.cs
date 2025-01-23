﻿#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http
{
   
    public interface IHttpHandler
    {

        //bool IsReusable { get; }
        Task ProcessRequestAsync(HttpContext context);
    }
}

#endif