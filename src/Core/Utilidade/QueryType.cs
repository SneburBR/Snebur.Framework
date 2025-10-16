namespace Snebur.Utilidade;

public enum QueryType
{
    [UndefinedEnumValue] Undefined = -1,
    A = 1,
    MX = 15,
    NS = 2,
    CNAME = 5,
    SOA = 6,
    PTR = 12,
    TXT = 16,
    AAAA = 28,
    SRV = 33,
    ANY = 255
}