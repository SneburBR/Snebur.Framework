namespace Snebur.Utils.Tests.Helpers;

public sealed class TempDir : IDisposable
{
    public string Path { get; }

    public TempDir(string? nameHint = null)
    {
        var root = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Snebur.Tests");
        Directory.CreateDirectory(root);
        var name = (nameHint ?? Guid.NewGuid().ToString("n")).Trim();
        Path = System.IO.Path.Combine(root, name + "_" + Guid.NewGuid().ToString("n").Substring(0, 6));
        Directory.CreateDirectory(Path);
    }

    public string Combine(params string[] parts) => System.IO.Path.Combine(new[] { Path }.Concat(parts).ToArray());

    public void Dispose()
    {
        try { Directory.Delete(Path, recursive: true); } catch { /* ignore */ }
    }
}

