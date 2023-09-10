using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System;
using System.Linq;
using System.Reflection;

namespace Snebur.AcessoDados
{
    internal class OrdenacaoUtil
    {
        internal static PropertyInfo RetornarPropriedadeOrdenacao(Type tipoEntidade)
        {
            var nomePropriedade = nameof(IOrdenacao.Ordenacao);
            var propriedade = ReflexaoUtil.RetornarPropriedade(tipoEntidade, nomePropriedade, true);
            if (propriedade != null)
            {
                return propriedade;
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
                        return propriedadeMapeada;
                    }
                }
            }
            throw new Erro($"Não foi possível encontrar a propriedade {nomePropriedade} na entidade {tipoEntidade.Name}");
        }
    }
}
