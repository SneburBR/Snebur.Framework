namespace Snebur.AcessoDados.Mapeamento;

internal abstract class BaseEstruturaApelido
{
    internal string ApelidoOriginal { get; set; }

    internal string Apelido { get; set; }

    internal string CaminhoBanco { get; set; }

    private int ContadorApelido { get; set; }

    internal BaseMapeamentoConsulta MapeamentoConsulta { get; set; }

    public BaseEstruturaApelido(BaseMapeamentoConsulta mapeamentoConsulta,
                                string apelido,
                                string caminhoBanco)
    {
        this.MapeamentoConsulta = mapeamentoConsulta;
        this.ContadorApelido = this.MapeamentoConsulta.RetornarContadorApelido();
        this.ApelidoOriginal = apelido;

        this.CaminhoBanco = caminhoBanco;
        this.Apelido = this.RetornarApelido();
        this.MapeamentoConsulta.EstruturasApelido.Add(this.Apelido, this);
    }

    private string RetornarApelido(bool unico = false)
    {
        if (ConfiguracaoAcessoDados.ApelidoUnicoCurto)
        {
            var prefixo = this.RetornarPrefixoApelido();
            return String.Format("[{0}_{1}]", prefixo, this.ContadorApelido);
        }
        else
        {
            var apelido = this.ApelidoOriginal;

            if (apelido.StartsWith("["))
            {
                apelido = apelido.Substring(1);
            }
            if (apelido.EndsWith("]"))
            {
                apelido = apelido.Substring(0, apelido.Length - 1);
            }
            if (apelido.Length > 60 || unico)
            {
                var prefixo = this.RetornarPrefixoApelido();
                apelido = String.Format("{0}_{1}_{2}", prefixo, this.ContadorApelido, apelido).RetornarPrimeirosCaracteres(60);
                apelido = String.Format("[{0}]", apelido);
                return apelido;

            }
            else
            {
                apelido = String.Format("[{0}]", apelido);
                if (this.MapeamentoConsulta.EstruturasApelido.ContainsKey(apelido))
                {
                    return this.RetornarApelido(true);
                }
                return apelido;
            }
        }
    }

    private string RetornarPrefixoApelido()
    {
        var tipo = this.GetType();
        if (tipo == typeof(EstruturaCampoApelido))
        {
            return "C";
        }
        else if (tipo.IsSubclassOf(typeof(BaseEstruturaEntidadeApelido)))
        {
            return "E";
        }
        else
        {
            throw new NotImplementedException("Prefixo do apelido não é suportado");
        }
    }
}