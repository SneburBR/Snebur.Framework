namespace Snebur.Dominio.Atributos;

//TODO: Rename to DomainPropyPropertyAttribute
[AttributeUsage(AttributeTargets.Property)]
public class PropriedadeTSEspecializadaAttribute : BaseAtributoDominio
{
    public string NomePropriedade { get; set; }

    public PropriedadeTSEspecializadaAttribute(string nomePropriedade)
    {
        this.NomePropriedade = nomePropriedade;
    }
}