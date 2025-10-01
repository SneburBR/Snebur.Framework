namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property)]
public class RelacaoUmUmFilhoAttribute : BaseRelacaoPaiAttribute, IIgnorarAlerta
{
    public RelacaoUmUmFilhoAttribute()
    {

    }
}

[AttributeUsage(AttributeTargets.Property)]
public class RelacaoUmUmPaiAttribute : BaseRelacaoPaiAttribute, IIgnorarAlerta
{
    public RelacaoUmUmPaiAttribute()
    {

    }
}