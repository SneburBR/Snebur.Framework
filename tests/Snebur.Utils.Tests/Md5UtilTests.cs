namespace Snebur.Utils.Tests;

public class Md5UtilTests
{
    [Fact]
    public void Md5_KnownVector()
    {
        var hash = Md5Util.RetornarHash("abc");
        hash.Should().Be("900150983cd24fb0d6963f7d28e17f72");
    }
}

