using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Snebur.AcessoDados.Estrutura
{
    internal class AjudanteEstruturaBancoDados
    {
        //public List<PropertyInfo> RetornarPropriedadesCampo(tipo)

        internal static string RetornarNomeTabela(Type tipo)
        {
            var atributoTable = (TableAttribute)tipo.GetCustomAttribute(typeof(TabelaAttribute), false);
            if (atributoTable == null)
            {
                Debug.WriteLine(String.Format("Não foi encontrada a atributo Table não no tipo entidade {0} ", tipo.Name));

                return tipo.Name;
            }
            return atributoTable.Name;
        }

        internal static string RetornarNomeSchemaTabela(Type tipo)
        {
            var atributoTable = (TableAttribute)tipo.GetCustomAttribute(typeof(TabelaAttribute), false);
            if (atributoTable == null)
            {

                throw new Erro($"O atribuo {nameof(TableAttribute)} não foi definido no tipo {tipo.Name}");
            }
            return atributoTable.Schema;
        }

        internal static List<PropertyInfo> RetornarPropriedadesCampos(Type tipoEntidade)
        {
            return EntidadeUtil.RetornarPropriedadesCampos(tipoEntidade, EnumFiltroPropriedadeCampo.IgnorarTipoBase |
                                                                         EnumFiltroPropriedadeCampo.IgnorarChavePrimaria);
        }

        internal static string RetornarSchema(Type tipo)
        {
            var atributoTable = (TabelaAttribute)tipo.GetCustomAttribute(typeof(TabelaAttribute), false);
            if (atributoTable == null)
            {
                throw new Exception($"O atribuo {nameof(TabelaAttribute)} não foi encontrado no tipo {tipo.RetornarCaminhoTipo()}");
            }
            if (String.IsNullOrEmpty(atributoTable.Schema))
            {
                return AjudanteEstruturaBancoDados.RetornarSchemaPadrao();
            }
            return atributoTable.Schema;
        }

        internal static string RetornarGrupoArquivoIndices(Type tipo)
        {
            var atributoTable = (TabelaAttribute)tipo.GetCustomAttribute(typeof(TabelaAttribute), false);
            if (atributoTable == null)
            {
                throw new Exception($"O atributo {nameof(TabelaAttribute)} não foi encontrado no tipo {tipo.RetornarCaminhoTipo()}");
            }
            if (String.IsNullOrEmpty(atributoTable.Schema))
            {
                return "PRIMARY";
            }
            return atributoTable.GrupoArquivoIndices;
        }

        internal static bool PropriedadeRequerida(PropertyInfo propriedade)
        {
            return ReflexaoUtil.IsPropriedadePossuiAtributo(propriedade, typeof(ValidacaoRequeridoAttribute));
        }

        internal static PropertyInfo RetornarPropriedadeChavePrimaria(Type tipoEntidade)
        {
            var proriedades = ReflexaoUtil.RetornarPropriedades(tipoEntidade);
            var propriedadesChavePrimaria = proriedades.Where(x =>
                                                            {
                                                                if (ReflexaoUtil.IsPropriedadeRetornaTipoPrimario(x))
                                                                {
                                                                    var atributoChavePrimaria = x.GetCustomAttribute<KeyAttribute>();
                                                                    return (atributoChavePrimaria != null);
                                                                }
                                                                return false;

                                                            }).ToList();

            if (propriedadesChavePrimaria.Count == 0)
            {
                throw new Erro(String.Format("Não existe propriedade chave primária no tipo {0}", tipoEntidade.Name));
            }
            if (propriedadesChavePrimaria.Count > 1)
            {
                throw new Erro(String.Format("Existe mais de uma propriedade chave primária no tipo {0}", tipoEntidade.Name));
            }
            return propriedadesChavePrimaria.Single();
        }

        internal static List<PropertyInfo> RetornarPropriedadesRelacao(Type tipoEntidade)
        {
            var proriedades = ReflexaoUtil.RetornarPropriedades(tipoEntidade, true);
            proriedades = proriedades.Where(x => x.GetCustomAttribute<BaseRelacaoAttribute>(false) != null).ToList();
            return proriedades;
        }

        internal static bool TipoEntidadeBaseNaoMepeada(Type tipoEntidade)
        {
            var atributoNaoMepear = tipoEntidade.GetCustomAttribute(typeof(NaoCriarTabelaEntidadeAttribute), false);
            var resultado = atributoNaoMepear != null;
            return resultado;
        }

        internal static PropertyInfo RetornarPropriedadeRelacao(Type tipoEntidade,
                                                                Type tipoPropriedadeRelacao,
                                                                EnumTipoRelacao tipoRelacaoEnum,
                                                                bool ignorarPropridadesClasseBase = true)
        {
            var propriedades = ReflexaoUtil.RetornarPropriedades(tipoEntidade, ignorarPropridadesClasseBase);
            switch (tipoRelacaoEnum)
            {
                case (EnumTipoRelacao.RelacaoUmUm):
                case (EnumTipoRelacao.RelacaoPai):

                    propriedades = propriedades.Where(x => x.PropertyType == tipoPropriedadeRelacao).ToList();
                    break;

                case (EnumTipoRelacao.RelacaoFilhos):

                    throw new NotImplementedException();

                case (EnumTipoRelacao.RelacaoNn):

                    throw new NotImplementedException();

                case (EnumTipoRelacao.RelacaoNnEspecializada):

                    throw new NotImplementedException();

                default:

                    throw new Erro(String.Format("O tipo relação enum {0} não é suportado", EnumUtil.RetornarDescricao(tipoRelacaoEnum)));
            }
            if (propriedades.Count == 0)
            {
                if (!ignorarPropridadesClasseBase)
                {
                    throw new Erro($"A propriedade relação não foi encontrada,  Tipo dae entidade : '{tipoEntidade.Name}', " +
                                  $" Tipo da propriedade relação : {tipoPropriedadeRelacao.Name} ," +
                                  $" Tipo da relação : {EnumUtil.RetornarDescricao(tipoRelacaoEnum)} ");
                }
                else
                {
                    return AjudanteEstruturaBancoDados.RetornarPropriedadeRelacao(tipoEntidade, tipoPropriedadeRelacao, tipoRelacaoEnum, false);
                }
            }
            if (propriedades.Count > 1)
            {
                throw new Erro(String.Format("O existe mais de uma propriedade relação "));
            }
            return propriedades.Single();
        }

        internal static string RetornarNomePropriedadeChaveEstrangeira(PropertyInfo propriedade)
        {
            var atributoChaveEstrangeira = propriedade.RetornarAtributoChaveEstrangeira();
            if (atributoChaveEstrangeira == null)
            {
                throw new Erro(String.Format("Não foi encontrado um chave estrangeira para a propriedade {0} em {1} ", propriedade.Name, propriedade.DeclaringType.Name));
            }
            return atributoChaveEstrangeira.Name;
        }
        /// <summary>
        /// Retornar o tipo generico da List, ex List<Pedido>() retorn typeof(Pedido)
        /// </summary>
        /// <param name="propriedade"></param>
        /// <returns></returns>
        internal static Type RetornarTipoEntidadeLista(PropertyInfo propriedade)
        {
            return AjudanteEstruturaBancoDados.RetornarTipoGerencio(propriedade.PropertyType);
        }

        private static Type RetornarTipoGerencio(Type tipoGenerico)
        {
            if (tipoGenerico == null)
            {
                return null;
            }
            if (tipoGenerico.IsGenericType)
            {
                var parametrosGenericos = tipoGenerico.GetGenericArguments();
                if (parametrosGenericos.Count() != 1)
                {
                    throw new Erro(String.Format("O numero de argumentos genérico no tipo é invalido {0} ", parametrosGenericos.Count()));
                }
                return parametrosGenericos.Single();
            }
            else
            {
                return AjudanteEstruturaBancoDados.RetornarTipoGerencio(tipoGenerico.BaseType);
            }
        }

        #region TiposComplexos

        internal static List<PropertyInfo> RetornarPropriedadesTipoComplexo(Type tipoEntidade)
        {
            return ReflexaoUtil.RetornarPropriedades(tipoEntidade, true).Where(x => x.PropertyType.IsSubclassOf(typeof(BaseTipoComplexo)) && IsPropriedadeMapeada(x)).ToList();
        }

        private static bool IsPropriedadeMapeada(PropertyInfo propriedade)
        {
            var atributoNaoMapear = propriedade.GetCustomAttribute<NaoMapearAttribute>();
            var atributoNaoMapearInterno = propriedade.GetCustomAttribute<NaoMapearInternoAttribute>();
            return atributoNaoMapear == null && atributoNaoMapearInterno == null;
        }

        internal static List<PropertyInfo> RetornarPropriedadesCamposTipoComplexo(Type tipoTipoComplexo)
        {
            var propriedadesCampo = AjudanteEstruturaBancoDados.RetornarPropriedadesCampos(tipoTipoComplexo);
            return propriedadesCampo;
        }
        #endregion

        private static string RetornarSchemaPadrao()
        {
            switch (ConfiguracaoAcessoDados.TipoBancoDadosEnum)
            {
                case (EnumTipoBancoDados.SQL_SERVER):

                    return "dbo";

                case (EnumTipoBancoDados.PostgreSQL):

                    return "public";

                case (EnumTipoBancoDados.PostgreSQLImob):

                    return "public";
                default:

                    throw new Exception(String.Format("O tipo do banco de dados não é suportado {0}", EnumUtil.RetornarDescricao(ConfiguracaoAcessoDados.TipoBancoDadosEnum)));
            }
        }
    }
}