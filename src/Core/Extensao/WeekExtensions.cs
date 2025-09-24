using System.Threading;
using System.Threading.Tasks;

namespace Snebur.Extensao;

public static class WeakReferenceExtensions
{
    public static async Task WaitForUnloadAsync(this WeakReference weak, CancellationToken ct)
    {
        for (int i = 0; i < 20; i++)
        {
            if (!weak.IsAlive) return;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.WaitForFullGCComplete();
            GC.WaitForFullGCApproach();
            await Task.Delay(50, ct);
        }

        // If still alive, it means there are references somewhere
        // This is a signal to look for static events or cached types outside the ALC
    }
}
