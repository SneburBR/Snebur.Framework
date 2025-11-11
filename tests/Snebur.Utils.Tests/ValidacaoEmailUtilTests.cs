namespace Snebur.Utils.Tests;

public class ValidacaoEmailUtilTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid-without-at")]
    public void IsExisteEmail_NullOrInvalid_ReturnsFalse(string? email)
    {
        var result = ValidacaoEmailUtil.IsExisteEmail(email);
        result.Should().BeFalse();
    }

    // Network/DNS dependent positive checks are intentionally omitted to keep tests stable.
}

