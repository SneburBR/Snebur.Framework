using System.ComponentModel.DataAnnotations.Schema;

namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property)]
public class ChaveEstrangeiraRelacaoAttribute : ForeignKeyAttribute, IChaveEstrangeiraAttribute
{
    public string NomePropriedade { get; set; }

    public ChaveEstrangeiraRelacaoAttribute(string nomePropriedade): base(nomePropriedade)
    {
        this.NomePropriedade = nomePropriedade;
    }
}
