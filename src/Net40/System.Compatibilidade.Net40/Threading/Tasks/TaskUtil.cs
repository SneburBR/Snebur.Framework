using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace System.Threading.Tasks
{
    public readonly struct VoidTaskResult { }
    public static class TaskUtil
    {
        public static Func<VoidTaskResult> Func = () => default;

        public static readonly Task<VoidTaskResult> s_cachedCompleted
            = new Task<VoidTaskResult>(() => default);

        public static Task CompletedTask()
            => s_cachedCompleted;

        public static async Task SalvarStreamBufferizadaAsync(Stream streamOrigem,
                                                             Stream streamDestino)
        {
            await Delay(0);
        }

        public static Task Delay(int delay)
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(delay);

            });
        }
        public static Task Run(Action action)
        {
            return Task.Factory.StartNew(action);
        }

        public static Task<T> Run<T>(Func<T> action)
        {
            return Task.Factory.StartNew<T>(action);
        }

        public static Task<Task> WhenAny(params Task[] tasks)
        {
            return Task.Factory.StartNew(() =>
            {
                Task.WaitAny(tasks);
                return TaskUtil.CompletedTask();
            });
        }

        public static Task<Task> WhenAny(IEnumerable<Task> tasks)
        {
            return Task.Factory.StartNew(() =>
            {
                Task.WaitAny(tasks.ToArray());
                return TaskUtil.CompletedTask();
            });

        }
       
        //public static Task WhenAny(params Task[] tasks)
        //{
        //    return Task.Factory.StartNew(() =>
        //    {
        //        Task.WaitAny(tasks);
        //    });
            
        //}
        //public static Task WhenAny(IEnumerable< Task> tasks, Task task)
        //{
        //    return Task.Factory.StartNew(() =>
        //    {
        //        Task.WaitAny(tasks,
        //    });

        //}
    }
}