namespace Snebur.Windows;

public static class DispatcherExtensao
{
    public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Action acao)
    {
        return dispatcher.BeginInvoke((Delegate)acao);
    }
}
