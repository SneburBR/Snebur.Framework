using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Snebur;

public static class EntidadeExtensao
{
    [Obsolete("Use GetRequired, it avoids optional args in expression trees.")]
    public static TResut GetRequired2<TEntidade, TResut>(
        this TEntidade? entidade, Func<TEntidade, TResut?> relationFactory)
        where TEntidade : IEntidade
        where TResut : IEntidade
    {
        Debugger.Break();
        return entidade.GetRequired(relationFactory);
    }

    public static TResut GetRequired<TEntidade, TResut>(
        this TEntidade? entidade,
        Func<TEntidade, TResut?> relationFactory,
        [CallerMemberName] string nomeMetodo = "",
        [CallerFilePath] string caminhoArquivo = "",
        [CallerLineNumber] int linhaDoErro = 0)
        where TEntidade : IEntidade
        where TResut : IEntidade
    {
        if (entidade is null)
        {
            throw new ArgumentNullException(nameof(entidade));
        }

        var relation = relationFactory.Invoke(entidade);
        if (relation is not null)
        {
            return relation;
        }

        if (entidade is Entidade entidadeTipada)
        {
            var expression = ToExpression(relationFactory);
            var propertyInfo = ExpressaoUtil.RetornarPropriedade(expression);
            var idChaveEstrangeira = EntidadeUtil.RetornarValorIdChaveEstrangeira(
                entidadeTipada,
                propertyInfo);

            var mensagemErro = idChaveEstrangeira > 0
                ? $" A entidade {entidade.GetType().Name}{entidade} possui a relação aberta {relationFactory}, " +
                  $"porém a chave estrangeira {propertyInfo.Name} está com valor {idChaveEstrangeira}."
                : $" A entidade {entidade.GetType().Name}{entidade} não possui a relação aberta {relationFactory}, " +
                  $"e a chave estrangeira {propertyInfo.Name} = '{idChaveEstrangeira}' não está definida.";

            if (idChaveEstrangeira > 0)
            {
                throw new ErroOperacaoInvalida(
                    mensagemErro,
                    null,
                    nomeMetodo,
                    caminhoArquivo,
                    linhaDoErro);
            }
        }

        throw new ErroOperacaoInvalida(
              $" A entidade {entidade.GetType().Name}{entidade} não possui a relação aberta {relationFactory}.",
              null,
              nomeMetodo,
              caminhoArquivo,
              linhaDoErro); ;
    }

    private static Expression<Func<TEntidade, object?>> ToExpression<TEntidade, TResut>(
        Func<TEntidade, TResut?> relationFactory)
    {
        var param = Expression.Parameter(typeof(TEntidade), "x");
        var body = Expression.Invoke(Expression.Constant(relationFactory), param);
        var converted = Expression.Convert(body, typeof(object));
        return Expression.Lambda<Func<TEntidade, object?>>(converted, param);
    }
}