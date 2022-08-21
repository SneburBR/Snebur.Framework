using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using System.Reflection;
using Snebur.Utilidade;
using Snebur.Reflexao;

namespace Snebur.Schema
{

    public class ReflexaoSchemaUtil
    {
        
        public static string RetornarNomeElementoLista(Type tipo)
        {
            var atributoPlural = tipo.GetCustomAttribute<PluralAttribute>(false);
            if (atributoPlural != null)
            {
                return atributoPlural.Nome;
            }
            else
            {
                return String.Format("{0}s", tipo.Name);
            }
        }

        public static Type RetornarTipoNormalizado(Type tipo)
        {
            if (tipo.IsGenericType)
            {
                var tipoG = tipo.GetGenericTypeDefinition();
                if ((Object.ReferenceEquals(tipoG, typeof(Nullable<>))))
                {
                    return tipo.GetGenericArguments().Single();
                }
            }
            return tipo;
        }

        public static EnumTipoReflexao RetornarTipoReflexaoEnum(Type tipo)
        {
            //ErroUtils.ValidarReferenciaNula(tipo, nameof(tipo));

            //tipo = ReflexaoSchemaUtils.RetornarTipoNormalizado(tipo);

            //if (ReflectionUtils.TipoIgualOuHerda(tipo, typeof(Schema)))
            //{
            //    return Dominio.Reflexao.EnumTipoReflexao.SchemaElementos;
            //}

            //if (ReflectionUtils.TipoIgualOuHerda(tipo, typeof(SchemaAtributos)))
            //{
            //    return Dominio.Reflexao.EnumTipoReflexao.SchemaAtributos;
            //}

            //if (ReflectionUtils.TipoIgualOuHerda(tipo, typeof(BaseEntidade)))
            //{
            //    return Dominio.Reflexao.EnumTipoReflexao.Entidade;
            //}

            //if (ReflectionUtils.TipoRetornaColecao(tipo))
            //{

            //    if (!tipo.IsGenericType || (!object.ReferenceEquals(tipo.GetGenericTypeDefinition(), (typeof(List<>)))))
            //    {
            //        throw new InvalidOperationException("Tipo não suportado  para a coleção " + tipo.Name);
            //    }

            //    var tipoLista = tipo.GetGenericArguments().Single();
            //    if (ReflectionUtils.TipoIgualOuHerda(tipoLista, typeof(BaseEntidade)))
            //    {
            //        return Dominio.Reflexao.EnumTipoReflexao.ListaEntidades;
            //    }

            //    if (ReflectionUtils.TipoIgualOuHerda(tipoLista, typeof(BaseDominio)))
            //    {
            //        return Dominio.Reflexao.EnumTipoReflexao.Lista;
            //    }
            //    throw new NotSupportedException("Tipolista não suportado " + tipoLista.Name);

            //}

            //if (ReflectionUtils.TipoIgualOuHerda(tipo, typeof(BaseTipoComplexo)))
            //{
            //    return Dominio.Reflexao.EnumTipoReflexao.TipoComplexo;
            //}

            //if (ReflectionUtils.TipoIgualOuHerda(tipo, typeof(ValueCampo)))
            //{
            //    return Dominio.Reflexao.EnumTipoReflexao.ValueCampo;
            //}

            //if (ReflectionUtils.TipoIgualOuHerda(tipo, typeof(BaseViewModel)))
            //{
            //    return Dominio.Reflexao.EnumTipoReflexao.ViewModel;
            //}

            //if (ReflectionUtils.TipoIgualOuHerda(tipo, typeof(Snebur.Dominio.Enum)))
            //{
            //    return Dominio.Reflexao.EnumTipoReflexao.EnumValores;
            //}

            //if (tipo.IsEnum)
            //{
            //    return Dominio.Reflexao.EnumTipoReflexao.TipoPrimario;
            //}

            //if (ReflectionUtils.TipoIgualOuHerda(tipo, typeof(BaseSnebur)))
            //{
            //    return Dominio.Reflexao.EnumTipoReflexao.BaseSnebur;
            //}

            //if (ReflectionUtils.TipoIgualOuHerda(tipo, typeof(BaseDominio)))
            //{
            //    return Dominio.Reflexao.EnumTipoReflexao.BaseDominio;
            //}

            ////if (ReflectionUtils.TipoIgualOuHerda(tipo, typeof(BaseTipoComunicacao)))
            ////{
            ////    return EnumTipoReflexao.TipoComunicacao;
            ////}
 
            //if (ReflectionUtils.RetornarTipoPrimarioEnum(tipo) != EnumTipoReflexao.Desconhecido)
            //{
            //    return Dominio.Reflexao.EnumTipoReflexao.TipoPrimario;
            //}
 
            throw new NotSupportedException(String.Format("O tipo {0} não é suportado ", tipo.Name));
        }
        
        public static Type RetornarTipoSchameTipoPrimario(Type tipo)
        {
            var tipoEnum = ReflexaoUtil.RetornarTipoPrimarioEnum(tipo);
            return ReflexaoSchemaUtil.RetornarTipoSchameTipoPrimario(tipoEnum);
        }

        public static Type RetornarTipoSchameTipoPrimario(EnumTipoPrimario tipoPrimario)
        {
            switch (tipoPrimario)
            {
                case EnumTipoPrimario.String:

                    return typeof(SchemaCampoString);

                case EnumTipoPrimario.Boolean:

                    return typeof(SchemaCampoBoolean);

                case EnumTipoPrimario.Integer:
                case EnumTipoPrimario.Long:

                    return typeof(SchemaCampoInteiro);

                case EnumTipoPrimario.Decimal:

                    return typeof(SchemaCampoDecimal);

                case EnumTipoPrimario.DateTime:

                    return typeof(SchemaCampoData);

                case EnumTipoPrimario.TimeSpan:

                    return typeof(SchemaCampoTempo);

                case EnumTipoPrimario.EnumValor:

                    return typeof(SchemaCampoNumero);

                default:

                    return null;
            }
        }
    }
}