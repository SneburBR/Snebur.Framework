using Snebur.Utils.Tests.Helpers;

namespace Snebur.Utils.Tests;

public class ArquivoUtilTests
{
    [Fact]
    public void AlterarExtensaoNomeArquivo_ChangesExtensionOnly()
    {
        using var dir = new TempDir();
        var path = System.IO.Path.Combine(dir.Path, "data.json");
        var changed = ArquivoUtil.AlterarExtensaoNomeArquivo(path, ".txt");
        System.IO.Path.GetFileName(changed).Should().Be("data.txt");
        System.IO.Path.GetDirectoryName(changed).Should().Be(System.IO.Path.GetDirectoryName(path));
    }

    [Fact]
    public void SalvarStream_WritesBytes()
    {
        using var dir = new TempDir();
        var dest = System.IO.Path.Combine(dir.Path, "out.bin");
        using var ms = new MemoryStream(new byte[] { 1, 2, 3, 4 });
        ArquivoUtil.SalvarStream(ms, dest).Should().BeTrue();
        File.ReadAllBytes(dest).Should().Equal(1, 2, 3, 4);
    }
}

