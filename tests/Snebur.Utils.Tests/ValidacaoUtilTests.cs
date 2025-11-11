namespace Snebur.Utils.Tests;

public class ValidacaoUtilTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("with space@example.com", false)]
    [InlineData("user.name@example.com", true)]
    [InlineData("a@b.co", true)]
    [InlineData("a@b", false)]
    public void IsEmail_Works(string? input, bool expected)
        => ValidacaoUtil.IsEmail(input).Should().Be(expected);

    [Theory]
    [InlineData("11 91234-5678", true)]
    [InlineData("(11) 1234-5678", true)]
    [InlineData("5511912345678", false)] // international-like, not supported by IsTelefone
    [InlineData(null, false)]
    [InlineData("abc", false)]
    public void IsTelefone_Basic(string? input, bool expected)
        => ValidacaoUtil.IsTelefone(input).Should().Be(expected);

    [Theory]
    [InlineData("12345-678", true)]
    [InlineData("12345678", true)]
    [InlineData("12345", true)] // library accepts 5 or 8 digits
    [InlineData("1234", false)]
    [InlineData(null, false)]
    public void IsCep_Validations(string? input, bool expected)
        => ValidacaoUtil.IsCep(input).Should().Be(expected);

    [Theory]
    [InlineData("52998224725", true)] // known valid CPF
    [InlineData("11111111111", false)]
    [InlineData("123", false)]
    [InlineData(null, false)]
    public void IsCpf_Validations(string? input, bool expected)
        => ValidacaoUtil.IsCpf(input).Should().Be(expected);

    [Theory]
    [InlineData("11.444.777/0001-61", true)] // common valid example
    [InlineData("11444777000161", true)]
    [InlineData("11111111111111", false)]
    [InlineData(null, false)]
    public void IsCnpj_Validations(string? input, bool expected)
        => ValidacaoUtil.IsCnpj(input).Should().Be(expected);

    [Fact]
    public void IsCpfOuCnpj_Works()
    {
        ValidacaoUtil.IsCpfOuCpj("52998224725").Should().BeTrue();
        ValidacaoUtil.IsCpfOuCpj("11444777000161").Should().BeTrue();
        ValidacaoUtil.IsCpfOuCpj("123").Should().BeFalse();
    }

    [Theory]
    [InlineData("127.0.0.1", true)]
    [InlineData("0.0.0.0", false)]
    [InlineData("999.0.0.1", false)]
    [InlineData(null, false)]
    public void IsIp_Validations(string? ip, bool expected)
        => ValidacaoUtil.IsIp(ip).Should().Be(expected);

    [Theory]
    [InlineData("1.2.3.4", true)]
    [InlineData("1.2.3", false)]
    [InlineData("1.2.-1.0", false)]
    [InlineData("a.b.c.d", false)]
    [InlineData(null, false)]
    public void IsVersao_Validations(string? v, bool expected)
        => ValidacaoUtil.IsVersao(v).Should().Be(expected);

    [Theory]
    [InlineData("example.com", true)]
    [InlineData("sub.domain.co.uk", true)]
    [InlineData("invalid_domain", false)]
    [InlineData(null, false)]
    public void IsDominioDns_FormatValidation(string? domain, bool expected)
        => ValidacaoUtil.IsDominioDns(domain).Should().Be(expected);

    [Theory]
    [InlineData("25f9e794323b453885f5181f1b624d0b", true)]
    [InlineData("900150983CD24FB0D6963F7D28E17F72", true)] // case-insensitive
    [InlineData("short", false)]
    [InlineData(null, false)]
    public void IsMd5_Regex(string? value, bool expected)
        => ValidacaoUtil.IsMd5(value).Should().Be(expected);

    [Fact]
    public void IsGuid_Various()
    {
        var g = Guid.NewGuid();
        ValidacaoUtil.IsGuid(g.ToString()).Should().BeTrue();
        ValidacaoUtil.IsGuid(g.ToString("N")).Should().BeTrue();
        ValidacaoUtil.IsGuid("not-a-guid").Should().BeFalse();
        ValidacaoUtil.IsGuid(null).Should().BeFalse();
    }

    [Theory]
    [InlineData("a b", true)]
    [InlineData("ab", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsPossuiEspacoEmBranco_Tests(string? s, bool expected)
        => ValidacaoUtil.IsPossuiEspacoEmBranco(s).Should().Be(expected);

    [Fact]
    public void IsEmailOuTelefone_Tests()
    {
        ValidacaoUtil.IsEmailOuTelefone("user@example.com").Should().BeTrue();
        ValidacaoUtil.IsEmailOuTelefone("11 91234-5678").Should().BeTrue();
        ValidacaoUtil.IsEmailOuTelefone("invalid").Should().BeFalse();
    }

    [Fact]
    public void NomeChecks()
    {
        ValidacaoUtil.IsNomeCompleto("Joao da Silva").Should().BeTrue();
        ValidacaoUtil.IsPossuiSobrenome("Joao Silva").Should().BeTrue();
        ValidacaoUtil.IsPossuiPrimeiroNome("A B").Should().BeFalse();
        ValidacaoUtil.IsPossuiPrimeiroNome("Jo Ana").Should().BeTrue();
    }

    [Theory]
    [InlineData("#fff", true)]
    [InlineData("#1a2b3c", true)]
    [InlineData("#AABBCCDD", true)]
    [InlineData("fff", false)]
    [InlineData("#zzzzzz", false)]
    public void IsCorHexa_Tests(string value, bool expected)
        => ValidacaoUtil.IsCorHexa(value).Should().Be(expected);

    [Theory]
    [InlineData("rgb(255, 0, 10)", true)]
    [InlineData("rgba(255,255,255,0.5)", true)]
    [InlineData("rgba(1,2,3,)", false)]
    [InlineData("rgb(,)", false)]
    public void IsCorRgbOuRgba_Tests(string value, bool expected)
        => ValidacaoUtil.IsCorRgbOuRgba(value).Should().Be(expected);

    [Theory]
    [InlineData("/v1/items/", true)]
    [InlineData("/v1/items", false)]
    [InlineData(null, false)]
    public void IsRota_Tests(string? rota, bool expected)
        => ValidacaoUtil.IsRota(rota).Should().Be(expected);
}

