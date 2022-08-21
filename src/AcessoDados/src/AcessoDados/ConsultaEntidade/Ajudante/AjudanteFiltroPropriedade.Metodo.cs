﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur;
using Snebur.Utilidade;
using System.Linq.Expressions;
using System.Reflection;

namespace Snebur.AcessoDados.Ajudantes
{
    internal partial class AjudanteFiltroPropriedade
    {

        public static FiltroPropriedade RetornarFiltroPropriedadeMetodo(EstruturaConsulta estruturaConsulta, MethodCallExpression expressao, StringComparison comparacao)
        {
            var argumento = expressao.Arguments.First();
            var valorPropriedade = AjudanteFiltroPropriedade.RetornarValorPropriedade(argumento);
            var operadorFiltro = AjudanteFiltroPropriedade.RetornarOperadorFiltroMetodo(expressao.Method, comparacao);

            return AjudanteFiltroPropriedade.RetornarFiltroPropriedade(estruturaConsulta, (MemberExpression)expressao.Object, valorPropriedade, operadorFiltro);
        }

        public static EnumOperadorFiltro RetornarOperadorFiltroMetodo(MethodInfo metodo, StringComparison comparacao)
        {
            switch (metodo.Name)
            {
                case "StartsWith":

                    return EnumOperadorFiltro.IniciaCom;

                case "EndsWith":

                    return EnumOperadorFiltro.TerminaCom;

                case "Contains":

                    return EnumOperadorFiltro.Possui;

                case "Equals":

                    var isComparacaoAbsoluta = AjudanteFiltroPropriedade.IsComparacaoAbsoluta(comparacao);
                    return isComparacaoAbsoluta ? EnumOperadorFiltro.IgualAbsoluto : EnumOperadorFiltro.Igual;

                default:
                    throw new ErroNaoSuportado(String.Format("Método não suportado {0}", metodo.Name));
            }
        }

        private static bool IsComparacaoAbsoluta(StringComparison comparacao)
        {
            switch (comparacao)
            {
                case StringComparison.CurrentCulture:
                case StringComparison.InvariantCulture:
                case StringComparison.Ordinal:

                    return true;

                case StringComparison.CurrentCultureIgnoreCase:
                case StringComparison.OrdinalIgnoreCase:
                case StringComparison.InvariantCultureIgnoreCase:

                    return false;

                default:

                    throw new ErroNaoSuportado(String.Format("A comparação não é suportada {0} ", EnumUtil.RetornarDescricao(comparacao)));
            }
        }
    }
}