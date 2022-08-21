using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snebur.Imagem
{
    public interface IpIndex
    {
        int pIndex { get; set; }
    }

    public class pIndexIntancia : IpIndex
    {
        public int pIndex { get; set; }
    }
}
