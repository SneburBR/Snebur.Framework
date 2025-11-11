using System.Diagnostics;

namespace Snebur.Utilidade;

public static class TraceUtil
{
    public static void Error(Func<string> value)
    {
        Write(value, Category.Error);
        Debugger.Break();
    }

    public static void Verbose(Func<string> value)
    {
        Write(value, Category.Verbose);
    }

    public static void Info(Func<string> value)
    {
        Write(value, Category.Info);
    }

    // Using Fun<string> will avoid unnecessary string allocation when debugger is not attached
    public static void Warning(Func<string> messageFactory)
    {
        Write(messageFactory, Category.Warning);
    }

    public static void Timing(
        Func<string> messageFactory,
        Stopwatch stopwatch )
    {
        if (!DebugUtil.IsAttached)
        {
            return;
        }

        var category = stopwatch.ElapsedMilliseconds > 1000
             ? Category.LongDelay
             : Category.Timing;

        Write(messageFactory, category);
    }
    private static void Write(Func<string> value, Category category)
    {
        if (DebugUtil.IsAttached)
        {
            var message = value();
            Trace.WriteLine($"[{category}] {message}", category.ToString());
        }
    }

    private enum Category
    {
        Info,
        Warning,
        Error,
        Verbose,
        Timing,
        LongDelay
    }
}
