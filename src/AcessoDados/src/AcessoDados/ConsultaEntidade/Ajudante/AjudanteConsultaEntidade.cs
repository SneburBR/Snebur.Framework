using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Snebur.AcessoDados.Ajudantes
{
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
                                                                PropertyInfo ipropriedade)
        {
            if (!ipropriedade.DeclaringType.IsInterface)
            {
                throw new InvalidOperationException($"a propriedade {ipropriedade.Name} não foi declarada em um interface");
            }

            var propriedade = ReflexaoUtil.RetornarPropriedade(tipoDeclarado, ipropriedade.Name, true);
            if (propriedade == null)
            {
                var nomePropriedadeExplicida = String.Format("{0}.{1}.{2}", ipropriedade.DeclaringType.Namespace, ipropriedade.DeclaringType.Name, ipropriedade.Name);
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
                       ReflexaoUtil.TipoImplementaInterface(tipoEntidade, typeof(IEntidade), false);
            }
            return false;
        }

        public static Type RetornarTipoListaEntidade(Type tipoDeclarado, PropertyInfo propriedade)
        {
            if (propriedade.DeclaringType.IsInterface)
            {
                propriedade = AjudanteConsultaEntidade.RetornarPropriedadeInterface(tipoDeclarado, propriedade);
            }
            var tipoEntidade = AjudanteConsultaEntidade.RetornarTipoListaEntidade(propriedade.PropertyType);
            if (!tipoEntidade.IsSubclassOf(typeof(Entidade)))
            {
                throw new ErroNaoSuportado(String.Format("O Tipo não é um entidade {0}", tipoEntidade.Name));
            }
            return AjudanteConsultaEntidade.RetornarTipoListaEntidade(propriedade.PropertyType);
        }


        #endregion

        #region Metodos Privados

        private static Type RetornarTipoListaEntidade(Type tipo)
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
}