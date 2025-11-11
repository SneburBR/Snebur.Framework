namespace Snebur.Utils.Tests;

public class FormatacaoUtilTests
{
    [Fact]
    public void PrimeiraLetraMaiuscula()
    {
        var s = TextoUtil.RetornarPrimeiraLetraMaiuscula("snebur framework");
        s.Should().Be("Snebur framework");
    }

    [Theory]
    [InlineData(1024L, "^1[.,]00 Kb$")]
    [InlineData(1536L, "^1[.,]50 Kb$")]
    public void RetornarTamnhoArquivoHumano(long bytes, string expectedPattern)
    {
        var result = FormatarByteUtil.Formatar(bytes, EnumFormatacaoBytes.Kilobytes, 2);
        result.Should().MatchRegex(expectedPattern);
    }
}
