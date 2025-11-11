namespace Snebur.Utils.Tests;

public class ChecksumUtilTests
{
    [Fact]
    public void MD5_KnownVector()
    {
        var bytes = Encoding.ASCII.GetBytes("123456789");
        var md5 = ChecksumUtil.RetornarChecksum(bytes);
        md5.Should().Be("25f9e794323b453885f5181f1b624d0b");
    }
}
