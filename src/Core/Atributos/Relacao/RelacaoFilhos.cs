namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property)]
public class RelacaoFilhosAttribute : BaseRelacaoAttribute
{
    public string? NomePropriedadeChaveEstrangeira { get; set; }

    public RelacaoFilhosAttribute(string nomePropriedadeChaveEstrangeira)
    {
        this.NomePropriedadeChaveEstrangeira = nomePropriedadeChaveEstrangeira;
    }

    [IgnorarConstrutorTS]
    public RelacaoFilhosAttribute()
    {
        this.NomePropriedadeChaveEstrangeira = null;
    }
}