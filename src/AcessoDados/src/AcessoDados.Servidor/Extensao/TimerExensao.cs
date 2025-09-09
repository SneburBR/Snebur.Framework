namespace System.Timers;

public static class TimerExensao
{
    public static void Reiniciar(this Timer timer)
    {
        timer.Stop();
        timer.Start();
    }
}
