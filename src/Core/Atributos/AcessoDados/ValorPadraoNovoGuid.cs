namespace Snebur.Dominio.Atributos;

[AttributeUsage(AttributeTargets.Property)]
public class ValorPadraoNovoGuidAttribute : Attribute, IValorPadrao
{
    public bool IsValorPadraoOnUpdate { get; set; } = false;
    public bool IsString { get; set; }
    public bool IsRemoverTracos { get; set; }

    public object RetornarValorPadrao(object contexto,
                                     Entidade entidade,
                                     object? valorPropriedade)
    {
        if(valorPropriedade is string str && !String.IsNullOrWhiteSpace(str))
        {
            return valorPropriedade;
        }
        var guid = Guid.NewGuid();
        if (this.IsString || this.IsRemoverTracos)
        {
            if (this.IsRemoverTracos)
            {
                return guid.ToString().Replace("-", "");
            }
            return guid.ToString();
        }
        return guid;
    }

    #region IValorPadrao 

    public bool IsTipoNullableRequerido { get { return false; } }

    #endregion
}