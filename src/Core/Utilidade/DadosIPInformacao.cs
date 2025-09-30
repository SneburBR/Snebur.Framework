namespace Snebur.Dominio
{
    public class DadosIPInformacao : BaseDominio, IIPInformacao
    {
        public required string IP { get; set; } = "";

        public required string MascaraIp4 { get; set; }

        public string? Cidade { get; set; }

        public string? CodigoPostal { get; set; }

        public string? Hostname { get; set; }

        public Localizacao Localizacao { get; set; } = new Localizacao();

        public string? Pais { get; set; }

        public string? ProvedorInternet { get; set; }

        public string? Regiao { get; set; }

        [IgnorarPropriedadeTS]
        public static DadosIPInformacao Vazio
        {
            get
            {
                return new DadosIPInformacao
                {
                    IP = IpUtil.Empty,
                    MascaraIp4= IpUtil.Empty,
                };
            }
        }

        internal static DadosIPInformacao SneburChapeco
        {
            get
            {
                var ipLocal = "177.128.0.19";
                return new DadosIPInformacao
                {
                    IP = ipLocal,
                    Hostname = "CHA-SNEBUR",
                    Regiao = "Santa Catarina",
                    Pais = "BR",
                    Cidade = "Chapecó",
                    Localizacao = new Localizacao(-27.1000, -52.6000),
                    ProvedorInternet = "AS262361 Deznet Telecom Ltda.",
                    MascaraIp4 = IpUtil.RetornarMascaraIp4(ipLocal)
                };
            }
        }
    }
}