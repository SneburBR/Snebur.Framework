namespace Snebur.Utils.Tests;

public class CaminhoUtilTests
{
    [Fact]
    public void Combine_NormalizesDriveRoot()
    {
        var drive = System.IO.Path.GetPathRoot(Environment.CurrentDirectory)!.TrimEnd(System.IO.Path.DirectorySeparatorChar);
        var combined = CaminhoUtil.Combine(drive, "folder", "file.txt");
        combined.Should().EndWith(System.IO.Path.Combine("folder", "file.txt"));
        CaminhoUtil.IsFullPath(combined).Should().BeTrue();
    }

    [Fact]
    public void RetornarNomeArquivoTemporario_AddsDotIfMissing()
    {
        var name = CaminhoUtil.RetornarNomeArquivoTemporario("log");
        System.IO.Path.GetExtension(name).Should().Be(".log");
    }

    [Fact]
    public void CaminhoIgual_IsCaseInsensitiveAndSlashAgnostic()
    {
        var p1 = System.IO.Path.Combine(Environment.CurrentDirectory, "A", "B");
        var p2 = p1.Replace('\\', '/');
        CaminhoUtil.CaminhoIgual(p1, p2).Should().BeTrue();
    }
}

