namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property)]
public class RelacaoUmUmFilhoAttribute : RelacaoPaiAttribute, IIgnorarAlerta
{
    public RelacaoUmUmFilhoAttribute()
    {

    }
}

[AttributeUsage(AttributeTargets.Property)]
public class RelacaoUmUmPaiAttribute : RelacaoPaiAttribute, IIgnorarAlerta
{
    public RelacaoUmUmPaiAttribute()
    {

    }
}