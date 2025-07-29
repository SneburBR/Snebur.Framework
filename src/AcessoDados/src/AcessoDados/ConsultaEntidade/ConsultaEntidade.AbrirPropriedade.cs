using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Snebur.AcessoDados;

public partial class ConsultaEntidade<TEntidade> where TEntidade : IEntidade
{
    public ConsultaEntidade<TEntidade> AbrirPropriedade<TPropriedade>(Expression<Func<TEntidade, TPropriedade>> expresssao)
    {
        return this.AbrirPropriedade((Expression)expresssao);
    }

    public ConsultaEntidade<TEntidade> AbrirPropriedade(Expression<Func<TEntidade, string>> expresssao)
    {
        return this.AbrirPropriedade((Expression)expresssao);
    }

    public ConsultaEntidade<TEntidade> AbrirPropriedades<TPropriedade>(params Expression<Func<TEntidade, TPropriedade>>[] expressoes)
    {
        return this.AbrirPropriedades(expressoes.Cast<Expression>());
    }

    public ConsultaEntidade<TEntidade> AbrirPropriedades(params Expression<Func<TEntidade, string>>[] expressoes)
    {
        return this.AbrirPropriedades(expressoes.Cast<Expression>());
    }

    public ConsultaEntidade<TEntidade> AbrirPropriedadeTipoComplexo<TTipoComplexo>(Expression<Func<TEntidade, TTipoComplexo>> expresssao) where TTipoComplexo : BaseTipoComplexo
    {
        throw new NotImplementedException();
        //return this.AbrirPropriedade((Expression)expresssao);
    }

    private ConsultaEntidade<TEntidade> AbrirPropriedades(IEnumerable<Expression> expressoesPropriedade)
    {
        foreach (var expressaoPropriedade in expressoesPropriedade)
        {
            this.AbrirPropriedade(expressaoPropriedade);
        }
        return this;
    }

    public ConsultaEntidade<TEntidade> AbrirPropriedade(Expression expressaoPropriedade)
    {
        var propriedades = ExpressaoUtil.RetornarPropriedades(expressaoPropriedade);

        if (propriedades.Count != 1)
        {
            throw new Erro(String.Format("A expressao do campo não é valida {0}", expressaoPropriedade.ToString()));
        }
        var propriedade = propriedades.Single();
        if (!ReflexaoUtil.IsPropriedadeRetornaTipoPrimario(propriedade))
        {
            throw new Erro(String.Format("O tipo da propriedade não é um tipo campo valido {0}", propriedade.PropertyType.Name.ToString()));
        }
        this.EstruturaConsulta.PropriedadesAbertas.Add(propriedade.Name);
        return this;
    }
}