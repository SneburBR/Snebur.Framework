using System.Diagnostics;

namespace Snebur.Utilidade
{
    public static class DebugUtil
    {
        public static bool IsAttached
        {
            get
            {
                //return true;
                return Debugger.IsAttached;
            }
        }
        
    }
}
