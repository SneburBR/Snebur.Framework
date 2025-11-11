namespace Snebur.Utils.Tests;

public class DiretorioUtilTests
{
    [Fact]
    public void FormatarNomePasta_RemovesInvalidChars()
    {
        var raw = "invalido<>:\"|?*";
        var formatted = DiretorioUtil.FormatarNomePasta(raw);
        formatted.Should().NotContainAny(System.IO.Path.GetInvalidPathChars().Select(x=> x.ToString()).ToArray<string>());
        formatted.Should().NotContainAny(System.IO.Path.GetInvalidFileNameChars().Select(x=> x.ToString()).ToArray<string>());
    }

    [Fact]
    public void IsDiretorioRaiz_RecognizesRoots()
    {
        var root = System.IO.Path.GetPathRoot(Environment.CurrentDirectory);
        DiretorioUtil.IsDireotrioRaiz(root!).Should().BeTrue();
        DiretorioUtil.IsDireotrioRaiz("not/root").Should().BeFalse();
    }
}

