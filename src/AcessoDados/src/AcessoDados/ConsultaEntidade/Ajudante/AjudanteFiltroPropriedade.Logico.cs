using System;
using System.Linq.Expressions;

namespace Snebur.AcessoDados.Ajudantes;

internal partial class AjudanteFiltroPropriedade
{

    internal static FiltroPropriedade RetornarFiltroPropriedadeLogico(EstruturaConsulta estruturaConsulta, BinaryExpression expressao)
    {
        var esquerda = expressao.Left;
        var direita = expressao.Right;

        MemberExpression expressaoPropriedade;
        Expression expressaoValorPropreidade;

        var isEsquerdaExpressaoPropriedade = AjudanteFiltroPropriedade.ExpressaoPropriedade(esquerda);
        var isDireitaExpressaoPropriedade = AjudanteFiltroPropriedade.ExpressaoPropriedade(direita);

        if (isEsquerdaExpressaoPropriedade && isDireitaExpressaoPropriedade)
        {
            var esquerdaString = esquerda.ToString();
            var direitoString = direita.ToString();

            if (IsExpressaoLabda(esquerdaString) && IsExpressaoLabda(direitoString))
            {
                throw new Erro($"A expressão lambda não é suportada em ambos lados '{esquerdaString}' '{direitoString}'");
            }

            if (IsExpressaoLabda(esquerdaString))
            {
                expressaoPropriedade = AjudanteFiltroPropriedade.RetornarExpresaoPropriedade(esquerda);
                expressaoValorPropreidade = direita;
            }
            else if (IsExpressaoLabda(direitoString))
            {
                expressaoPropriedade = AjudanteFiltroPropriedade.RetornarExpresaoPropriedade(direita);
                expressaoValorPropreidade = esquerda;
            }
            else
            {
                throw new Erro($"A expressão lambda não foi encontrada '{esquerdaString}' '{direitoString}'");
            }
        }
        else if (isEsquerdaExpressaoPropriedade)
        {
            expressaoPropriedade = RetornarExpresaoPropriedade(esquerda);
            expressaoValorPropreidade = direita;
        }
        else if (isDireitaExpressaoPropriedade)
        {
            expressaoPropriedade = RetornarExpresaoPropriedade(direita);
            expressaoValorPropreidade = esquerda;
        }
        else
        {
            throw new ErroNaoSuportado($"Expressão {expressao.ToString()} não é suportada, somente comparações entre propriedade e valor.");
        }

        var valorPropriedade = AjudanteFiltroPropriedade.RetornarValorPropriedade(expressaoValorPropreidade);
        var operadorFiltro = AjudanteFiltroPropriedade.RetornarOperadorDoFiltroPropriedade(expressao.NodeType);

        return AjudanteFiltroPropriedade.RetornarFiltroPropriedade(estruturaConsulta,
                                                                   expressaoPropriedade,
                                                                   valorPropriedade,
                                                                   operadorFiltro);
    }

    private static bool IsExpressaoLabda(string expressao)
    {
        return expressao.StartsWith("x.") || expressao.StartsWith("Convert(x");
    }

    private static bool ExpressaoPropriedade(Expression expressao)
    {
        if (expressao.NodeType == ExpressionType.MemberAccess)
        {
            var expressaoInterna = ((MemberExpression)expressao).Expression;
            if (expressaoInterna == null)
            {
                return false;
            }

            if (expressaoInterna.NodeType == ExpressionType.MemberAccess ||
                expressaoInterna.NodeType == ExpressionType.Parameter)
            {
                return expressaoInterna.GetType().Name != "FieldExpression";
            }

            if (expressaoInterna.NodeType == ExpressionType.Convert )
            {
                var expressaoString = expressao.ToString();
                return expressaoString == "Convert(x).Id" ||
                       expressaoString == "Convert(x, IEntidade).Id" ||
                       expressaoString == "Convert(x, System.Object).Id";
            }
        }

        if (expressao.NodeType == ExpressionType.Convert)
        {
            var expressaoUnary = (UnaryExpression)expressao;
            return ExpressaoPropriedade(expressaoUnary.Operand);
        }
        return false;
    }

    private static MemberExpression RetornarExpresaoPropriedade(Expression expressao)
    {
        if (expressao is MemberExpression)
        {
            return (MemberExpression)expressao;
        }
        if (expressao is UnaryExpression expressaoUnary)
        {
            if (expressaoUnary.Operand is MemberExpression memberExpression)
            {
                return memberExpression;
            }
        }
        throw new Erro(String.Format("Não foi encontrado as expressão da propriedade {0}", expressao.ToString()));
    }
}