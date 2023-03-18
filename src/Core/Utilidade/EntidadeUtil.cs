using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Snebur.Utilidade
{
    public class EntidadeUtil
    {
        private static PropertyInfo _propriedadeNomeTipoEntidade;
        private static List<Type> _tiposInterfacesEntidade;
        public static PropertyInfo PropriedadeNomeTipoEntidade
        {
            get
            {
                if (EntidadeUtil._propriedadeNomeTipoEntidade == null)
                {
                    EntidadeUtil._propriedadeNomeTipoEntidade = ReflexaoUtil.RetornarPropriedade<Entidade>(x => x.__NomeTipoEntidade);
                }
                return _propriedadeNomeTipoEntidade;
            }
        }

        public static List<Type> TiposInterfaceEntidade
        {
            get
            {
                if (EntidadeUtil._tiposInterfacesEntidade == null)
                {
                    _tiposInterfacesEntidade = typeof(EntidadeUtil).Assembly.GetAccessibleTypes().Where(x => x.IsInterface && x.GetInterface(typeof(IEntidade).Name) != null).ToList();
                }
                return EntidadeUtil._tiposInterfacesEntidade;
            }
        }

        public static PropertyInfo RetornarPropriedadeChaveEstrangeira(Type tipoEntidade, PropertyInfo propriedade)
        {
            var atributoChaveEstrangeira = propriedade.RetornarAtributoChaveEstrangeira();
            if (atributoChaveEstrangeira == null)
            {
                throw new ErroNaoDefinido(String.Format("O atributo chave estrangeira não foi definido na propriedade {0}", propriedade.Name));
            }
            var nomePropriedade = atributoChaveEstrangeira.Name;
            var propriedadeChaveEstrangeira = tipoEntidade.GetProperties().Where(x => x.Name == nomePropriedade).SingleOrDefault();
            if (propriedadeChaveEstrangeira == null)
            {
                throw new ErroNaoDefinido(String.Format("Não foi encontrada a propriedade chave estrangeira {0} para a propriedade {1} em {2} ", propriedadeChaveEstrangeira, propriedade.Name, tipoEntidade.Name));
            }
            return propriedadeChaveEstrangeira;
        }

        public static PropertyInfo RetornarPropriedadeChavaPrimaria(Type tipoEntidade)
        {
            var propriedade = tipoEntidade.GetProperties().Where(x => x.GetCustomAttribute<KeyAttribute>() != null).SingleOrDefault();
            if (propriedade == null)
            {
                throw new Erro($"A propriedade chave primaria não foi encontrada para o tipo {tipoEntidade.Name}");
            }
            return propriedade;
        }

        public static long RetornarValorIdChaveEstrangeira(Entidade entidade, PropertyInfo propriedade)
        {
            var valorEntidade = propriedade.GetValue(entidade);
            if (valorEntidade is Entidade)
            {
                return (valorEntidade as Entidade).Id;
            }
            else
            {
                var propriedadeChaveEstrangeira = EntidadeUtil.RetornarPropriedadeChaveEstrangeira(entidade.GetType(), propriedade);
                var valorPropriedade = propriedadeChaveEstrangeira.GetValue(entidade);
                return Convert.ToInt64(valorPropriedade);
            }
        }

        public static List<PropertyInfo> RetornarPropridadesDescricao(Type tipoEntidade)
        {
            var propriedadesDescricao = ReflexaoUtil.RetornarPropriedadePossuiAtributo<PropriedadeDescricaoAttribute>(tipoEntidade);
            if (propriedadesDescricao.Count == 0)
            {
                var propriedade = ReflexaoUtil.RetornarPropriedade(tipoEntidade, "Nome", true);
                if (propriedade != null)
                {
                    propriedadesDescricao.Add(propriedade);
                }
                else
                {
                    propriedade = ReflexaoUtil.RetornarPropriedade(tipoEntidade, "Descricao", true);
                    if (propriedade != null)
                    {
                        propriedadesDescricao.Add(propriedade);
                    }
                }
            }
            return propriedadesDescricao;
        }

        public static List<PropertyInfo> RetornarPropriedadesCampos(Type tipoEntidade)
        {
            return EntidadeUtil.RetornarPropriedadesCampos(tipoEntidade, EnumFiltroPropriedadeCampo.Todas);
        }
        

        public static IValorPadrao RetornarAtributoImplementaIValorPradao(PropertyInfo propriedade)
        {
            var atributos = propriedade.GetCustomAttributes();
            var tipoIValorPadrao = typeof(IValorPadrao);
            var atributosValorPadrao = atributos.Where(x => ReflexaoUtil.TipoImplementaInterface(x.GetType(), tipoIValorPadrao, true)).ToArray();
            if (atributosValorPadrao.Count() > 1)
            {
                throw new Erro($"Existe mais de um atributo que implementa a interface {nameof(IValorPadrao)}, Entidade: '{propriedade.DeclaringType.Name}', Propriedade: '{propriedade.Name}'");
            }
            return (IValorPadrao)atributosValorPadrao.SingleOrDefault();
        }

        public static List<PropertyInfo> RetornarPropriedadesCampos(Type tipoEntidade, EnumFiltroPropriedadeCampo filtro)
        {
            var filtros = EnumUtil.RetornarFlags<EnumFiltroPropriedadeCampo>(filtro).ToHashSet();

            var ignorarTipoBase = filtros.Contains(EnumFiltroPropriedadeCampo.IgnorarTipoBase);
            var ignorarChavePrimaria = filtros.Contains(EnumFiltroPropriedadeCampo.IgnorarChavePrimaria);
            var ignorarPropriedadeProtegida = filtros.Contains(EnumFiltroPropriedadeCampo.IgnorarPropriedadeProtegida);
            var ignorarChaveEstrangeira = filtros.Contains(EnumFiltroPropriedadeCampo.IgnorarChaveEstrangeira);

            return RetornarPropriedadesCampos(tipoEntidade, ignorarTipoBase: ignorarTipoBase,
                                                            ignorarChavePrimaria: ignorarChavePrimaria,
                                                            ignorarPropriedadeProtegida: ignorarPropriedadeProtegida,
                                                            ignorarChaveEstrangeira: ignorarChaveEstrangeira);
        }
        //public static List<PropertyInfo> RetornarPropriedadesCampos(Type tipoEntidade, bool ignorarTipoBase)
        //{
        //    return EntidadeUtil.RetornarPropriedadesCampos(tipoEntidade, ignorarTipoBase, false, false);
        //}

        private static List<PropertyInfo> RetornarPropriedadesCampos(Type tipoEntidade,
                                                                    bool ignorarTipoBase = false,
                                                                    bool ignorarChavePrimaria = false,
                                                                    bool ignorarPropriedadeProtegida = false,
                                                                    bool ignorarChaveEstrangeira = false,
                                                                    bool ignorarSobreescritas = false )
        {
            var propriedades = ReflexaoUtil.RetornarPropriedades(tipoEntidade, ignorarTipoBase);
            if (tipoEntidade.BaseType == typeof(Entidade))
            {
                if (!ignorarPropriedadeProtegida)
                {
                    propriedades.Insert(0, EntidadeUtil.PropriedadeNomeTipoEntidade);
                }
            }
            HashSet<string> nomePrpriedadesChaveEstrangeiras = null;

            if (ignorarChaveEstrangeira)
            {
                nomePrpriedadesChaveEstrangeiras = propriedades.Select(x => x.RetornarAtributoChaveEstrangeira()).
                                                                Where(x => x != null).
                                                                Select(x => x.NomePropriedade).ToHashSet();
            }
            return propriedades.Where(x =>
                                    {
                                        if (ReflexaoUtil.PropriedadeRetornaTipoPrimario(x, true) &&
                                            x.GetGetMethod() != null && x.GetGetMethod().IsPublic &&
                                            x.GetSetMethod() != null && x.GetSetMethod().IsPublic ||
                                            x == EntidadeUtil.PropriedadeNomeTipoEntidade)
                                        {
                                            var atrituboNaoMapear = x.GetCustomAttribute<NaoMapearAttribute>();
                                            var atrituboNaoMapearInterno = x.GetCustomAttribute<NaoMapearInternoAttribute>();
                                            var atributoChavePrimaria = x.GetCustomAttribute<KeyAttribute>();
                                            var atributoPropriedadeProtegida = x.GetCustomAttribute<PropriedadeProtegidaAttribute>();
                                            var getMethod = x.GetGetMethod();
                                            bool resultado = atrituboNaoMapear == null && atrituboNaoMapearInterno == null;
                                            if (resultado)
                                            {
                                                // ignorar propriedades sobreescritas - override
                                                if (atributoChavePrimaria == null)
                                                {
                                                    resultado = resultado && getMethod.GetBaseDefinition().DeclaringType == getMethod.DeclaringType;
                                                }
                                                if (ignorarChavePrimaria)
                                                {
                                                    resultado = resultado && atributoChavePrimaria == null;
                                                }
                                                if (ignorarPropriedadeProtegida)
                                                {
                                                    resultado = resultado && atributoPropriedadeProtegida == null;
                                                }
                                                if (ignorarChaveEstrangeira)
                                                {
                                                    resultado = resultado && !nomePrpriedadesChaveEstrangeiras.Contains(x.Name);
                                                }
                                                if (ignorarSobreescritas)
                                                {
                                                    resultado = resultado && !nomePrpriedadesChaveEstrangeiras.Contains(x.Name);
                                                }
                                            }
                                            return resultado;
                                        }
                                        return false;

                                    }).ToList();
        }

        public static string RetornarNomeCampo(PropertyInfo propriedade)
        {
            var atributoCampo = propriedade.GetCustomAttribute<CampoAttribute>();
            if (atributoCampo != null)
            {
                return atributoCampo.NomeCampo;
            }
            return propriedade.Name;
        }

        public static List<PropertyInfo> RetornarPropriedadesRelacaoPai(Type tipoEntidade, bool ignorarTipoBase = false)
        {
            var propriedades = ReflexaoUtil.RetornarPropriedades(tipoEntidade, ignorarTipoBase);
            propriedades.Where(x => x.RetornarAtributoChaveEstrangeira() != null);
            return propriedades.ToList();
        }

        public static Dictionary<string, PropertyInfo> RetornarDicionarioPropriedades(Type tipoEntidade)
        {
            var dicionario = new Dictionary<string, PropertyInfo>();
            var propriedades = EntidadeUtil.RetornarPropriedadesCampos(tipoEntidade, EnumFiltroPropriedadeCampo.Todas);
            var propriedadesRelacao = EntidadeUtil.RetornarPropriedadesRelacaoPai(tipoEntidade);

            foreach (var propriedade in propriedades)
            {
                dicionario.Add(propriedade.Name, propriedade);
            }
            return dicionario;
        }

        public static Dictionary<string, FieldInfo> RetornarDicionarioCamposPrivados(Type tipoEntidade, List<PropertyInfo> propriedades)
        {
            var dicionario = new Dictionary<string, FieldInfo>();

            foreach (var propriedade in propriedades)
            {
                var nomeCampoPrivado = EntidadeUtil.RetornarNomeCampoPrivado(propriedade);
                var campo = tipoEntidade.GetField(nomeCampoPrivado);
                if (campo == null)
                {
                    throw new Exception($"Não foi encontrado  o campo privado para a propriedade {propriedade.Name}, nome do campo {nomeCampoPrivado}");
                }
                dicionario.Add(propriedade.Name, campo);
            }
            return dicionario;
        }

        private static string RetornarNomeCampoPrivado(PropertyInfo propriedade)
        {
            var nomePropriedade = TextoUtil.RetornarPrimeiraLetraMaiuscula(propriedade.Name);
            if (nomePropriedade.StartsWith("iD"))
            {
                nomePropriedade = "id" + nomePropriedade.Substring(2);
            }
            var nomeCampo = "_" + nomePropriedade;
            return nomeCampo;
        }


    }

    public enum EnumFiltroPropriedadeCampo
    {
        Todas = 1,
        IgnorarTipoBase = 2,
        IgnorarChavePrimaria = 4,
        IgnorarPropriedadeProtegida = 8,
        IgnorarChaveEstrangeira = 16
    }
}