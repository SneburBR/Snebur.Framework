using Snebur.Dominio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Snebur.AcessoDados
{
    internal class ConstrutorConsultaEntidade<TEntidade> : ConstrutorConsultaEntidade, IConsultaEntidade<TEntidade> where TEntidade : IEntidade
    {
        public ConstrutorConsultaEntidade(__BaseContextoDados contexto) : base(contexto, typeof(TEntidade))
        {
        }

        public ConstrutorConsultaEntidade(__BaseContextoDados contexto, Type tipoEntidade) : base(contexto, tipoEntidade)
        {
        }

        public TEntidade Find(long id)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Find(id);
        }

        public int Count()
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Count();
        }

        public ConsultaEntidade<TEspecializacao> OfType<TEspecializacao>() where TEspecializacao : TEntidade
        {
            return new ConsultaEntidade<TEspecializacao>(this.ContextoDados, this.TipoEntidade).OfType<TEspecializacao>();
        }

        public ConsultaEntidade<TEntidade> WhereIds(List<long> ids)
        {
            var consulta = new ConsultaEntidade<TEntidade>(this.ContextoDados);
            return consulta.WhereIds(ids);
        }

        public ConsultaEntidade<TEntidade> WhereIn(Expression<Func<TEntidade, string>> expressaoPropriedade, IEnumerable<string> lista)
        {
            var consulta = new ConsultaEntidade<TEntidade>(this.ContextoDados);
            return consulta.WhereIn(expressaoPropriedade, lista);
        }

        public ConsultaEntidade<TEntidade> WhereIn(Expression<Func<TEntidade, long>> expressaoPropriedade, IEnumerable<long> lista)
        {
            var consulta = new ConsultaEntidade<TEntidade>(this.ContextoDados);
            return consulta.WhereIn(expressaoPropriedade, lista);
        }

        public ConsultaEntidade<TEntidade> WhereIn(Expression<Func<TEntidade, Enum>> expressaoPropriedade, IEnumerable<int> lista)
        {
            var consulta = new ConsultaEntidade<TEntidade>(this.ContextoDados);
            return consulta.WhereIn(expressaoPropriedade, lista);
        }

        public ConsultaEntidade<TEntidade> Take(int take)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Take(take);
        }

        public ConsultaEntidade<TEntidade> Skip(int skip)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Skip(skip);
        }

        public ListaEntidades<TEntidade> ToList()
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).ToList();
        }

        public TEntidade Single()
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Single();
        }

        public TEntidade SingleOrDefault()
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).SingleOrDefault();
        }

        public TEntidade First()
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).First();
        }

        public TEntidade FirstOrDefault()
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).FirstOrDefault();
        }

        public ConsultaEntidade<TEntidade> Where(Expression<Func<TEntidade, bool>> filtro)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Where(filtro);
        }

        public ConsultaEntidade<TEntidade> WhereOr(Expression<Func<TEntidade, bool>> filtro)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).WhereOr(filtro);
        }

        public ConsultaEntidade<TEntidade> OrderBy(Expression<Func<TEntidade, object>> expressaCaminhoPropriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).OrderBy(expressaCaminhoPropriedade);
        }

        public ConsultaEntidade<TEntidade> OrderByDescending(Expression<Func<TEntidade, object>> expressaCaminhoPropriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).OrderByDescending(expressaCaminhoPropriedade);
        }

        public ConsultaEntidade<TEntidade> AdicionarFiltroPropriedade(PropertyInfo propriedade, EnumOperadorFiltro operador, object valor)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).AdicionarFiltroPropriedade(propriedade, operador, valor);
        }
        #region Abrir  Relação

        public ConsultaEntidade<TEntidade> AbrirRelacao(List<PropertyInfo> propriedades)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).AbrirRelacao(propriedades);
        }

        public ConsultaEntidade<TEntidade> AbrirRelacao(string caminhoPropriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).AbrirRelacao(caminhoPropriedade);
        }

        public ConsultaEntidade<TEntidade> AbrirRelacao<TRelacao>(Expression<Func<TEntidade, TRelacao>> expressao) where TRelacao : IEntidade
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).AbrirRelacao(expressao);
        }

        public ConsultaEntidade<TEntidade> AbrirRelacoes<TRelacao>(Expression<Func<TEntidade, TRelacao>>[] expressoes) where TRelacao : IEntidade
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).AbrirRelacoes(expressoes);
        }

        public ConsultaEntidade<TEntidade> AbrirRelacoes(params string[] caminhosPropriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).AbrirRelacoes(caminhosPropriedade);
        }

        public ConsultaEntidade<TEntidade> AbrirRelacoes(params Expression[] expressoes)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).AbrirRelacoes(expressoes);
        }
        #endregion

        #region Abrir Colecao

        //public ConsultaEntidade<TEntidade> AbrirRelacao<TRelacao>(Expression<Func<TEntidade, IEnumerable<TRelacao>>> expressao) where TRelacao : IEntidade
        //{
        //    return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).AbrirColecao(expressao);

        //}

        public ConsultaEntidade<TEntidade> AbrirColecao(string caminhoColecao)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).AbrirColecoes(caminhoColecao);
        }

        public ConsultaEntidade<TEntidade> AbrirColecao<TRelacao>(Expression<Func<TEntidade, IEnumerable<TRelacao>>> expressao) where TRelacao : IEntidade
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).AbrirColecao(expressao);
        }

        public ConsultaEntidade<TEntidade> AbrirColecao(Expression<Func<TEntidade, IEnumerable>> expressao)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).AbrirColecao(expressao);
        }

        public ConsultaEntidade<TEntidade> AbrirColecoes(params string[] caminhosColecao)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).AbrirColecoes(caminhosColecao);
        }

        public ConsultaEntidade<TEntidade> AbrirColecoes(params Expression<Func<TEntidade, IEnumerable>>[] expressoes)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).AbrirColecoes(expressoes);
        }

        public ConsultaEntidade<TEntidade> WhereColecao<TRelacao>(Expression<Func<TEntidade, ListaEntidades<TRelacao>>> caminhoColecao, Expression<Func<TRelacao, bool>> filtro) where TRelacao : Entidade
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).WhereColecao(caminhoColecao, filtro);
        }

        public ConsultaEntidade<TEntidade> OrderByColecao<TRelacao>(Expression<Func<TEntidade, ListaEntidades<TRelacao>>> caminhoColecao, Expression<Func<TRelacao, object>> expressaoCaminhoPropriedade) where TRelacao : Entidade
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).OrderByColecao(caminhoColecao, expressaoCaminhoPropriedade);
        }

        public ConsultaEntidade<TEntidade> OrderByDescendingColecao<TRelacao>(Expression<Func<TEntidade, ListaEntidades<TRelacao>>> caminhoColecao, Expression<Func<TRelacao, object>> expressaoCaminhoPropriedade) where TRelacao : Entidade
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).OrderByDescendingColecao(caminhoColecao, expressaoCaminhoPropriedade);
        }

        public ConsultaEntidade<TEntidade> TakeColecao<TRelacao>(Expression<Func<TEntidade, ListaEntidades<TRelacao>>> caminhoColecao, int take) where TRelacao : Entidade
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).TakeColecao(caminhoColecao, take);
        }

        public ConsultaEntidade<TEntidade> SkipColecao<TRelacao>(Expression<Func<TEntidade, ListaEntidades<TRelacao>>> caminhoColecao, int skip) where TRelacao : Entidade
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).SkipColecao(caminhoColecao, skip);
        }
        #endregion

        #region Abrir Propriedade

        public ConsultaEntidade<TEntidade> AbrirPropriedade<TPropriedade>(Expression<Func<TEntidade, TPropriedade>> expresssao)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).AbrirPropriedade(expresssao);
        }

        public ConsultaEntidade<TEntidade> AbrirPropriedade(Expression<Func<TEntidade, string>> expresssao)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).AbrirPropriedade(expresssao);
        }

        public ConsultaEntidade<TEntidade> AbrirPropriedades<TPropriedade>(params Expression<Func<TEntidade, TPropriedade>>[] expressoes)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).AbrirPropriedades(expressoes);
        }

        public ConsultaEntidade<TEntidade> AbrirPropriedades(params Expression<Func<TEntidade, string>>[] expressoes)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).AbrirPropriedades(expressoes);
        }

        public ConsultaEntidade<TEntidade> AbrirPropriedadeTipoComplexo<TTipoComplexo>(Expression<Func<TEntidade, TTipoComplexo>> expresssao) where TTipoComplexo : BaseTipoComplexo
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).AbrirPropriedadeTipoComplexo(expresssao);
        }
        #endregion

        #region Funcoes

        #region Sum

        public int Sum(Expression<Func<TEntidade, int>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Sum(propriedade);
        }

        public int? Sum(Expression<Func<TEntidade, int?>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Sum(propriedade);
        }

        public long Sum(Expression<Func<TEntidade, long>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Sum(propriedade);
        }

        public decimal Sum(Expression<Func<TEntidade, decimal>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Sum(propriedade);
        }

        public decimal? Sum(Expression<Func<TEntidade, decimal?>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Sum(propriedade);
        }

        public double Sum(Expression<Func<TEntidade, double>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados).Sum(propriedade);
        }

        public double? Sum(Expression<Func<TEntidade, double?>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Sum(propriedade);
        }
        #endregion

        #region Max

        public int Max(Expression<Func<TEntidade, int>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Max(propriedade);
        }

        public int? Max(Expression<Func<TEntidade, int?>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Max(propriedade);
        }

        public long Max(Expression<Func<TEntidade, long>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Max(propriedade);
        }

        public decimal Max(Expression<Func<TEntidade, decimal>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Max(propriedade);
        }

        public decimal? Max(Expression<Func<TEntidade, decimal?>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Max(propriedade);
        }

        public double Max(Expression<Func<TEntidade, double>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Max(propriedade);
        }

        public double? Max(Expression<Func<TEntidade, double?>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados).Max(propriedade);
        }

        public DateTime Max(Expression<Func<TEntidade, DateTime>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Max(propriedade);
        }

        public DateTime? Max(Expression<Func<TEntidade, DateTime?>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Max(propriedade);
        }
        #endregion

        #region Min

        public int Min(Expression<Func<TEntidade, int>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Min(propriedade);
        }

        public int? Min(Expression<Func<TEntidade, int?>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Min(propriedade);
        }

        public long Min(Expression<Func<TEntidade, long>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Min(propriedade);
        }

        public decimal Min(Expression<Func<TEntidade, decimal>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Min(propriedade);
        }

        public decimal? Min(Expression<Func<TEntidade, decimal?>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Min(propriedade);
        }

        public double Min(Expression<Func<TEntidade, double>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Min(propriedade);
        }

        public double? Min(Expression<Func<TEntidade, double?>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Min(propriedade);
        }

        public DateTime Min(Expression<Func<TEntidade, DateTime>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Min(propriedade);
        }

        public DateTime? Min(Expression<Func<TEntidade, DateTime?>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados).Min(propriedade);
        }
        #endregion

        #region Average

        public int Average(Expression<Func<TEntidade, int>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Average(propriedade);
        }

        public int? Average(Expression<Func<TEntidade, int?>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Average(propriedade);
        }

        public long Average(Expression<Func<TEntidade, long>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Average(propriedade);
        }

        public decimal Average(Expression<Func<TEntidade, decimal>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Average(propriedade);
        }

        public decimal? Average(Expression<Func<TEntidade, decimal?>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Average(propriedade);
        }

        public double Average(Expression<Func<TEntidade, double>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Average(propriedade);
        }

        public double? Average(Expression<Func<TEntidade, double?>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Average(propriedade);
        }

        public DateTime Average(Expression<Func<TEntidade, DateTime>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Average(propriedade);
        }

        public DateTime? Average(Expression<Func<TEntidade, DateTime?>> propriedade)
        {
            return new ConsultaEntidade<TEntidade>(this.ContextoDados, this.TipoEntidade).Average(propriedade);
        }

        #endregion

        #endregion

        public EstruturaConsulta RetornarEstruturaConsulta()
        {
            var consulta = new ConsultaEntidade<TEntidade>(this.ContextoDados);
            return consulta.RetornarEstruturaConsulta();
        }

        public ConsultaEntidade<TEntidade> IncluirDeletados()
        {
            var consulta = new ConsultaEntidade<TEntidade>(this.ContextoDados);
            return consulta.IncluirDeletados();
        }
        //public TEntidade RetornarPorId(long id)
        //{
        //    var consulta = new ConsultaEntidade<TEntidade>(this.ContextoDados);
        //    return consulta.RetornarPorId(id);
        //}
    }
}