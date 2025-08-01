﻿namespace Snebur.AcessoDados.Ajudantes;

internal class AjudanteConsultaEntidade
{

    public static string RetornarCaminhoPropriedade(Expression expressao)
    {
        var propriedades = ExpressaoUtil.RetornarPropriedades(expressao);
        return AjudanteConsultaEntidade.RetornarCaminhoPropriedade(propriedades);
    }

    public static string RetornarCaminhoPropriedade(List<PropertyInfo> propriedades)
    {
        return String.Join(".", propriedades.Select(x => x.Name));
    }

    public static PropertyInfo RetornarPropriedadeInterface(Type tipoDeclarado,
                                                            PropertyInfo propriedadeInterface)
    {
        if (propriedadeInterface.DeclaringType?.IsInterface == false)
        {
            throw new InvalidOperationException($"a propriedade {propriedadeInterface.Name} não foi declarada em um interface");
        }

        var propriedade = ReflexaoUtil.RetornarPropriedade(tipoDeclarado, propriedadeInterface.Name, true);
        if (propriedade is null)
        {
            var nomePropriedadeExplicida = String.Format("{0}.{1}.{2}", propriedadeInterface.DeclaringType?.Namespace, propriedadeInterface.DeclaringType?.Name, propriedadeInterface.Name);
            propriedade = ReflexaoUtil.RetornarPropriedade(tipoDeclarado, nomePropriedadeExplicida);
        }
        
        var atributoProprieadeInterface = propriedade.GetCustomAttribute<PropriedadeInterfaceAttribute>();
        if (atributoProprieadeInterface != null)
        {
            propriedade = ReflexaoUtil.RetornarPropriedade(tipoDeclarado, atributoProprieadeInterface.NomePropriedade);
        }
        return propriedade;
    }

    #region Lista Entidades

    public static bool IsPropriedadeRetornarListaEntidade(PropertyInfo propriedade)
    {
        var tipoEntidade = AjudanteConsultaEntidade.RetornarTipoListaEntidade(propriedade.PropertyType);
        if (!(tipoEntidade == null))
        {
            return tipoEntidade.IsSubclassOf(typeof(Entidade)) ||
                   ReflexaoUtil.IsTipoImplementaInterface(tipoEntidade, typeof(IEntidade), false);
        }
        return false;
    }

    public static Type? RetornarTipoListaEntidade(Type tipoDeclarado, PropertyInfo propriedade)
    {
        if (propriedade.DeclaringType?.IsInterface == true)
        {
            propriedade = AjudanteConsultaEntidade.RetornarPropriedadeInterface(tipoDeclarado, propriedade);
        }
        var tipoEntidade = AjudanteConsultaEntidade.RetornarTipoListaEntidade(propriedade.PropertyType);
        if (tipoEntidade?.IsSubclassOf(typeof(Entidade)) == false)
        {
            throw new ErroNaoSuportado(String.Format("O Tipo não é um entidade {0}", tipoEntidade.Name));
        }
        return AjudanteConsultaEntidade.RetornarTipoListaEntidade(propriedade.PropertyType);
    }

    #endregion

    #region Metodos Privados

    private static Type? RetornarTipoListaEntidade(Type? tipo)
    {
        if (tipo == null)
        {
            return null;
        }
        if (tipo.IsGenericType)
        {
            var tiposGenerico = tipo.GetGenericArguments();
            if (tiposGenerico.Count() > 1)
            {
                var mensagemErroTipoGenerico = String.Format("O Tipo possui mais de argumento genericos {0} ", String.Join(",", tiposGenerico.Select(x => x.Name)));
                throw new Erro(mensagemErroTipoGenerico);
            }
            return tiposGenerico.Single();
        }
        return AjudanteConsultaEntidade.RetornarTipoListaEntidade(tipo.BaseType);
    }
    #endregion
}