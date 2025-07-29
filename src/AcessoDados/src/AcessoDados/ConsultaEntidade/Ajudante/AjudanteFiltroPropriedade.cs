using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Reflexao;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Snebur.AcessoDados.Ajudantes;

internal partial class AjudanteFiltroPropriedade
{
    internal static FiltroPropriedade RetornarFiltroPropriedade(
        EstruturaConsulta estruturaConsulta,
        MemberExpression? expressao,
        object? valorPropriedade,
        EnumOperadorFiltro operadorFiltro)
    {
        var propriedades = ExpressaoUtil.RetornarPropriedades(expressao, false);
        var partes = new List<string>();
        var tipoEntidadeAtual = estruturaConsulta.TipoEntidadeConsulta;
        Guard.NotNull(tipoEntidadeAtual);

        PropertyInfo? propriedade = null;
        foreach (var p in propriedades)
        {
            propriedade = p;
            if (propriedade.DeclaringType?.IsInterface == true)
            {
                propriedade = AjudanteConsultaEntidade.RetornarPropriedadeInterface(tipoEntidadeAtual, propriedade);
                if (propriedade.PropertyType.IsSubclassOf(typeof(Entidade)))
                {
                    tipoEntidadeAtual = propriedade.PropertyType;
                }
            }
            var atributoProprieadeInterface = propriedade.GetCustomAttribute<PropriedadeInterfaceAttribute>();
            if (atributoProprieadeInterface != null)
            {
                Guard.NotNull(propriedade.DeclaringType);
                propriedade = ReflexaoUtil.RetornarPropriedade(propriedade.DeclaringType,
                                                               atributoProprieadeInterface.NomePropriedade);
            }
            partes.Add(propriedade.Name);
        }
        var caminhoPropriedade = String.Join(".", partes);

        return RetornarFiltroCaminhoPropriedadePropriedade(estruturaConsulta, propriedade, caminhoPropriedade, valorPropriedade, operadorFiltro);
    }

    internal static FiltroPropriedade RetornarFiltroCaminhoPropriedadePropriedade(
        EstruturaConsulta estruturaConsulta,
        PropertyInfo? propriedade,
        string? caminhoPropriedade,
        object? valorPropriedade,
        EnumOperadorFiltro operadorFiltro)
    {
        if(propriedade is not null)
        {
            Guard.NotNull(propriedade.DeclaringType);
            Guard.NotEmpty(estruturaConsulta);
            Guard.NotEmpty(estruturaConsulta.TipoEntidadeConsulta);

            if (propriedade.DeclaringType.IsInterface == true)
            {
                if (caminhoPropriedade?.Contains(".") == true)
                {
                    throw new InvalidOperationException("não é possível filtrar um propriedade interface com caminho");
                }
                propriedade = AjudanteConsultaEntidade.RetornarPropriedadeInterface(estruturaConsulta.TipoEntidadeConsulta, propriedade);
            }
            //propriedade = AjudanteFiltroPropriedade.NormalizarPropriedadeEspecializada(propriedade);

            if (propriedade.PropertyType.IsSubclassOf(typeof(Entidade)) &&
                valorPropriedade is Entidade entidade)
            {
                if (!entidade.GetType().IsTipoIguaOuHerda(propriedade.PropertyType))
                {
                    throw new Erro($"A entidade '{valorPropriedade.GetType().Name}' não é compatível com tipo propriedade '{propriedade.Name}' do tipo '{propriedade.PropertyType.Name}' ");
                }

                var novoCaminhoPropriedade = caminhoPropriedade?.Contains(".") == true
                    ? caminhoPropriedade?.Substring(0, caminhoPropriedade.LastIndexOf(".") + 1) 
                    : String.Empty;

                Guard.NotNull(propriedade.DeclaringType);

                var propriedadeChavaEstrangeira = EntidadeUtil.RetornarPropriedadeChaveEstrangeira(propriedade.DeclaringType, propriedade);

                Guard.NotNull(propriedadeChavaEstrangeira);

                novoCaminhoPropriedade = novoCaminhoPropriedade + propriedadeChavaEstrangeira.Name;

                //Guard.NotNull(entidade);
                var novoValorPropriedade = entidade.Id;

                valorPropriedade = novoValorPropriedade;
                propriedade = propriedadeChavaEstrangeira;
                caminhoPropriedade = novoCaminhoPropriedade;
            }
        }
         
        var filtroPropriedade = new FiltroPropriedade()
        {
            Operador = operadorFiltro,
            CaminhoPropriedade = caminhoPropriedade,
            TipoPrimarioEnum = AjudanteFiltroPropriedade.RetornarTipoPropriedade(propriedade?.PropertyType),
            Valor = valorPropriedade
        };
        //filtroPropriedade.Valor = AjudanteFiltroPropriedade.RetornarValorPropriedadeFormatado(valorPropriedade, filtroPropriedade.TipoPropriedadeEnum);

        return filtroPropriedade;
    }

    //internal static PropertyInfo NormalizarPropriedadeEspecializada(PropertyInfo propriedade)
    //{
    //    var atributoPropriedadeEspecializada = propriedade.GetCustomAttribute<PropriedadeTSEspecializadaAttribute>();
    //    if (atributoPropriedadeEspecializada != null)
    //    {
    //        var nomePropriedadeEspecializada = atributoPropriedadeEspecializada.NomePropriedade;
    //        var propriedadeEspecialidada = propriedade.DeclaringType.GetProperty(nomePropriedadeEspecializada);
    //        return NormalizarPropriedadeEspecializada(propriedadeEspecialidada);
    //    }
    //    return propriedade;
    //}

    private static EnumTipoPrimario RetornarTipoPropriedade(
        [NotNull] Type? tipo)
    {
        Guard.NotNull(tipo);
        tipo = ReflexaoUtil.RetornarTipoSemNullable(tipo);

        if (tipo.IsEnum)
        {
            return EnumTipoPrimario.EnumValor;
        }
        else
        {
            switch (tipo.Name)
            {
                case nameof(String):

                    return EnumTipoPrimario.String;

                case nameof(Byte):
                case nameof(Int16):
                case nameof(Int32):

                    return EnumTipoPrimario.Integer;

                case nameof(Int64):

                    return EnumTipoPrimario.Long;

                case nameof(Double):
                case nameof(Decimal):

                    return EnumTipoPrimario.Decimal;

                case nameof(Boolean):

                    return EnumTipoPrimario.Boolean;

                case nameof(DateTime):

                    return EnumTipoPrimario.DateTime;

                case nameof(Guid):

                    return EnumTipoPrimario.Guid;

                case nameof(System.Enum):

                    return EnumTipoPrimario.EnumValor;

                case nameof(TimeSpan):

                    return EnumTipoPrimario.TimeSpan;

                default:
                    throw new ErroNaoSuportado(String.Format("Tipo não suportado {0} ", tipo.Name));
            }
        }
    }

    //private static object RetornarValorPropriedadeFormatado(object valorPropriedade, EnumTipoPrimario tipoPrimario)
    //{
    //    switch (tipoPrimario)
    //    {
    //        case EnumTipoPrimario.String:

    //            return valorPropriedade.ToString();

    //        case EnumTipoPrimario.Integer:

    //            return Convert.ToInt32(valorPropriedade).ToString();

    //        case EnumTipoPrimario.Long:

    //            return Convert.ToInt64(valorPropriedade).ToString();

    //        case EnumTipoPrimario.Decimal:

    //            return Convert.ToDecimal(valorPropriedade).ToString().Replace(",", ".");

    //        case EnumTipoPrimario.Boolean:

    //            return Convert.ToBoolean(valorPropriedade).ToString();

    //        case EnumTipoPrimario.DateTime:

    //            return Convert.ToDateTime(valorPropriedade).ToString();

    //        case EnumTipoPrimario.Guid:
    //            {
    //                if (valorPropriedade == null)
    //                {
    //                    return Guid.Empty.ToString();
    //                }
    //                else
    //                {
    //                    Guid guidValor;
    //                    if (!Guid.TryParse(valorPropriedade.ToString(), out guidValor))
    //                    {
    //                        throw new Erro(String.Format("Não foi possível converter o valor {0} para guid", valorPropriedade.ToString()));
    //                    }
    //                }
    //                break;
    //            }
    //        default:

    //            throw new ErroNaoSuportado(String.Format("Tipo não suportado {0} ", EnumUtil.RetornarDescricao(tipoPrimario)));
    //    }
    //    throw new ErroNaoSuportado(String.Format("Tipo não suportado {0} ", EnumUtil.RetornarDescricao(tipoPrimario)));
    //}

    private static PropertyInfo RetornarPropriedade(MemberExpression expressao)
    {
        return (PropertyInfo)expressao.Member;
    }

    private static string RetornarCaminhoPropriedade(Expression expressao)
    {
        var propriedades = ExpressaoUtil.RetornarPropriedades(expressao, false);
        var caminhoPropriedade = AjudanteConsultaEntidade.RetornarCaminhoPropriedade(propriedades);
        return caminhoPropriedade;
    }

    private static object? RetornarValorPropriedade(Expression expressao)
    {
        if (expressao.NodeType == ExpressionType.Constant)
        {
            var constantExpression = (ConstantExpression)expressao;
            return constantExpression.Value;
        }
        if (expressao.NodeType == ExpressionType.Convert)
        {
            var unaryExpression = (UnaryExpression)expressao;
            return RetornarValorVariavelLocal(unaryExpression);
        }
        if (expressao.NodeType == ExpressionType.MemberAccess)
        {
            var memberExpression = (MemberExpression)expressao;
            return RetornarValorVariavelLocal(memberExpression);
        }
        if (expressao.NodeType == ExpressionType.Call)
        {
            var methodCallExpression = (MethodCallExpression)expressao;
            var valor = RetornarValorVariavelLocal(methodCallExpression);
            return valor;
        }
        throw new NotSupportedException();
    }

    private static object RetornarValorVariavelLocal(Expression expressao)
    {
        var objeto = Expression.Convert(expressao, typeof(object));
        var lambda = Expression.Lambda<Func<object>>(objeto);
        var compile = lambda.Compile();
        var valor = compile();
        return valor;
    }

    private static EnumOperadorFiltro RetornarOperadorDoFiltroPropriedade(ExpressionType tipoExpressao)
    {
        switch (tipoExpressao)
        {
            case ExpressionType.Equal:

                return EnumOperadorFiltro.Igual;

            case ExpressionType.NotEqual:

                return EnumOperadorFiltro.Diferente;

            case ExpressionType.GreaterThan:

                return EnumOperadorFiltro.Maior;

            case ExpressionType.GreaterThanOrEqual:

                return EnumOperadorFiltro.MaiorIgual;

            case ExpressionType.LessThan:

                return EnumOperadorFiltro.Menor;

            case ExpressionType.LessThanOrEqual:

                return EnumOperadorFiltro.MenorIgual;

            default:

                throw new NotFiniteNumberException();
        }
    }
}