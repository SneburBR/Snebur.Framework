namespace Snebur.Utils.Tests;

public class UriUtilTests
{
    [Fact]
    public void UriJoin_CombinesPathsCorrectly()
    {
        var baseUrl = "https://example.com/api/";
        var path = "v1/items";
        var combined = UriUtil.CombinarCaminhos(baseUrl, path);
        combined.Should().Be("https://example.com/api/v1/items");
    }
}
