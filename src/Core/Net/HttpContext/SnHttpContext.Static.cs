using System.Collections.Generic;

namespace Snebur.Net
{
    public partial class SnHttpContext
    {
        private static Dictionary<int, SnHttpContext> ContextosAtivos { get; } = new Dictionary<int, SnHttpContext>();

        //public static ZyonHttpContext Current
        //{
        //    get
        //    {
        //        var chave = Thread.CurrentThread.ManagedThreadId;
        //        lock (((ICollection)ContextosAtivos).SyncRoot)
        //        {
        //            if (ContextosAtivos.ContainsKey(chave))
        //            {
        //                return ContextosAtivos[chave];
        //            }
        //        }
        //        return null;
        //    }
        //}

        public static void AdicioanrContexto(SnHttpContext zyonHttpContext)
        {
            //var chave = Thread.CurrentThread.ManagedThreadId;
            //lock (((ICollection)ContextosAtivos).SyncRoot)
            //{
            //    if (ContextosAtivos.ContainsKey(chave))
            //    {
            //        ContextosAtivos[chave] = zyonHttpContext;
            //    }
            //    else
            //    {
            //        ContextosAtivos.Add(chave, zyonHttpContext);
            //    }
            //}
        }

        public static void RemoverContexto(SnHttpContext zyonHttpContext)
        {

            //var chave = Thread.CurrentThread.ManagedThreadId;
            //lock (((ICollection)ContextosAtivos).SyncRoot)
            //{
            //    if (ContextosAtivos.ContainsKey(chave))
            //    {
            //        ContextosAtivos.Remove(chave);
            //    }

            //}
        }

    }
}
