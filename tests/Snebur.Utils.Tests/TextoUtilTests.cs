namespace Snebur.Utils.Tests;

public class TextoUtilTests
{
    [Theory]
    [InlineData("ação", "acao")]
    [InlineData("ÁÉÍÓÚÇ", "AEIOUC")]
    [InlineData(null, "")] // null -> string.Empty
    public void RemoverAcentos_Works(string? input, string expected)
    {
        var result = TextoUtil.RemoverAcentos(input);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("a1b2c3", "123")]
    [InlineData("  99,50 ", "9950")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void RetornarSomenteNumeros_Basic(string? input, string expected)
    {
        var result = TextoUtil.RetornarSomenteNumeros(input);
        result.Should().Be(expected);
    }

    [Fact]
    public void RemoverCaracteresEspecial_RemovesAndOptionallyReplaces()
    {
        var input = "nome@inválido!.txt";
        // CaracteresPadrao permite '.', '@' e '!', portanto permanece igual
        TextoUtil.RemoverCaracteresEspecial(input).Should().Be(input);
        TextoUtil.RemoverCarecteres(input, new HashSet<char> { '@', '!', '.' }, '_')
            .Should().Be("nome_inválido__txt");
    }

    [Fact]
    public void DividirLetraMaiuscula_SplitsCamelCase()
    {
        var parts = TextoUtil.DividirLetraMaiuscula("ClienteAtivoVIP");
        parts.Should().Equal("Cliente", "Ativo", "VIP");
    }
}
