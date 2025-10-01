namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property)]
public abstract class BaseRelacaoPaiAttribute : RelacaoChaveEstrangeiraAttribute, IIgnorarAlerta
{
    public bool IgnorarAlerta { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public class RelacaoPaiAttribute : BaseRelacaoPaiAttribute
{
    public string? NomePropriedadeLinkNavegacao { get; set; }

    public EnumTipoExclusaoRelacao TipoExclusao { get; set; } = EnumTipoExclusaoRelacao.NaoDeletar;

    public RelacaoPaiAttribute(
        [ParametroOpcionalTS]
        string? nomePropriedadeLinkNavegacao,
        [ParametroOpcionalTS]
        EnumTipoExclusaoRelacao tipoExclusao)
    {
        this.TipoExclusao = tipoExclusao;
        this.NomePropriedadeLinkNavegacao = nomePropriedadeLinkNavegacao;
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