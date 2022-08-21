using Snebur.Utilidade;

namespace System
{
    public static class DateTimeExtensao
    {
        public static DateTime DataInicioUnix = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static long RetornarHashValidacaoUnico(this DateTime dateTime)
        {
            var dias = (int)Math.Floor((dateTime - DataInicioUnix).TotalDays);
            return (long)new TimeSpan(dias, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond).TotalMilliseconds;
        }

        public static DateTime RetornarDataComHoraZerada(this DateTime dataHora)
        {
            return new DateTime(dataHora.Hour, dataHora.Month, dataHora.Day, 0, 0, 0);
        }

        public static long RetornarMilisegundosJavascript(this DateTime dataHora, bool isUtc = false)
        {
            if (isUtc && dataHora.Kind != DateTimeKind.Utc)
            {
                dataHora = dataHora.ToUniversalTime();
            }
            return DataHoraUtil.RetornarMiliesegundosJavascript(dataHora, isUtc);
        }

        public static bool Equals(this DateTime? data,
                                  DateTime? dataComparar,
                                  OpcoesCompararData opcoesData,
                                  OpcoesCompararHora opcoesHora = OpcoesCompararHora.Ignorar)
        {
            if (data.HasValue && dataComparar.HasValue)
            {
                return DataHoraUtil.SaoIgual(data.Value,
                                             dataComparar.Value,
                                             opcoesData,
                                             opcoesHora);
            }
            return false;
        }
        public static bool Equals(this DateTime data,
                                  DateTime? dataComparar,
                                  OpcoesCompararData opcoesData,
                                  OpcoesCompararHora opcoesHora = OpcoesCompararHora.Ignorar)
        {
            if (dataComparar.HasValue)
            {
                return DataHoraUtil.SaoIgual(data,
                                             dataComparar.Value,
                                             opcoesData,
                                             opcoesHora);
            }
            return false;
        }

        public static DateTime DataZeroHora(this DateTime data, bool isUtc = true)
        {
            var tipo = isUtc ? DateTimeKind.Utc : DateTimeKind.Local;
            return new DateTime(data.Year, data.Month, data.Day, 0, 0, 0, tipo);
        }
    }
}
