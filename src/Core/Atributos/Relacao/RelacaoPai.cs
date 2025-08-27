namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property)]
public class RelacaoPaiAttribute : RelacaoChaveEstrangeiraAttribute, IIgnorarAlerta
{
    public bool IgnorarAlerta { get; set; }

    public bool IsRelacaoUmUm { get; set; }

    public string? NomePropriedadeLinkNavegacao { get; set; }

    public EnumTipoExclusaoRelacao TipoExclusao { get; set; } = EnumTipoExclusaoRelacao.NaoDeletar;

    public RelacaoPaiAttribute(
        string? nomePropriedadeNavegacao,
        EnumTipoExclusaoRelacao tipoExclusao)
    {
        this.TipoExclusao = tipoExclusao;
        this.NomePropriedadeLinkNavegacao = nomePropriedadeNavegacao;
    }

    [IgnorarConstrutorTS]
    public RelacaoPaiAttribute()
        : this(null, EnumTipoExclusaoRelacao.NaoDeletar)
    {

    }

    [IgnorarConstrutorTS]
    public RelacaoPaiAttribute(EnumTipoExclusaoRelacao tipoExclusao)
        : this(null, tipoExclusao)
    {

    }

    [IgnorarConstrutorTS]
    public RelacaoPaiAttribute(string nomePropriedadeNavegacao)
      : this(nomePropriedadeNavegacao, EnumTipoExclusaoRelacao.NaoDeletar)
    {

    }
}