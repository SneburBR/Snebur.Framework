

namespace Snebur.Comunicacao
{
#if NetCore

    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract partial class BaseComunicacaoServidor
    {
        public Task ProcessRequestAsync(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
#endif
}
