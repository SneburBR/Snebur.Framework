namespace Snebur.Utilidade
{
    public static class NormalizacaoUtil
    {
        public static string NormlizarHost(string _host)
        {
            var posicao = _host.IndexOf(':');
            if (posicao >= 0)
            {
                _host = _host.Substring(posicao + 1);
                posicao = _host.IndexOf(':');
                if (posicao > 0)
                {
                    _host = _host.Substring(0, posicao);
                }
            }
            return _host?.ToLower();
        }
    }
}
