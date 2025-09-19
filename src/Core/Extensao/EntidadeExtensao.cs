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
        Debugger.Break();

        if (entidade is null)
        {
            throw new ArgumentNullException(nameof(entidade));
        }

        var relation = relationFactory.Invoke(entidade);
        if (relation != null)
        {
            return relation;
        }

        if (entidade is Entidade entidadeTipada)
        {
            var expression = ToExpression(relationFactory);
            var propertyInfo = ExpressaoUtil.RetornarPropriedade(expression);

            var idChaveEstrangeira = EntidadeUtil.RetornarValorIdChaveEstrangeira(entidadeTipada, propertyInfo);
            if (idChaveEstrangeira > 0)
            {
                throw new ErroOperacaoInvalida(
                    $" A entidade {entidade.GetType().Name}{entidade} não possui a relação aberta {relationFactory}. A chave estrangeira {propertyInfo.Name} possui o valor {idChaveEstrangeira}, mas a relação não foi carregada. Utilize o método AbrirRelacao para carregar a relação.",
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