namespace Snebur.Utils.Tests;

public class EnumUtilTests
{
    private enum Sample
    {
        None = 0,
        One = 1,
        Two = 2
    }

    [Fact]
    public void TryRetornarValorEnum_AndDefined()
    {
        var parsed = EnumUtil.TryRetornarValorEnum<Sample>("One");
        parsed.Should().NotBeNull();
        parsed!.Value.Should().Be(Sample.One);

        Enum.IsDefined(typeof(Sample), Sample.Two).Should().BeTrue();
        Enum.IsDefined(typeof(Sample), (Sample)999).Should().BeFalse();
    }
}
