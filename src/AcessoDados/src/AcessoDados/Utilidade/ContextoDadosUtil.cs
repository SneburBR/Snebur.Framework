namespace Snebur.AcessoDados;

public class ContextoDadosUtil
{
    public static List<PropertyInfo> RetornarPropriedadesIConsultaEntidade(Type tipoContexto)
    {
        return tipoContexto.GetProperties(ReflexaoUtil.BindingFlags).
                              Where(x => x.PropertyType.IsInterface &&
                                         ReflexaoUtil.IsTipoImplementaInterface(x.PropertyType, typeof(IConsultaEntidade), false)).ToList();
    }
}