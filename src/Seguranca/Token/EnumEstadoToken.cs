using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.Seguranca
{
    public enum EnumEstadoToken
    {
        Valido = 1,
        Expirado = 2,
        Invalido = 3,
        ChaveInvalida = 4,
        ExpiradoDataFuturaUltrapassada = 5

    }
}
