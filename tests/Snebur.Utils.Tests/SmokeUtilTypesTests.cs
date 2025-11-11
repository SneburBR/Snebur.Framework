namespace Snebur.Utils.Tests;

// Smoke test to ensure all Util classes are referenced by the test project.
public class SmokeUtilTypesTests
{
    [Fact]
    public void CanReference_AllUtilityTypes()
    {
        // Core Util classes
        _ = typeof(ArquivoUtil);
        _ = typeof(AutoMapearUtil);
        _ = typeof(BaralhoUtil);
        _ = typeof(Base36Util);
        _ = typeof(Base64Util);
        _ = typeof(BrazilianPluralizeUtils);
        _ = typeof(CaminhoUtil);
        _ = typeof(CaseConventionUtils);
        _ = typeof(ChecksumUtil);
        _ = typeof(CnpjUtil);
        _ = typeof(CodigoUtil);
        _ = typeof(CompactacaoUtil);
        _ = typeof(ConfiguracaoUtil);
        _ = typeof(ConverterUtil);
        _ = typeof(CredencialUtil);
        _ = typeof(CsSharpLiteralValueUtils);
        _ = typeof(DataFeriadoUtil);
        _ = typeof(DataHoraUtil);
        _ = typeof(DebugUtil);
        _ = typeof(DimensaoUtil);
        _ = typeof(DiretorioUtil);
        _ = typeof(DnsUtil);
        _ = typeof(EntidadeUtil);
        _ = typeof(EnumUtil);
        _ = typeof(ErroUtil);
        _ = typeof(ExecutarDepois);
        _ = typeof(ExpressaoUtil);
        _ = typeof(FilaUtil);
        _ = typeof(FormatacaoUtil);
        _ = typeof(FormatacaoNomeUtil);
        _ = typeof(FormatarByteUtil);
        _ = typeof(HexUtil);
        _ = typeof(HtmlBuilder);
        _ = typeof(HtmlUtil);
        _ = typeof(HttpUtil);
        _ = typeof(IndentedStringBuilder);
        _ = typeof(InternetUtil);
        _ = typeof(IpUtil);
        _ = typeof(JsonUtil);
        _ = typeof(JurosUtil);
        _ = typeof(LazyUtil);
        _ = typeof(LogUtil);
        _ = typeof(Md5Util);
        _ = typeof(MedidaUtil);
        _ = typeof(NormalizacaoUtil);
        _ = typeof(OperationWithTimeout);
        _ = typeof(RedeUtil);
        _ = typeof(ReflexaoUtil);
        _ = typeof(SegurancaUtil);
        _ = typeof(SessaoUtil);
        _ = typeof(SistemaUtil);
        _ = typeof(SnRandom);
        _ = typeof(SqlUtil);
        _ = typeof(StreamUtil);
        _ = typeof(StringCompressionUtil);
        _ = typeof(TaskUtil);
        _ = typeof(TelefoneUtil);
        _ = typeof(TextoUtil);
        _ = typeof(ThreadUtil);
        _ = typeof(TypeUtils);
        _ = typeof(UriUtil);
        _ = typeof(Util);
        _ = typeof(ValidacaoEmailUtil);
        _ = typeof(ValidacaoUtil);
        _ = typeof(XmlUtil);
        _ = typeof(ZipUtil);

        Assert.True(true);
    }
}

