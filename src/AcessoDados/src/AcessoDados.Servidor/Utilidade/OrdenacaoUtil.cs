namespace Snebur.AcessoDados
{
    internal class OrdenacaoUtil
    {
        internal static (PropertyInfo, OrdenacaoOpcoesAttribute) RetornarPropriedadeOrdenacao(Type tipoEntidade)
        {
            var nomePropriedade = nameof(IOrdenacao.Ordenacao);
            var propriedade = ReflexaoUtil.RetornarPropriedade(tipoEntidade, nomePropriedade, true);
            if (propriedade != null)
            {
                var atributo = propriedade.GetCustomAttribute<OrdenacaoOpcoesAttribute>();
                return (propriedade, atributo);
            }

            var metodos = tipoEntidade.GetInterfaceMap(typeof(IOrdenacao)).TargetMethods;
            propriedade = tipoEntidade.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                                                    .Where(p => metodos.Contains(p.GetGetMethod(true)))
                                                    .Where(prop => metodos.Contains(prop.GetSetMethod(true)))
                                                    .FirstOrDefault();

            if (propriedade != null)
            {
                var atributo = propriedade.GetCustomAttribute<OrdenacaoOpcoesAttribute>();
                if (!String.IsNullOrWhiteSpace(atributo.NomePropriedadeMapeada))
                {
                    var propriedadeMapeada = ReflexaoUtil.RetornarPropriedade(tipoEntidade, atributo.NomePropriedadeMapeada, true);
                    if (propriedadeMapeada != null)
                    {
                        return (propriedadeMapeada, atributo);
                    }
                }
            }
            throw new Erro($"Não foi possível encontrar a propriedade {nomePropriedade} na entidade {tipoEntidade.Name}");
        }
    }
}
