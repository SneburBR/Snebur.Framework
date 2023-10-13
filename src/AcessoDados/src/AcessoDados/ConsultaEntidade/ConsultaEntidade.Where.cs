using Snebur.AcessoDados.Ajudantes;
using Snebur.Dominio;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Snebur.AcessoDados
{
    public partial class ConsultaEntidade<TEntidade> where TEntidade : IEntidade
    {
        public TEntidade Find(long id)
        {
            return this.Where(x => x.Id == id).Single();
        }

        public TEntidade FindOrDefault(long id)
        {
            return this.Where(x => x.Id == id).SingleOrDefault();
        }

        public bool Exists(long id)
        {
            return this.Where(x => x.Id == id).SingleOrDefault() != null;
        }

        public ConsultaEntidade<TEntidade> Where(Expression<Func<TEntidade, bool>> filtro)
        {
            Expression expressao = (Expression)filtro;
            this.AdicionarFiltro(this.EstruturaConsulta, expressao, this.EstruturaConsulta.FiltroGrupoE);
            return this;
        }

        public ConsultaEntidade<TEntidade> WhereOr(Expression<Func<TEntidade, bool>> filtro)
        {
            Expression expressao = (Expression)filtro;
            this.AdicionarFiltro(this.EstruturaConsulta, expressao, this.EstruturaConsulta.FiltroGrupoOU);
            return this;
        }

        public ConsultaEntidade<TEntidade> WhereIds(List<long> ids)
        {
            this.EstruturaConsulta.FiltroGrupoE.Filtros.Add(new FiltroIds(ids.ToList()));
            return this;
        }

        public ConsultaEntidade<TEntidade> WhereIn(Expression<Func<TEntidade, string>> expressaoPropriedade, IEnumerable<string> lista)
        {
            return this.WhereInInterno(expressaoPropriedade, lista.Cast<string>().ToList());
        }

        public ConsultaEntidade<TEntidade> WhereIn(Expression<Func<TEntidade, long>> expressaoPropriedade, IEnumerable<long> lista)
        {
            return this.WhereInInterno(expressaoPropriedade, lista.Select(x => x.ToString()).ToList());
        }

        public ConsultaEntidade<TEntidade> WhereIn(Expression<Func<TEntidade, long?>> expressaoPropriedade, IEnumerable<long> lista)
        {
            return this.WhereInInterno(expressaoPropriedade, lista.Select(x => x.ToString()).ToList());
        }

        public ConsultaEntidade<TEntidade> WhereIn(Expression<Func<TEntidade, Enum>> expressaoPropriedade, IEnumerable<int> lista)
        {
            return this.WhereInInterno(expressaoPropriedade, lista.Cast<string>().ToList());
        }

        private ConsultaEntidade<TEntidade> WhereInInterno(Expression expressaoPropriedade, List<string> lista)
        {
            var caminhoPropriedade = this.RetornarCaminhoPropriedade(expressaoPropriedade);
            this.EstruturaConsulta.FiltroGrupoE.Filtros.Add(new FiltroPropriedadeIn(caminhoPropriedade, lista));
            return this;
        }

        public ConsultaEntidade<TEntidade> AdicionarFiltroPropriedade(PropertyInfo propriedade, EnumOperadorFiltro operador, object valor)
        {
            var filtroPropriedade = AjudanteFiltroPropriedade.RetornarFiltroCaminhoPropriedadePropriedade(this.EstruturaConsulta, propriedade, propriedade.Name, valor, operador);
            this.EstruturaConsulta.FiltroGrupoE.Filtros.Add(filtroPropriedade);

            //if (propriedade.PropertyType.IsSubclassOf(typeof(Entidade)))
            //{
            //    this.AbrirRelacaoFiltro(this.ConsultaAcessoDados, filtroPropriedade.CaminhoPropriedade);
            //}
            return this;
        }

        private void AdicionarFiltro(EstruturaConsulta estruturaConsulta, Expression expressao, BaseFiltroGrupo grupoFiltroAtual)
        {
            if (expressao is LambdaExpression)
            {
                var lambdaExpression = (LambdaExpression)expressao;
                expressao = lambdaExpression.Body;
            }
            if (expressao != null)
            {
                switch (expressao.NodeType)
                {
                    case ExpressionType.NotEqual:
                    case ExpressionType.Equal:
                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:

                        var logicalBinaryExpression = (BinaryExpression)expressao;
                        var filtroPropriedade = AjudanteFiltroPropriedade.RetornarFiltroPropriedadeLogico(estruturaConsulta, logicalBinaryExpression);

                        //filtroPropriedade.CaminhoPropriedade
                        this.AbrirRelacaoFiltro(estruturaConsulta, filtroPropriedade.CaminhoPropriedade);

                        grupoFiltroAtual.Filtros.Add(filtroPropriedade);

                        expressao = logicalBinaryExpression.Conversion;

                        if (expressao != null)
                        {
                            throw new NotSupportedException(" Isso não era esperado ");
                        }
                        break;

                    case ExpressionType.Call:
                        {
                            var expressaoMetodo = (MethodCallExpression)expressao;

                            switch (expressaoMetodo.Method.Name)
                            {
                                case "EndsWith":
                                case "StartsWith":
                                case "Contains":
                                case "Equals":

                                    var objeto = expressaoMetodo.Object;

                                    if (!(expressaoMetodo.Object is MemberExpression))
                                    {
                                        var msgNumeroArgumentosNaoSuportado = String.Format("A o objeto {0} no método {1} na expressão {2} não é suportado ", expressaoMetodo.Object.ToString(), expressaoMetodo.Method.Name, expressao.ToString());
                                        throw new ErroNaoSuportado(msgNumeroArgumentosNaoSuportado);
                                    }
                                    var comparacao = StringComparison.InvariantCulture;
                                    if (expressaoMetodo.Arguments.Count() == 2)
                                    {
                                        var argumento2 = expressaoMetodo.Arguments.Last();
                                        if (!(argumento2 is ConstantExpression))
                                        {
                                            throw new ErroNaoSuportado(String.Format("O argumento '{0}' não é suportado.", argumento2.GetType().Name));
                                        }
                                        var valor = ((ConstantExpression)argumento2).Value;
                                        if (valor.GetType() != typeof(StringComparison))
                                        {
                                            throw new ErroNaoSuportado(String.Format("O valor do argumento '{0}' não é suportado.", valor.GetType().Name));
                                        }
                                        comparacao = (StringComparison)valor;
                                    }
                                    if (expressaoMetodo.Arguments.Count() > 2)
                                    {
                                        var msgNumeroArgumentosNaoSuportado = String.Format("A o numero de argumentos {0} no método {1} na expressão {2} não é suportado ", expressaoMetodo.Arguments.Count(), expressaoMetodo.Method.Name, expressao.ToString());
                                        throw new ErroNaoSuportado(msgNumeroArgumentosNaoSuportado);
                                    }
                                    var filtroPropriedadeMetodo = Ajudantes.AjudanteFiltroPropriedade.RetornarFiltroPropriedadeMetodo(estruturaConsulta, expressaoMetodo, comparacao);
                                    grupoFiltroAtual.Filtros.Add(filtroPropriedadeMetodo);
                                    this.AbrirRelacaoFiltro(estruturaConsulta, filtroPropriedadeMetodo.CaminhoPropriedade);
                                    break;

                                default:

                                    var msgMetodoNaoSuportada = String.Format("A o método {0} na expressão {1} não é suportado ", expressaoMetodo.Method.Name, expressao.ToString());
                                    throw new ErroNaoSuportado(msgMetodoNaoSuportada);
                            }
                            break;
                        }
                    case ExpressionType.MemberAccess:

                        //Esperamos uma expressão boolean
                        var expressaoMembro = (MemberExpression)(expressao);
                        if (expressaoMembro.Type == typeof(Boolean))
                        {
                            var filtroVerdadeiro = Ajudantes.AjudanteFiltroPropriedade.RetornarFiltroPropriedadeVardadeiro(estruturaConsulta, expressaoMembro);
                            grupoFiltroAtual.Filtros.Add(filtroVerdadeiro);
                            this.AbrirRelacaoFiltro(estruturaConsulta, filtroVerdadeiro.CaminhoPropriedade);
                            break;
                        }
                        var msgExpressaoMemberAccessNaoSuportada = String.Format("A expressão MemberAccess não é suportado {0} do tipo {1} ", EnumUtil.RetornarDescricao(expressao.NodeType), expressaoMembro.Type.Name);

                        throw new ErroNaoSuportado(msgExpressaoMemberAccessNaoSuportada);

                    case ExpressionType.And:
                    case ExpressionType.AndAlso:

                        var expressaoE = (BinaryExpression)expressao;

                        var expressaoEsquerdaE = expressaoE.Left;
                        var expressaoDireitaE = expressaoE.Right;

                        var filtroGrupoE = new FiltroGrupoE();
                        grupoFiltroAtual.Filtros.Add(filtroGrupoE);

                        this.AdicionarFiltro(estruturaConsulta, expressaoEsquerdaE, filtroGrupoE);
                        this.AdicionarFiltro(estruturaConsulta, expressaoDireitaE, filtroGrupoE);

                        break;

                    case ExpressionType.Or:
                    case ExpressionType.OrElse:

                        var expressaoOU = (BinaryExpression)expressao;

                        var expressaoEsquerdaOU = expressaoOU.Left;
                        var expressaoDireitaOU = expressaoOU.Right;

                        var filtroGrupoOU = new FiltroGrupoOU();

                        this.AdicionarFiltro(estruturaConsulta, expressaoEsquerdaOU, filtroGrupoOU);
                        this.AdicionarFiltro(estruturaConsulta, expressaoDireitaOU, filtroGrupoOU);
                        grupoFiltroAtual.Filtros.Add(filtroGrupoOU);
                        break;

                    case ExpressionType.Not:

                        var expressaoNao = (UnaryExpression)expressao;

                        var filtroGroupoNao = new FiltroGrupoNAO();
                        grupoFiltroAtual.Filtros.Add(filtroGroupoNao);

                        var expressaoInterna = expressaoNao.Operand;

                        this.AdicionarFiltro(estruturaConsulta, expressaoInterna, filtroGroupoNao);

                        break;

                    default:

                        var msgExpressaoNaoSuportada = String.Format("A expressão não é suportado {0} ", EnumUtil.RetornarDescricao(expressao.NodeType));
                        throw new ErroNaoSuportado(msgExpressaoNaoSuportada);
                }
            }
        }


    }
}