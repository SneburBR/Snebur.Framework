namespace Snebur.Utils.Tests;

public class UtilTests
{
    [Theory]
    [InlineData("2024-12-31", true)]
    [InlineData("not a date", false)]
    public void IsDate_HandlesStrings(string input, bool expected)
    {
        Util.IsDate(input).Should().Be(expected);
    }

    [Theory]
    [InlineData(123, true)]
    [InlineData("3.14", true)]
    [InlineData("abc", false)]
    public void IsNumeric_HandlesCommonCases(object value, bool expected)
    {
        Util.IsNumeric(value).Should().Be(expected);
    }

    [Fact]
    public void SaoIgual_NullsAndStrings()
    {
        Util.SaoIgual(null, null).Should().BeTrue();
        Util.SaoIgual(null, "").Should().BeTrue();
        Util.SaoIgual("", null).Should().BeTrue();
        Util.SaoIgual("a", null).Should().BeFalse();
        Util.SaoIgual(null, "a").Should().BeFalse();
    }

    private sealed class Sample
    {
        public string? Name { get; set; }
        public Sample? Child { get; set; }
    }

    [Fact]
    public void RetornarTodosObjetoTipo_FindsNested()
    {
        var s = new Sample
        {
            Name = "root",
            Child = new Sample { Name = "child" }
        };
        var found = Util.RetornarTodosObjetoTipo<Sample>(s);
        found.Should().HaveCount(2);
    }
}

