using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snebur
{
    public interface IDispatcher
    {
        object Invoke(Delegate method, params object[] args);
    }
}
