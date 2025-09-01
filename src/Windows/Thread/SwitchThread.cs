using System.Runtime.CompilerServices;

namespace Snebur.Windows;

/*credits https://medium.com/criteo-engineering/switching-back-to-the-ui-thread-in-wpf-uwp-in-modern-c-5dc1cc8efa5e */
public static class DispatcherExtensions
{
    public static SwitchToMainThreadAwaitable SwitchToMainThreadAsync(this Dispatcher dispatcher)
    {
        return new SwitchToMainThreadAwaitable(dispatcher);
    }

    public static SwitchToWorkThreadAwaitable SwitchToWorkerThreadAsync(this Dispatcher dispatcher)
    {
        return new SwitchToWorkThreadAwaitable(dispatcher);
    }
}
public readonly struct SwitchToMainThreadAwaitable : INotifyCompletion
{
    private readonly Dispatcher _dispatcher;

    public SwitchToMainThreadAwaitable(Dispatcher dispatcher)
    {
        this._dispatcher = dispatcher;
    }

    public SwitchToMainThreadAwaitable GetAwaiter()
    {
        return this;
    }

    public void GetResult()
    {
    }

    public bool IsCompleted => this._dispatcher.CheckAccess();

    public void OnCompleted(Action continuation)
    {
        this._dispatcher.BeginInvoke(continuation);
    }
}

public readonly struct SwitchToWorkThreadAwaitable : INotifyCompletion
{

    private readonly Dispatcher _dispatcher;

    public SwitchToWorkThreadAwaitable(Dispatcher dispatcher)
    {
        this._dispatcher = dispatcher;
    }

    public SwitchToWorkThreadAwaitable GetAwaiter()
    {
        return this;
    }

    public void GetResult()
    {
    }

    public bool IsCompleted => !this._dispatcher.CheckAccess();

    public void OnCompleted(Action continuation)
    {
        Task.Run(continuation);
        //_dispatcher.BeginInvoke(continuation);
    }
}
